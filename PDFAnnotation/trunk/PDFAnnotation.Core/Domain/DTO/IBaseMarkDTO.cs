namespace PDFAnnotation.Core.Domain.DTO
{
    public interface IBaseMarkDTO
    {
        string id { get; set; }

        int pageIndex { get; set; }

        float? rotation { get; set; }

        string displayFormat { get; set; }

        string type { get; set; }

        string fileId { get; set; }

        double datechanged { get; set; }

        double datecreated { get; set; }

        bool @readonly { get; set; }

    }

}
