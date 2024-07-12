namespace Mcsg.Social.Api.Validators;

using Common.SeedWork.Exceptions;
using Lib.Common.Interfaces;
using Lib.Data.Domain.Entities;
using Lib.Data.Repositories;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

public class PostFavoriteValidator : IValidator<PostFavorite>
{
    private readonly IRepository<Post> _postRepository;
    public PostFavoriteValidator(IRepository<Post> postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task OnValidate(PostFavorite data)
    {
        _ = await _postRepository.GetByIdAsync(data.PostId, "\"Id\"") ?? throw new BadRequestException(E204, M204);
    }
}
