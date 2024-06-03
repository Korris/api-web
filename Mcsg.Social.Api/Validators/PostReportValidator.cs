using Mcsg.Api.Constants;
using Mcsg.Lib.Common.Exceptions;
using Mcsg.Lib.Common.Interfaces;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Repositories;

namespace Mcsg.Api.Validators
{
    public class PostReportValidator : IValidator<PostReport>
    {
        private readonly IRepository<Post> _postRepository;
        public PostReportValidator(IRepository<Post> postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task OnValidate(PostReport data)
        {
            _ = await _postRepository.GetByIdAsync(data.PostId, "\"Id\"") ?? throw new BadRequestException(ApiErrorCode.POST_NOT_EXIST, ApiErrorMessage.POST_NOT_EXIST);
        }
    }
}
