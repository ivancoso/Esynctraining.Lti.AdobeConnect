using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;
using Dapper;

namespace EdugameCloud.BuildManager.Controllers.Api
{
    [RoutePrefix("lms")]
    public sealed class LmsController : ApiController
    {
        public class Item
        {
            public int Id { get; set; }

            public string Name { get; set; }

        }

        public sealed class Company: Item
        {
            public string ContactName { get; set; }

            public string AddressLine { get; set; }

        }

        [HttpGet]
        [Route("providers")]
        public IEnumerable<Item> GetProviders()
        {
            IEnumerable<Item> result;
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlSource"].ConnectionString))
            {
                result = connection.Query<Item>("SELECT lmsProviderId AS Id, lmsProvider AS Name FROM LmsProvider", commandType: CommandType.Text);
            }

            var list = result.AsList();
            list.Insert(0, new Item { Id = -1, Name = "-ALL-" });

            return list;
        }

        [HttpGet]
        [Route("companies")]
        public IEnumerable<Company> GetCompanies()
        {
            IEnumerable<Company> result;
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlSource"].ConnectionString))
            {
                result = connection.Query<Company>(
                    @"SELECT 
	                    c.companyId AS Id
                        ,c.companyName AS Name
	                    ,usr.firstName + ' ' + usr.LastName AS ContactName
	                    ,addr.address1 AS AddressLine
                    FROM Company c
	                    INNER JOIN [User] usr ON usr.userId = c.primaryContactId
	                    INNER JOIN [Address] addr ON addr.addressId = c.addressId
                    ORDER BY companyName", commandType: CommandType.Text);
            }

            return result;
        }

    }

}