namespace EdugameCloud.Core.Contracts
{
    using System.ServiceModel;

    /// <summary>
    /// The IMoodle
    /// </summary>
    [ServiceContract]
    public interface IMoodle
    {
        [OperationContract]
        object mod_adobeconnect_get_total_quiz_list();
    }
}
