using System.Runtime.Serialization;
using FluentValidation;

namespace EdugameCloud.Lti.DTO.Recordings
{
    [DataContract]
    public sealed class EditDto
    {
        [DataMember]
        //[Required]
        public int meetingId { get; set; }

        [DataMember]
        //[Required]
        public string recordingId { get; set; }

        [DataMember]
        //[Required]
        //[StringLength(60, ErrorMessageResourceType = typeof(Resources.Messages), ErrorMessageResourceName = "ValidateTitleLength")]
        public string name { get; set; }

        [DataMember]
        //[StringLength(4000, ErrorMessageResourceType = typeof(Resources.Messages), ErrorMessageResourceName = "ValidateSummaryLength")]
        public string summary { get; set; }

    }

    public sealed class EditDtoValidator : AbstractValidator<EditDto>
    {
        public EditDtoValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(model => model.meetingId).GreaterThan(0);
            //this.RuleFor(model => model.recordingId).NotEmpty();

            this.RuleFor(model => model.name).NotEmpty().WithLocalizedMessage(typeof(Resources.Messages), "ValidateTitleLength");
            this.RuleFor(model => model.name).Length(1, 60).WithLocalizedMessage(typeof(Resources.Messages), "ValidateTitleLength");
            this.RuleFor(model => model.summary).Length(0, 4000).WithLocalizedMessage(typeof(Resources.Messages), "ValidateSummaryLength");
        }

    }

}
