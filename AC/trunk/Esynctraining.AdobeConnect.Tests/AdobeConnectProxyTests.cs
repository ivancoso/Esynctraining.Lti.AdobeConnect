using System;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.AC.Provider;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using NSubstitute;
using NUnit.Framework;

namespace Esynctraining.AdobeConnect.Tests
{
    public class AdobeConnectProxyTests
    {
        [Test]
        public void ScoMove()
        {
            var acApiUrl = "https://stage1-melaleuca.acms.com/";
            var login = "developer@esynctraining.com";
            var password = "e$ync123MEL";
            var adobeConnectRoot = new Uri(acApiUrl);
            var con = new ConnectionDetails(adobeConnectRoot);
            var acProvider = new AdobeConnectProvider(con);
            var proxy = new AdobeConnectProxy(acProvider, new FakeLogger(), adobeConnectRoot, String.Empty);
            var result = proxy.Login(new UserCredentials(login, password));
            var scheduledWebinarsFolderId = "110965";
            var startDate = new DateTime(2017, 5, 20, 2, 0, 0);
            var nestedFolders = proxy.GetScoExpandedContent(scheduledWebinarsFolderId).Values.Where(x => x.Icon == "folder" && x.ScoId != scheduledWebinarsFolderId);
            var folderName = $"{startDate.Year}-{startDate.ToString("MM")}-{startDate.ToString("dd")}";
            var dateFolder =
                nestedFolders.FirstOrDefault(x => x.Name == folderName);
            string moveToFolderScoId;
            if (dateFolder == null)
            {
                var newFolder = proxy.CreateSco(new FolderUpdateItem()
                {
                    Type = ScoType.folder,
                    Name = folderName,
                    FolderId = scheduledWebinarsFolderId
                });
                moveToFolderScoId = newFolder.ScoInfo.ScoId;
            }
            else
            {
                moveToFolderScoId = dateFolder.ScoId;
            }
            var res = proxy.MoveSco(moveToFolderScoId, "109041");
            
            var t = 1;
            //proxy.MoveSco()

        }

        [Test]
        public void WillUpdatePassword()
        {
            var login = "anton.abyzov@gmail.com";
            var email = login;
            var password = "WH3hfQ5L";
            var acApiUrl = "https://connectstage.esynctraining.com/";
            var adobeConnectRoot = new Uri(acApiUrl);
            var con = new ConnectionDetails(adobeConnectRoot);
            var acProvider = new AdobeConnectProvider(con);
            var proxy = new AdobeConnectProxy(acProvider, new FakeLogger(), adobeConnectRoot, String.Empty);
            proxy.Login(new UserCredentials("nastya@esynctraining.com", "Welcome1"));//admin

            var _adobeConnectAccountService = new AdobeConnectAccountService(new FakeLogger());
            //proxy.PrincipalUpdatePassword()

            var existingPrincipalsResult = proxy.GetAllByEmail(email);
            if (!existingPrincipalsResult.Success)
            {
                return;
            }

            var existingPrincipal = existingPrincipalsResult.Values.FirstOrDefault();

            var principalSetup = new PrincipalSetup
            {
                PrincipalId = existingPrincipal?.PrincipalId,
                Email = email,
                Login = email,
                FirstName = "aaa",
                LastName = "bbb",
                SendEmail = true,
                HasChildren = false,
                Type = PrincipalType.user
            };
            var updatePrincipalResult = proxy.PrincipalUpdate(principalSetup, existingPrincipal != null);

            var updateResult = proxy.PrincipalUpdatePassword(existingPrincipal?.PrincipalId, password);

            var userProxy = _adobeConnectAccountService.GetProvider(new AdobeConnectAccess(new Uri(acApiUrl), email, password), true);
            
        }

        [Test]
        public void WillTestDateAddHours()
        {
            var date = new DateTime(2017, 5, 4, 23, 30, 0);
            Console.WriteLine(date.ToString("HH:mm"));
            var endDate = date.AddHours(1);
            var result = endDate.ToString("HH:mm");
            Console.WriteLine(result);
        }

        [Test]
        public void WillGetEventInfo()
        {
            var acApiUrl = new Uri("http://connect.esynctraining.com");
            var con = new ConnectionDetails(acApiUrl);
            var acProvider = new AdobeConnectProvider(con);
            var proxy = new AdobeConnectProxy(acProvider, new FakeLogger(), acApiUrl, String.Empty);
            //proxy.report
            proxy.Login(new UserCredentials("nastya@esynctraining.com", "tratata"));//admin
            var eventInfo = proxy.GetScoInfo("2957329");
            Assert.Equals(eventInfo.ScoInfo.EventTemplateScoId, 56489);
        }

        [Test]
        public void WillCreateEventViaProxy()
        {
            var acApiUrl = "https://connectstage.esynctraining.com/";
            var login = "nastya@esynctraining.com";
            var password = "Welcome1";
            var adobeConnectRoot = new Uri(acApiUrl);
            var con = new ConnectionDetails(adobeConnectRoot);
            var acProvider = new AdobeConnectProvider(con);
            var proxy = new AdobeConnectProxy(acProvider, new FakeLogger(), adobeConnectRoot, String.Empty);
            var eventName = $"__FromACLibTest{DateTime.Now:yyyy-M-d hh-mm-ss}";
            var eventStartDate = new DateTime(2017, 5, 4, 0, 30, 0);
            var eventEndDate = eventStartDate.AddDays(1);
            var saveEventFields = new SaveEventFields(new UserCredentials(login, password), eventName, eventStartDate, eventEndDate);
            saveEventFields.TimeZoneId = 85; //Greenwich
            var result = proxy.CreateEvent(saveEventFields);
        }


        [Test]
        public void WillReportUserTrainingsTaken()
        {
            var principalId = "1004370065";
            var acApiUrl = new Uri("http://rhi.adobeconnect.com");
            var con = new ConnectionDetails(acApiUrl);
            var acProvider = new AdobeConnectProvider(con);
            var proxy = new AdobeConnectProxy(acProvider, new FakeLogger(), acApiUrl, String.Empty);
            //proxy.report
            proxy.Login(new UserCredentials("mike@esynctraining.com", "e$ync123RHI"));//admin
            var result = proxy.ReportUserTrainingsTaken(principalId);
        }

        [Test]
        public void WillGetAllByPrincipalIds()
        {
            //var proxy = Substitute.For<IAdobeConnectProxy>();
            var ids = new List<string>();
            var apiUrl = new Uri("https://fiustg.adobeconnect.com");
            var connectionDetails = new ConnectionDetails(apiUrl);
            var login = "ssotest";
            var password = "Ch@ngeTEST1";
            var userCredentials = new UserCredentials(login, password, "965886535");
            var provider = new AdobeConnectProvider(connectionDetails);
            LoginResult loginResult = provider.Login(userCredentials);
            if (!loginResult.Success)
                throw new InvalidOperationException("Invalid login");
            var principalId = loginResult.User.UserId;

            var proxy = new AdobeConnectProxy(provider, new FakeLogger(), apiUrl, principalId);
            //var acProvider = Substitute.ForPartsOf<AdobeConnectProvider>();
            //acProvider.When(x => x.GetAllByPrincipalIds())
            //for (var i = 0; i < 375; i++)
            //{
            //    ids.Add(i);
            //}
            ids = new List<string>() 
            {
                "969980305",
"969994159",
"970000385",
"970031418",
"988325146",
"988325884",
"988333621",
"988336193",
"988345341",
"988345796",
"988392645",
"988392238",
"988392115",
"988390536",
"988386250",
"988385271",
"988384635",
"988383270",
"988383256",
"988382042",
"988381526",
"988374150",
"988372782",
"988372008",
"988371915",
"988371234",
"988364932",
"988363108",
"988362537",
"988361823",
"988466848",
"988464487",
"988463493",
"988463137",
"988462642",
"988462637",
"988458941",
"988458741",
"988458278",
"988458012",
"988456474",
"988449598",
"988441839",
"988441817",
"988439110",
"988438412",
"988437927",
"988437623",
"988433234",
"988432937",
"988429393",
"988423628",
"988421518",
"988419726",
"988419367",
"988417948",
"988416743",
"988415048"

            };
            var result = proxy.GetAllByPrincipalIds(ids.Select(x => x.ToString()).ToArray());

            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Values.Any());
        }


        [Test]
        public void WillChunkRecordingsForTransactions()
        {
            var scoId = "1064081639";
            var acApiUrl = new Uri("https://ncvps.adobeconnect.com");
            var con = new ConnectionDetails(acApiUrl);
            var acProvider = new AdobeConnectProvider(con);
            var proxy = new AdobeConnectProxy(acProvider, new FakeLogger(), acApiUrl, String.Empty);
            //proxy.report
            proxy.Login(new UserCredentials("ncvpsadobe@ncpublicschools.gov", "***get-from-ltidb***"));//admin
            var recordingsSco = proxy.GetRecordingsList(scoId).Values.Select(x => x.ScoId);
            Assert.IsTrue(recordingsSco.Any());

            var transactions = proxy.ReportRecordingTransactions(recordingsSco).Values.ToList();

            Assert.IsTrue(transactions.Any());
        }

        [Test]
        public void WillConvertGuestToUser()
        {
            var email = "***guest-email***";
            var acApiUrl = new Uri("http://connectdev.esynctraining.com");
            var con = new ConnectionDetails(acApiUrl);
            var acProvider = new AdobeConnectProvider(con);
            var proxy = new AdobeConnectProxy(acProvider, new FakeLogger(), acApiUrl, String.Empty);
            
            proxy.Login(new UserCredentials("", ""));//admin
            var guestResult = proxy.ReportGuestsByLogin(email);
            if (guestResult.Values.Any())
            {
                var guest = guestResult.Values.First();
                var updateResult = proxy.PrincipalUpdateType(guest.PrincipalId, PrincipalType.user);
                Assert.IsTrue(updateResult.Success);
                var principalInfo = proxy.GetPrincipalInfo(guest.PrincipalId);
                Assert.AreEqual(principalInfo.PrincipalInfo.Principal.Type, PrincipalType.user.ToString());
            }
        }
    }
}