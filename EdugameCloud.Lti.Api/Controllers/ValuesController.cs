using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.API.AdobeConnect;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Logging;

namespace EdugameCloud.Lti.Api.Controllers
{
    /// <summary>
    /// Used to test REST API is alive.
    /// </summary>
    [Route("api/[controller]")]
    [EdugameCloud.Lti.Api.Filters.LmsAuthorizeBase]
    public class ValuesController : BaseApiController
    {
        public class TestDto
        {
            [Required]
            public int Id { get; set; }

            public string Description { get; set; }
        }
        
        public ValuesController(
           IAdobeConnectAccountService acAccountService,
           ApplicationSettingsProvider settings,
           ILogger logger,
           ICache cache
        )
            : base(acAccountService, settings, logger, cache)
        {
        }

        // GET api/values
        [HttpGet]
        [EdugameCloud.Lti.Api.Filters.LmsAuthorizeBase(ApiCallEnabled = true)]
        public IEnumerable<TestDto> Get()
        {
            return new TestDto[] { new TestDto { Id = 1, Description = "test" }, new TestDto { Id = 2 } };
        }

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
