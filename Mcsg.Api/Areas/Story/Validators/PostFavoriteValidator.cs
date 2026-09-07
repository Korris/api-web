namespace Mcsg.Api.Areas.Story.Validators;

using Common.Domain.Entities;
using Common.Interfaces;
using Common.SeedWork.Exceptions;
using static Common.SeedWork.Constants.Error;

public class PostFavoriteValidator : IValidator<StoryPostFavorite>
{
    private readonly IRepository<StoryPost> _postRepository;
    public PostFavoriteValidator(IRepository<StoryPost> postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task OnValidate(StoryPostFavorite data)
    {
        _ = await _postRepository.GetByIdAsync(data.PostId, "\"Id\"") ?? throw new BadRequestException(nameof(E204), E204);
    }
}
