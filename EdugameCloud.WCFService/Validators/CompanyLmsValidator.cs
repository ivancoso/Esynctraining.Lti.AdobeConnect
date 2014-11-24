namespace EdugameCloud.WCFService.Validators
{
    using System;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    using FluentValidation;

    /// <summary>
    /// The company LMS validator.
    /// </summary>
    public class CompanyLmsValidator : AbstractValidator<CompanyLmsDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyLmsValidator"/> class.
        /// </summary>
        /// <param name="lmsProviderModel">
        /// The LMS Provider Model.
        /// </param>
        public CompanyLmsValidator(LmsProviderModel lmsProviderModel)
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(x => x.acUsername).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Adobe Connect username is empty");
            this.RuleFor(x => x.acServer).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Adobe Connect server is empty");
            this.RuleFor(x => x.lmsProvider)
                .NotEmpty()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Lms provider is empty")

                .Must(x => lmsProviderModel.GetOneByName(x).Value != null)
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Invalid LMS Provider Name")

                .Must((model, x) => 
                    x.Equals(LmsProviderNames.Canvas, StringComparison.OrdinalIgnoreCase)
                    || (!string.IsNullOrWhiteSpace(model.lmsAdmin) && !string.IsNullOrWhiteSpace(model.lmsAdminPassword)))
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Invalid LMS setup. Please provide with LMS Admin and Password");
        }
    }
}