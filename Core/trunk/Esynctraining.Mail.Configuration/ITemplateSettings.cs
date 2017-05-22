namespace Esynctraining.Mail.Configuration
{
    public interface ITemplateSettings
    {
        // TODO: DO WE USE?????
        string AttachmentsFolderPath { get; }

        //[Required]
        string TemplatesFolderPath { get; }

        string ImagesFolderPath { get; }

    }

}
