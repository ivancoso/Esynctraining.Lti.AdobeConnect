namespace Esynctraining.AC.Provider.Entities
{
    public interface IPermissionUpdateTrio
    {
        string ScoId { get; }

        string PrincipalId { get; }

        PermissionId Permission { get; }

    }

}
