using AutoMapper;
using Grpc.Core;
using PlatformService.Data;

namespace PlatformService.SyncDataServices.Grpc
{
    public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
    {
        private readonly IPlatformRepository _platformRepository;
        private readonly IMapper _mapper;

        public GrpcPlatformService(IPlatformRepository platformRepository, IMapper mapper)
        {
            this._platformRepository = platformRepository;
            this._mapper = mapper;
        }

        public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
        {
            var response = new PlatformResponse();
            var platforms = _platformRepository.GetAllPlatforms();

            foreach (var platform in platforms)
            {
                response.Platforms.Add(_mapper.Map<GrpcPlatformModel>(platform));
            }

            return Task.FromResult(response);
        }
    }
}