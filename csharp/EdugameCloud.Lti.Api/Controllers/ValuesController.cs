using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EdugameCloud.Lti.API.AdobeConnect;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Microsoft.AspNetCore.Mvc;

namespace EdugameCloud.Lti.Api.Controllers
{
    /// <summary>
    /// Used to test REST API is alive.
    /// </summary>
    [Route("api/[controller]")]
    [EdugameCloud.Lti.Api.Filters.LmsAuthorizeBase]
    public class ValuesController : BaseApiController
    {
        /// <summary>
        /// Test input DTO.
        /// </summary>
        public class InputDto
        {
            /// <summary>
            /// String value with (50-100) length validation.
            /// </summary>
            [StringLength(10, MinimumLength = 5)]
            [Required]
            public string MeetingId { get; set; }
        }

        /// <summary>
        /// out DTO
        /// </summary>
        public class TestDto
        {
            /// <summary>
            /// ID
            /// </summary>
            [Required]
            public int Id { get; set; }

            /// <summary>
            /// Description can be empty.
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// This is DateTime property. Should be returned as Unix timestamp.
            /// </summary>
            public DateTime Date { get; set; }
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

        
        /// <summary>
        /// Test method to check API works and alive. Returns hard-coded values.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [EdugameCloud.Lti.Api.Filters.LmsAuthorizeBase(ApiCallEnabled = true)]
        public IEnumerable<TestDto> Get2([FromBody]InputDto request)
        {
            return new TestDto[] { new TestDto { Id = 1, Description = "test", Date = DateTime.Now }, new TestDto { Id = 2, Date = DateTime.Today } };
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
