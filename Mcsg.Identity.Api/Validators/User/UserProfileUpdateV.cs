using FluentValidation;

namespace Mcsg.Identity.Api.Validators.User
{
    /// <summary>
    /// Validator
    /// </summary>
    /// 
    using Common.SeedWork.Extensions;
    using Identity.Api.Requests;
    using static Common.SeedWork.Constants.Validator;
    public class UserProfileUpdateV : AbstractValidator<UserProfileUpdateR>
    {
        #region -- Methods --

        /// <summary>
        /// Initialize
        /// </summary>
        public UserProfileUpdateV()
        {
            var t = nameof(ProfileName);
            RuleFor(p => p.ProfileName.Triz())
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage($"{t} {NotEmpty}")
                .MinimumLength(ProfileName.Min).WithMessage($"{t} {MinimumLength} {ProfileName.Min}")
                .MaximumLength(ProfileName.Max).WithMessage($"{t} {MaximumLength} {ProfileName.Max}")
                .Matches(ProfileName.Regex).WithMessage($"{t} {ProfileName.Message}");

            When(p => p.IsPremium, () =>
            {
                t = nameof(UserNamePremium);
                RuleFor(p => p.UserName.Triz())
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage($"{t} {NotEmpty}")
                    .MinimumLength(UserNamePremium.Min).WithMessage($"{t} {MinimumLength} {UserNamePremium.Min}")
                    .MaximumLength(UserNamePremium.Max).WithMessage($"{t} {MaximumLength} {UserNamePremium.Max}")
                    .Matches(UserNamePremium.Regex).WithMessage($"{t} {UserNamePremium.Message}");
            });

            When(p => !p.IsPremium, () =>
            {
                t = nameof(UserNameFree);
                RuleFor(p => p.UserName.Triz())
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage($"{t} {NotEmpty}")
                    .MinimumLength(UserNameFree.Min).WithMessage($"{t} {MinimumLength} {UserNameFree.Min}")
                    .MaximumLength(UserNameFree.Max).WithMessage($"{t} {MaximumLength} {UserNameFree.Max}")
                    .Matches(UserNameFree.Regex).WithMessage($"{t} {UserNameFree.Message}");
            });

            t = nameof(Location);
            RuleFor(p => p.Location.Triz())
                .Cascade(CascadeMode.Stop)
                .MaximumLength(Location.Max).WithMessage($"{t} {MaximumLength} {Location.Max}")
                .Matches(Location.Regex).WithMessage($"{t} {Location.Message}");
        }

        #endregion
    }
}
