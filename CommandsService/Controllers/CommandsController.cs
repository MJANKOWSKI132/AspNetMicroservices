using AutoMapper;
using CommandsService.Data;
using CommandsService.Dto;
using CommandsService.Model;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controller
{
    [ApiController]
    [Route("/api/c/platforms/{platformId}/[controller]")]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepository _commandRepository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepository commandRepository, IMapper mapper)
        {
            this._commandRepository = commandRepository;
            this._mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform([FromRoute] int platformId)
        {
            Console.WriteLine($"--> Retrieving Commands for Platform with ID: {platformId}");
            var platformExists = _commandRepository.PlatformExists(platformId);
            if (!platformExists)
            {
                return NotFound($"Error, no such platform exists with ID: {platformId}");
            }
            var commands = _commandRepository.GetCommandsForPlatform(platformId);
            var commandReadDtoList = _mapper.Map<IEnumerable<CommandReadDto>>(commands);
            return Ok(commandReadDtoList);
        }

        [HttpGet("{commandId}", Name = nameof(GetCommandForPlaform))]
        public ActionResult<CommandReadDto> GetCommandForPlaform([FromRoute] int platformId, [FromRoute] int commandId)
        {
            Console.WriteLine($"--> Retrieving Command with ID: {commandId} for Platform with ID: {platformId}");
            var platformExists = _commandRepository.PlatformExists(platformId);
            if (!platformExists)
            {
                return NotFound($"Error, no such platform exists with ID: {platformId}");
            }
            var command = _commandRepository.GetCommand(platformId, commandId);
            if (command == null)
            {
                return NotFound($"Error, no such Command with ID: {commandId} exists for the Platform with ID: {platformId}");
            }
            var response = _mapper.Map<CommandReadDto>(command);
            return Ok(response);
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommand([FromRoute] int platformId, [FromBody] CommandCreateDto commandCreateRequest)
        {
            Console.WriteLine($"--> Creating command for the Platform with ID: {platformId}");
            var platformExists = _commandRepository.PlatformExists(platformId);
            if (!platformExists)
            {
                return NotFound($"Error, no such platform exists with ID: {platformId}");
            }
            var command = _mapper.Map<Command>(commandCreateRequest);
            _commandRepository.CreateCommand(platformId, command);
            var changesSaved = _commandRepository.SaveChanges();
            if (!changesSaved)
            {
                return BadRequest($"An error occurred whilst saving the created command");
            }
            var response = _mapper.Map<CommandReadDto>(command);
            return CreatedAtAction(nameof(GetCommandForPlaform), new { platformId = platformId, commandId = response.Id }, response);
        }
    }
}