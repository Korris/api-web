namespace Mcsg.Story.Api.Validators;

using Common.Domain.Entities;
using Common.Interfaces;
using Common.SeedWork.Exceptions;
using Lib.Common.Interfaces;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

public class PostFavoriteValidator : IValidator<StoryPostFavorite>
{
    private readonly IRepository<StoryPost> _postRepository;
    public PostFavoriteValidator(IRepository<StoryPost> postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task OnValidate(StoryPostFavorite data)
    {
        _ = await _postRepository.GetByIdAsync(data.PostId, "\"Id\"") ?? throw new BadRequestException(E204, M204);
    }
}
