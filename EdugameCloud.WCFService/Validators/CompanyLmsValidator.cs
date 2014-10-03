namespace EdugameCloud.WCFService.Validators
{
    using System;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    using FluentValidation;

    public class CompanyLmsValidator : AbstractValidator<CompanyLmsDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileDtoValidator"/> class.
        /// </summary>
        public CompanyLmsValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(x => x.dateCreated).NotEqual(default(DateTime)).WithError(Errors.CODE_ERRORTYPE_INVALID_PARAMETER, "Date is invalid");
            
        }
    }
}