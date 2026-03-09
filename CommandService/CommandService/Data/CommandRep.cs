using CommandService.Models;
using System;

namespace CommandService.Data
{
    public class CommandRep : ICommandRepo
    {
        private readonly AppDbContext _context;

        public CommandRep(AppDbContext context)
        {
            _context = context;
        }
        public void CreateCommand(int platformId, Command command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            command.PlatformId = platformId;
            _context.Commands.Add(command);
        }

        public void CreatePlatform(Platform platform)
        {
            if (platform == null)
            {
                throw new ArgumentNullException(nameof(platform));
            }
            _context.Platforms.Add(platform);
        }

        public bool ExternalPlatformExist(int externalPlatformId)
        {
            return _context.Platforms.Any(x => x.ExternalId == externalPlatformId);
        }

        public Command GetCommand(int platformId, int commandId)
        {
            return _context.Commands.FirstOrDefault(x => x.Id == commandId && x.PlatformId == platformId);
        }

        public IEnumerable<Command> GetCommandsForPlatform(int platformId)
        {
            return _context.Commands.Where(x => x.PlatformId == platformId);
        }

        public IEnumerable<Platform> GetPlatforms()
        {
            return _context.Platforms.ToList();
        }

        public bool PlatformExists(int platformId)
        {
            return _context.Platforms.Any(x => x.Id == platformId);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}