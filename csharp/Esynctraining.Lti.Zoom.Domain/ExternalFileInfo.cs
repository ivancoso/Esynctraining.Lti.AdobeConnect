using System.ComponentModel.DataAnnotations.Schema;

namespace Esynctraining.Lti.Zoom.Domain
{
    [Table("ExternalFileInfo")]
    public class ExternalFileInfo : BaseEntity
    {
        public virtual LmsCourseMeeting Meeting { get; set; }
        public ExternalStorageProvider ProviderId { get; set; }
        public string ProviderFileRecordId { get; set; }
    }
}