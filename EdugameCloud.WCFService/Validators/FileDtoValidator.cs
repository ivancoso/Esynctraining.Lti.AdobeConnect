namespace EdugameCloud.WCFService.Validators
{
    using System;
    using System.IO;
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    using FluentValidation;

    /// <summary>
    /// The file validator.
    /// </summary>
    public class FileDtoValidator : AbstractValidator<FileDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileDtoValidator"/> class.
        /// </summary>
        public FileDtoValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(x => x.dateCreated).NotEqual(default(DateTime)).WithError(Errors.CODE_ERRORTYPE_INVALID_PARAMETER, "Date is invalid");
            this.RuleFor(x => x.fileName)
                .Must((model, x) =>
                    (model.fileId != default(Guid) && (string.IsNullOrWhiteSpace(x) || Path.GetExtension(x) != null))
                    || (!string.IsNullOrWhiteSpace(x) && Path.GetExtension(x) != null))
                .WithError(Errors.CODE_ERRORTYPE_INVALID_PARAMETER, "File name is empty or not contains extension");
            this.RuleFor(x => x.fileId)
                .Must((model, x) => (x == default(Guid) && model.createdBy.HasValue)
                    || (model.height.HasValue || model.width.HasValue || model.x.HasValue || model.y.HasValue || !string.IsNullOrWhiteSpace(model.fileName)))
                .WithError(Errors.CODE_ERRORTYPE_INVALID_PARAMETER, "Required param is missing: createdBy for insert; or height, width, x, y or fileName must be set for update");
        }
    }
}