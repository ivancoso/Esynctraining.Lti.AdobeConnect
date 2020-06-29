namespace Esynctraining.Lti.Zoom.Api.Dto
{
    public sealed class ExternalMediaDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int CreatedAt { get; set; }
        public int MsDuration { get; set; }
        public string DataUrl { get; set; }
        public string DownloadUrl { get; set; }
        public string Description { get; set; }
        public string ThumbnailUrl { get; set; }
        public int Views { get; set; }
        public string Status { get; set; }
    }
}