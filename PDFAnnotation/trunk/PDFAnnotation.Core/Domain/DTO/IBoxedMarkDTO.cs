namespace PDFAnnotation.Core.Domain.DTO
{
    /// <summary>
    /// Represents items drawn in rectangle boxes
    /// </summary>
    public interface IBoxedMarkDTO : IBaseMarkDTO
    {
        float positionX { get; set; }

        float positionY { get; set; }

        float width { get; set; }

        float height { get; set; }

        string color { get; set; }

    }

}
