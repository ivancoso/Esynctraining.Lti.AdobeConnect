using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esynctraining.Core.Enums;
using FluentValidation;
using Esynctraining.FluentValidation;

namespace EdugameCloud.Lti.DTO.Recordings
{
    public sealed class EditDto
    {
//        [Required]
        public string lmsProviderName { get; set; }

        //[Required]
        public int meetingId { get; set; }

        //[Required]
        public string id { get; set; }

        //[Required]
        //[StringLength(60, ErrorMessageResourceType = typeof(Resources.Messages), ErrorMessageResourceName = "ValidateTitleLength")]
        public string name { get; set; }

        //[StringLength(4000, ErrorMessageResourceType = typeof(Resources.Messages), ErrorMessageResourceName = "ValidateSummaryLength")]
        public string summary { get; set; }

    }

    public sealed class EditDtoValidator : AbstractValidator<EditDto>
    {
        public EditDtoValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(model => model.lmsProviderName).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Session is empty");
            this.RuleFor(model => model.meetingId).GreaterThan(0);
            this.RuleFor(model => model.id).NotEmpty();

            this.RuleFor(model => model.name).NotEmpty().WithLocalizedMessage(() => Resources.Messages.ValidateTitleLength);
            this.RuleFor(model => model.name).Length(1, 60).WithLocalizedMessage(() => Resources.Messages.ValidateTitleLength);
            this.RuleFor(model => model.summary).Length(0, 4000).WithLocalizedMessage(() => Resources.Messages.ValidateSummaryLength);
        }

    }

}
