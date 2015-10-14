using System;

namespace EdugameCloud.Lti.Domain.Entities
{
    public interface ILmsLicense
    {
        int Id { get; }
        
        string AcPassword { get; }

        string AcServer { get; }

        string AcUsername { get; }

        string LmsDomain { get; }

        bool IsActive { get; }

        int CompanyId { get; }

        bool HasLmsDomain(string domainToCheck);

    }

}
