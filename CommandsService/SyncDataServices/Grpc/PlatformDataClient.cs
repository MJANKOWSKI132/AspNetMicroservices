using AutoMapper;
using CommandsService.Model;
using Grpc.Net.Client;
using PlatformService;

namespace CommandsService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public PlatformDataClient(IConfiguration config, IMapper mapper)
        {
            this._config = config;
            this._mapper = mapper;
        }

        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            Console.WriteLine($"--> Calling GRPC Service {_config["GrpcPlatform"]}");
            var channel = GrpcChannel.ForAddress(_config["GrpcPlatform"]);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();

            try
            {
                var reply = client.GetAllPlatforms(request);
                return _mapper.Map<IEnumerable<Platform>>(reply.Platforms);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not call GRPC Server: {ex.Message}");
                return new List<Platform>();
            }
        }
    }
}