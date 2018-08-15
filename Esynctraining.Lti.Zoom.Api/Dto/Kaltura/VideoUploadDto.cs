using System.ComponentModel.DataAnnotations;

namespace Esynctraining.Lti.Zoom.Api.Dto.Kaltura
{
    public class VideoUploadDto
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string VideoTypeTag { get; set; }

        [Required]
        public string UploadedFileTokenId { get; set; }

    }

    public class MediaEntryDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DataUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public int Views { get; set; }
        public int CreatedAt { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; }
    }
}