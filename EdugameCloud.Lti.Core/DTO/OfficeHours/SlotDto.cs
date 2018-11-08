namespace EdugameCloud.Lti.Core.DTO.OfficeHours
{
    public class SlotDto : CreateSlotDto
    {
        public int Id { get; set; }
        public int Status { get; set; } // 0 - Free, 1 - Booked, 2 - NotAvailable
        public string UserName { get; set; }
        public bool CanEdit { get; set; }
    }
}