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
            this.RuleFor(x => x.companyId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Company is empty");
            this.RuleFor(x => x.acPassword).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Adobe Connect password is empty");
            this.RuleFor(x => x.acUsername).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Adobe Connect username is empty");
            this.RuleFor(x => x.acServer).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Adobe Connect server is empty");
            this.RuleFor(x => x.createdBy).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Created by is empty");
            this.RuleFor(x => x.lmsProvider).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Lms provider is empty");
        }
    }
}