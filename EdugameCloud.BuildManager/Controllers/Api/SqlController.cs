using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;
using Dapper;

namespace EdugameCloud.BuildManager.Controllers.Api
{
    [RoutePrefix("sql")]
    public sealed class SqlController : ApiController
    {
        [HttpGet]
        [Route("build/{lmsProviderId}")]
        public IEnumerable<string> GetBuildSql(int? lmsProviderId = null)
        {
            if (lmsProviderId.HasValue && lmsProviderId.Value <= 0)
                lmsProviderId = null;

            IEnumerable<string> sql;
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlSource"].ConnectionString))
            {
                sql = connection.Query<string>("GetBuildSql", new { lmsProviderId = lmsProviderId }, commandType: CommandType.StoredProcedure);
            }

            return sql;
        }

        [HttpGet]
        [Route("company/{companyId}")]
        public IEnumerable<string> GetCompanySql(int companyId)
        {
            if (companyId <= 0)
                throw new ArgumentOutOfRangeException("companyId");
            
            IEnumerable<string> sql;
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlSource"].ConnectionString))
            {
                sql = connection.Query<string>("GetCompanySql", new { companyId = companyId }, commandType: CommandType.StoredProcedure);
            }

            return sql;
        }

    }

}