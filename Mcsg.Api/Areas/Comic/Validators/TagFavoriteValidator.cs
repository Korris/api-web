namespace Mcsg.Api.Areas.Comic.Validators;

using Common.Domain.Entities;
using Common.Interfaces;
using Common.SeedWork.Exceptions;
using Mcsg.Api.Areas.Comic.Constants;

public class TagFavoriteValidator : IValidator<TagFavorite>
{
    private readonly IRepository<Tag> _tagRepository;
    public TagFavoriteValidator(IRepository<Tag> tagRepository)
    {
        _tagRepository = tagRepository;
    }
    public async Task OnValidate(TagFavorite data)
    {
        _ = await _tagRepository.GetByIdAsync(data.TagId, "\"Id\"") ?? throw new BadRequestException(ApiErrorCode.TAG_NOT_EXIST, ApiErrorMessage.TAG_NOT_EXIST);
    }
}
