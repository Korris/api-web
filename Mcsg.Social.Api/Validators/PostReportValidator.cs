namespace Mcsg.Social.Api.Validators
{
    using Common.SeedWork.Exceptions;
    using Constants;
    using Lib.Common.Interfaces;
    using Lib.Data.Domain.Entities;
    using Lib.Data.Repositories;

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
