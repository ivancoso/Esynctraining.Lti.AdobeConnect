using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using EdugameCloud.Lti.API.AdobeConnect;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.AdobeConnect.Caching
{
    public sealed class PrincipalCacher
    {
        private readonly ILog _log;

        public PrincipalCacher(ILog log)
        {
            _log = log;
        }

        public void Process(LmsCompany license)
        {

            try
            {
                _log.WriteLine("START " + license.AcServer);
                ProcessAc(license);
            }
            catch (Exception ex)
            {
                _log.WriteLine("    Error. AC: " + license.AcServer);
                _log.WriteLine(ex, "    ");
            }
            _log.WriteLine("STOP " + license.AcServer);
        }


        private void ProcessAc(LmsCompany lmsCompany)
        {
            IAdobeConnectProxy provider = IoC.Resolve<IMeetingSetup>().GetProvider(lmsCompany);

            var stopwatch = Stopwatch.StartNew();

            var result = provider.GetAllPrincipals();

            stopwatch.Stop();
            var acTime = stopwatch.Elapsed;

            if (!result.Success)
            {
                _log.WriteLine("    GetAllPrincipals Issue:" + result.Status.Code);
                return;
            }

            stopwatch = Stopwatch.StartNew();

            var table = new AcCacheDataSet.AcCachePrincipalDataTable();
            foreach (Principal p in result.Values)
            {
                AddToTable(table, p, lmsCompany.Id);
            }
            BatchBulkCopy(table, "AcCachePrincipal", 500);

            stopwatch.Stop();
            var dbTime = stopwatch.Elapsed;

            _log.WriteLine("    PrincipalCount: " + result.Values.Count() + ". AC fetch time: " + acTime.ToString() + ". DB save time: " + dbTime.ToString());
        }

        private static void BatchBulkCopy(DataTable dataTable, string destinationTableName, int batchSize)
        {
            DataTable dtInsertRows = dataTable;

            string connectionString = ConfigurationManager.ConnectionStrings["Database"].ConnectionString;

            using (var sbc = new SqlBulkCopy(connectionString, SqlBulkCopyOptions.Default))
            {
                sbc.DestinationTableName = destinationTableName;

                // NOTE: Number of records to be processed in one go
                sbc.BatchSize = batchSize;

                // Finally write to server
                sbc.WriteToServer(dtInsertRows);
            }
        }

        private static void AddToTable(AcCacheDataSet.AcCachePrincipalDataTable table, Principal source, int lmsCompanyId)
        {
            var dest = table.NewAcCachePrincipalRow();

            dest.lmsCompanyId = lmsCompanyId;

            dest.accountId = source.AccountId;
            dest.displayId = source.DisplayId;
            dest.email = source.Email;
            dest.firstName = source.FirstName;
            dest.hasChildren = source.HasChildren;
            dest.isHidden = source.IsHidden;
            dest.isPrimary = source.IsPrimary;
            dest.lastName = source.LastName;
            dest.login = source.Login;
            dest.name = source.Name;
            dest.principalId = source.PrincipalId;
            dest.type = source.Type;

            table.Rows.Add(dest);
        }

    }
}
