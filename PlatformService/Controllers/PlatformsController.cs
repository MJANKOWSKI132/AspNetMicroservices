using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dto;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepository _platformRepository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(IPlatformRepository platformRepository,
                                   IMapper mapper,
                                   ICommandDataClient commandDataClient,
                                   IMessageBusClient messageBusClient)
        {
            this._platformRepository = platformRepository;
            this._mapper = mapper;
            this._commandDataClient = commandDataClient;
            this._messageBusClient = messageBusClient;
        }

        [HttpGet(Name = nameof(GetPlatforms))]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("--> Getting Platforms...");
            var platformItems = _platformRepository.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }

        [HttpGet("{id}", Name = nameof(GetPlatformById))]
        public ActionResult<PlatformReadDto> GetPlatformById([FromRoute] int id)
        {
            var platform = _platformRepository.GetPlatformById(id);
            if (platform == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<PlatformReadDto>(platform));
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform([FromBody] PlatformCreateDto platformCreateRequest)
        {
            if (platformCreateRequest == null)
            {
                return BadRequest();
            }
            var platform = _mapper.Map<Platform>(platformCreateRequest);
            _platformRepository.CreatePlatform(platform);
            _platformRepository.SaveChanges();
            var platformReadDto = _mapper.Map<PlatformReadDto>(platform);

            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
            }

            try
            {
                var platformPublishDto = _mapper.Map<PlatformPublishDto>(platformReadDto);
                platformPublishDto.Event = "Platform_Published";
                this._messageBusClient.PublishNewPlatform(platformPublishDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
            }

            return CreatedAtRoute(nameof(GetPlatformById), new { Id = platform.Id }, platformReadDto);
        }
    }
}