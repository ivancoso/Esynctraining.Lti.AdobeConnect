namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    [DataContract]
    public class SubscriptionResultDTO
    {
        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        [DataMember]
        public string result { get; set; }
    }
}
