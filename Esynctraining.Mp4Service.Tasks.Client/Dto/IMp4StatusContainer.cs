namespace Esynctraining.Mp4Service.Tasks.Client.Dto
{
    public interface IMp4StatusContainer
    {
        string Id { get; set; }

        Mp4TaskStatusDto Mp4 { get; set; }

    }

}
