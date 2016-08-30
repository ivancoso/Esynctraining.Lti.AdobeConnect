namespace Esynctraining.AC.Provider.Entities
{
    public class UserSeminarLicenseSco
    {
        public string AccountId { get; set; }
        public string FolderId { get; set; }
        public string Icon { get; set; }
        public int LicenseQuota { get; set; }
        public string Name { get; set; }
        public string PrincipalId { get; set; }
        public int? Quota { get; set; }
        public string QuotaId { get; set; }
        public string ScoId { get; set; }
        public ScoType Type { get; set; }
        public string UrlPath { get; set; }
    }
}