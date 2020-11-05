namespace Esynctraining.AC.Provider.Entities
{
    public sealed class PermissionUpdateTrio : IPermissionUpdateTrio
    {
        public string ScoId { get; set; }

        public string PrincipalId { get; set; }

        public PermissionId Permission { get; set; }

    }
    
}
