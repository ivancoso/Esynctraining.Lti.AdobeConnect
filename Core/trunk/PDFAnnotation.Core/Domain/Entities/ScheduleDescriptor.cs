namespace PDFAnnotation.Core.Domain.Entities
{
    /// <summary>
    /// The schedule descriptor.
    /// </summary>
    public enum ScheduleDescriptor : byte
    {
        /// <summary>
        /// The paticipants report update.
        /// </summary>
        UpdateParticipants = 0,

        /// <summary>
        /// The recording import routing update.
        /// </summary>
        UpdateEventsWithRecordingAndParticipantsAndShutThemDown = 1,

    }
}