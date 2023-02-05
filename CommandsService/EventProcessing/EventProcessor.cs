using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dto;
using CommandsService.Model;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IMapper _mapper;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            this._serviceScopeFactory = scopeFactory;
            this._mapper = mapper;
        }

        public void ProcessEvent(string message)
        {
            var eventType = this.DetermineEventType(message);
            switch (eventType)
            {
                case EventType.PlatformPublished:
                    this.AddPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private EventType DetermineEventType(string notificationMessage)
        {
            Console.WriteLine($"--> Determining Event: {notificationMessage}");
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
            switch (eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("--> Platform Publish Event Detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("--> Could not determine received event type");
                    return EventType.Undetermined;
            }
        }

        private void AddPlatform(string platformPublishedMessage)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var commandRepository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
                var platformPublishDto = JsonSerializer.Deserialize<PlatformPublishDto>(platformPublishedMessage);
                try
                {
                    var platform = _mapper.Map<Platform>(platformPublishDto);
                    if (!commandRepository.ExternalPlatformExists(platform.ExternalId))
                    {
                        commandRepository.CreatePlatform(platform);
                        commandRepository.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine("Platform already exists...");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not add Platform to DB: {ex.Message}");
                }
            }
        }
    }

    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}