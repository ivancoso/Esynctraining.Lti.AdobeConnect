namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;
    using Esynctraining.Core.Enums;
    using Esynctraining.FluentValidation;

    using FluentValidation;

    /// <summary>
    /// The BatchUsersDTO validator.
    /// </summary>
    public sealed class BatchUsersDTOValidator : AbstractValidator<BatchUsersDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BatchUsersDTOValidator"/> class.
        /// </summary>
        public BatchUsersDTOValidator()
        {
            this.RuleFor(model => model.type)
                .NotEmpty()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Type is empty")
                .Must(x => x.ToLowerInvariant() == "csv" || x.ToLowerInvariant() == "xls" || x.ToLowerInvariant() == "xlsx")
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Type must be xls, csv or xlsx");
            this.RuleFor(x => x.csvOrExcelContent)
                .NotEmpty()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Content is empty");
            this.RuleFor(x => x.companyId)
                .NotEmpty()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Company is empty");
        }
    }
}