namespace Mcsg.Story.Api.Validators;

using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Lib.Common.Interfaces;
using Lib.Data.Repositories;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

public class PostReportValidator : IValidator<PostReport>
{
    private readonly IRepository<StoryPost> _postRepository;
    public PostReportValidator(IRepository<StoryPost> postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task OnValidate(PostReport data)
    {
        _ = await _postRepository.GetByIdAsync(data.PostId, "\"Id\"") ?? throw new BadRequestException(E204, M204);
    }
}
