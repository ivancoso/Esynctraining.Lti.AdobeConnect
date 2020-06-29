using System;

namespace Esynctraining.Lti.Zoom.Common.Dto.Reports
{
    public class ZoomSessionParticipantDetailsDto
    {
        public string Id { get; set; }   
        public string Name { get; set; }   
        public string Device { get; set; }   
        public string IpAddress { get; set; }   
        public string Location { get; set; }   
        public string NetworkType { get; set; }   
        public DateTime EnteredAt { get; set; }   
        public DateTime LeftAt { get; set; }   
        public bool ShareApplication { get; set; }   
        public bool ShareDesktop { get; set; }   
        public bool ShareWhiteboard { get; set; }   
        public bool Recording { get; set; }   
        public string PcName { get; set; }
        public string Domain { get; set; }
        public string MacAddr { get; set; }
        public string HarddiskId { get; set; }
        public string Version { get; set; }
    }
}