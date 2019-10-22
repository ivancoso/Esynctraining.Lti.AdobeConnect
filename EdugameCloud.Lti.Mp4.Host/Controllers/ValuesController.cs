using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using EdugameCloud.Lti.Api.Controllers;
using EdugameCloud.Lti.API.AdobeConnect;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Caching;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Providers;
using Microsoft.AspNetCore.Mvc;

namespace EdugameCloud.Lti.Mp4.Host.Controllers
{
    /// <summary>
    /// Used to test REST API is alive.
    /// </summary>
    [Route("api/[controller]")]
    public class ValuesController : //ControllerBase //
        BaseApiController
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
           API.AdobeConnect.IAdobeConnectAccountService acAccountService,
           ApplicationSettingsProvider settings,
           ILogger logger,
           ICache cache
        )
            : base(acAccountService, settings, logger, cache)
        {
        }

        [HttpGet("uploadcontent-test")]
        public IEnumerable<TestDto> Get2()
        {

            var apiUrl = new Uri("https://connectstage.esynctraining.com");

            var acService = new Esynctraining.AdobeConnect.AdobeConnectAccountService(Logger);

            var proxy = acService.GetProvider2(new AdobeConnectAccess2(apiUrl, "breezbreezhqpt892zszvoioyi"));


            //var content = File.ReadAllBytes(@"C:\Users\kniaz\Downloads\po82jtnycylf.html");
            var someString = @"WEBVTT

0
00:00:16.006 --> 00:00:18.001
will be at noon when you can please 

1
00:00:19.028 --> 00:00:20.049
great or so 

2
00:00:25.018 --> 00:00:28.025
Uh yes way let me know just 

3
00:00:28.067 --> 00:00:30.052
typical week to completely up 

4
00:00:40.067 --> 00:00:41.037
because one TEST UPLOAD

";
            var content = Encoding.ASCII.GetBytes(someString);

            var uploadScoInfo = new UploadScoInfo
            {
                scoId = "5174664",
                fileContentType = "text/html",
                fileName = "p8e0bmu3hkc.html",
                fileBytes = content,
                title = "p8e0bmu3hkc.html",
            };

            var d = proxy.UploadContent(uploadScoInfo);

            return new TestDto[]
            {
                new TestDto { Id = 1, Description = "Uploaded with code=" + d.Code, Date = DateTime.Now },
                new TestDto { Id = 2, Date = DateTime.Today }
            };
        }


        [HttpGet]
        public IEnumerable<TestDto> Get()
        {
            return new TestDto[]
            {
                new TestDto { Id = 1, Description = "test", Date = DateTime.Now },
                new TestDto { Id = 2, Date = DateTime.Today }
            };
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
