namespace EdugameCloud.MVC.Validators
{
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.MVC.ViewModels;
    using Esynctraining.Core.Extensions;
    using FluentValidation;

    /// <summary>
    ///     The ImportUsers view model validator.
    /// </summary>
    public class ImportUsersViewModelValidator : AbstractValidator<ImportUsersViewModel>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportUsersViewModelValidator"/> class.
        /// </summary>
        /// <param name="companyModel">
        /// The company Model.
        /// </param>
        public ImportUsersViewModelValidator(CompanyModel companyModel)
        {
            this.RuleFor(x => x.ProfilesFile).NotEmpty().WithMessage("File is null or empty");
            this.RuleFor(x => x.CompanyId)
                .NotEmpty()
                .WithMessage("Company is empty")
                .Must(x => companyModel.GetOneById(x).Value != null)
                .WithMessage("Company doesn't exist");
        }

        #endregion
    }
}