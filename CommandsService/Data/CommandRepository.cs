using CommandsService.Model;

namespace CommandsService.Data
{
    public class CommandRepository : ICommandRepository
    {
        private readonly AppDbContext _dbContext;

        public CommandRepository(AppDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public void CreateCommand(int platformId, Command command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            command.PlatformId = platformId;
            _dbContext.Commands.Add(command);
        }

        public void CreatePlatform(Platform platform)
        {
            if (platform == null)
            {
                throw new ArgumentNullException(nameof(platform));
            }
            _dbContext.Platforms.Add(platform);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _dbContext.Platforms.ToList();
        }

        public Command GetCommand(int platformId, int commandId)
        {
            return _dbContext
                .Commands
                .FirstOrDefault(c => c.PlatformId == platformId && c.Id == commandId);
        }

        public IEnumerable<Command> GetCommandsForPlatform(int platformId)
        {
            return _dbContext
                .Commands
                .Where(c => c.PlatformId == platformId)
                .OrderBy(c => c.Platform.Name);
        }

        public bool PlatformExists(int platformId)
        {
            return _dbContext.Platforms.Any(p => p.Id == platformId);
        }

        public bool SaveChanges() => _dbContext.SaveChanges() >= 0;

        public bool ExternalPlatformExists(int externalPlatformId) => _dbContext.Platforms.Any(p => p.ExternalId == externalPlatformId);
    }
}