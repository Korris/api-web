namespace Mcsg.Comic.Api.Validators;

using Common.SeedWork.Exceptions;
using Constants;
using Lib.Common.Interfaces;
using Lib.Data.Domain.Entities;
using Lib.Data.Repositories;

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
