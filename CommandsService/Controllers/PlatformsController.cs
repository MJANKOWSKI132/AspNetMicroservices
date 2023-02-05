using AutoMapper;
using CommandsService.Data;
using CommandsService.Dto;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controller
{
    [ApiController]
    [Route("/api/c/[controller]")]
    public class PlatformsController : ControllerBase
    {
        private readonly ICommandRepository _commandRepository;
        private readonly IMapper _mapper;

        public PlatformsController(ICommandRepository commandRepository, IMapper mapper)
        {
            this._commandRepository = commandRepository;
            this._mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("--> Getting Platforms from CommandService");
            var platforms = _commandRepository.GetAllPlatforms();
            var platformReadDtoList = _mapper.Map<IEnumerable<PlatformReadDto>>(platforms);
            return Ok(platformReadDtoList);
        }

        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            Console.WriteLine("--> Inbound POST # Command Service");
            return Ok("Inbound test ok from Platforms Controller");
        }
    }
}