using CommandsService.Model;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data
{
    public static class PrepareDb
    {
        public static void PreparePopulation(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
                var platforms = grpcClient.ReturnAllPlatforms();
                SeedData(serviceScope.ServiceProvider.GetService<ICommandRepository>(), platforms);
            }
        }

        private static void SeedData(ICommandRepository commandRepository, IEnumerable<Platform> platforms)
        {
            Console.WriteLine("--> Seeding new platforms");
            foreach (var platform in platforms)
            {
                if (!commandRepository.ExternalPlatformExists(platform.ExternalId))
                {
                    commandRepository.CreatePlatform(platform);
                    commandRepository.SaveChanges();
                }
            }
        }
    }
}