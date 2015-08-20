namespace EdugameCloud.WCFService.Validators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.Lti.Domain.Entities;
    using EdugameCloud.Lti.DTO;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using FluentValidation;

    /// <summary>
    /// The company LMS validator.
    /// </summary>
    public sealed class CompanyLmsValidator : AbstractValidator<CompanyLmsDTO>
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
                .Must(
                    (model, x) =>
                    x.Equals(LmsProviderNames.Canvas, StringComparison.OrdinalIgnoreCase)
                    || x.Equals(LmsProviderNames.Brightspace, StringComparison.OrdinalIgnoreCase)
                    || model.enableProxyToolMode
                    || !string.IsNullOrWhiteSpace(model.lmsAdmin))
                .WithError(
                    Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                    "Invalid LMS Setup. Please provide with administrative Username and Password");

            this.RuleFor(model => model.additionalLmsDomains)
               .Must((model, x) => UniqueLmsDomains(model.lmsDomain, x))
               .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "LMS domains should be unique.");

        }

        private static bool UniqueLmsDomains(string mainLmsDomain, string[] additionalDomains)
        {
            if (additionalDomains == null)
                additionalDomains = new string[0];

            var domains = new List<string>(additionalDomains);
            domains.Add(mainLmsDomain);
            return domains.Count == domains.Distinct().Count();
        }

    }

}