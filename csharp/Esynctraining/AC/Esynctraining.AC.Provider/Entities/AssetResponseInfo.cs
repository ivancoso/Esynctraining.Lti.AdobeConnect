namespace Esynctraining.AC.Provider.Entities
{
    // https://helpx.adobe.com/adobe-connect/webservices/report-asset-response-info.html
    public partial class AssetResponseInfo
    {
        // Source: attribute interaction-id
        public long InteractionId { get; set; }

        // Source: attribute display-seq
        public int DisplaySeq { get; set; }

        // Source: element response
        public string Response { get; set; }

        // Source: element description
        public string Description { get; set; }

    }

}
