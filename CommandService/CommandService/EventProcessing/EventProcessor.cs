using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using System.Text.Json;

namespace CommandService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeServiceFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeServiceFactory, IMapper mapper)
        {
            _scopeServiceFactory = scopeServiceFactory;
            _mapper = mapper;
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    addPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("Determining event");
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
            switch (eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine(" --> Platform Published Event Detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("Couldn't determine the event type");
                    return EventType.Undetermined;
            }
        }

        private void addPlatform(string platformPublishedMessage)
        {
            using (var scope =  _scopeServiceFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

                try
                {
                    var platform = _mapper.Map<Platform>(platformPublishedDto);
                    if (!repo.PlatformExists(platform.ExternalId))
                    {
                        repo.CreatePlatform(platform);
                        repo.SaveChanges();
                        Console.WriteLine(" --> Platform added");
                    }
                    else
                    {
                        Console.WriteLine(" --> Platform already exists");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("--> Couldn't add platform to DB...", ex.Message);
                }

            }
        }

        enum EventType
        {
            PlatformPublished,
            Undetermined
        }
    }
}
