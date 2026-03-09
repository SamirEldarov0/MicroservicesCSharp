using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(IPlatformRepo repository, IMapper mapper, ICommandDataClient commandDataClient, IMessageBusClient messageBusClient)
        {
            _repository = repository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetAll()
        {
            Console.WriteLine("Getting platforms...");

            var platforms = _repository.GetAllPlatforms();

            var result = _mapper.Map<IEnumerable<PlatformReadDto>>(platforms);

            return Ok(result);
        }

        [HttpGet("{id}", Name = "detail")]
        public ActionResult<PlatformReadDto> Detail(int id)
        {
            var platform = _repository.GetPlatformById(id);
            if (platform == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PlatformReadDto>(platform));
        }

        [HttpPost("create-platform")]
        public async Task<ActionResult<PlatformReadDto>> CreateAsync(PlatformCreateDto platformCreateDto)
        {
            var platform = _mapper.Map<Platform>(platformCreateDto);
            _repository.CreatePlatform(platform);
            _repository.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platformCreateDto);

            //Send sync message
            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Couldn't send {ex.Message}");
            }
            //Send async message
            try
            {
                var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
                platformPublishedDto.Event = "Platform_Published";
                await _messageBusClient.PublishNewPlatformAsync(platformPublishedDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Couldn't send {ex.Message}");
            }

            return CreatedAtRoute(nameof(Detail), new { Id = platformReadDto.Id, platformReadDto });
        }
    }
}