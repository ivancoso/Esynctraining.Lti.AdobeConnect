using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esynctraining.AC.Provider.DataObjects;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using NUnit.Framework;

namespace Esynctraining.AC.Provider.Tests
{
    internal class CustomFieldTests
    {
        [Test]
        public void GetCustomFieldByName()
        {
            AdobeConnectProvider provider = GetProvider();

            var result = provider.GetCustomField("esync-mp4-result");
        }

        [Test]
        public void CreateCustomField()
        {
            AdobeConnectProvider provider = GetProvider();

            //https://fiustg.adobeconnect.com/api/xml?action=custom-field-update&account-id=965886535&object-type=object-type-read-only
            //&permission-id=manage&name=esync-mp4-result&comments=eSyncTraining+MP4+Service+Internal+Use&field-type=text&is-required=false&is-primary=false

            var value = new CustomField
            {
                //FieldId
                AccountId = "7",
                ObjectType = ObjectType.object_type_hidden,
                PermissionId = PermissionId.manage,
                Name = "esync-mp4-result",
                Comments = "eSyncTraining MP4 Service Internal Use",
                FieldType = CustomField.CustomFieldType.text,
                IsPrimary = false,
                IsRequired = false,
            };

            var result = provider.CustomFieldUpdate(value);
        }

        [Test]
        public void UpdateCustomField()
        {
            AdobeConnectProvider provider = GetProvider();

            //https://fiustg.adobeconnect.com/api/xml?action=custom-field-update&account-id=965886535&object-type=object-type-read-only
            //&permission-id=manage&name=esync-mp4-result&comments=eSyncTraining+MP4+Service+Internal+Use&field-type=text&is-required=false&is-primary=false

            var value = new CustomField
            {
                FieldId = "385412",
                AccountId = "7",
                ObjectType = ObjectType.object_type_hidden,
                PermissionId = PermissionId.manage,
                Name = "esync-mp4-result",
                Comments = "eSyncTraining MP4 Service Internal Use Updated",
                FieldType = CustomField.CustomFieldType.text,
                IsPrimary = false,
                IsRequired = false,
            };

            var result = provider.CustomFieldUpdate(value);
        }

        [Test]
        public void DeleteCustomField()
        {
            AdobeConnectProvider provider = GetProvider();
            
            var result = provider.CustomFieldDelete("esync-mp4-result");
        }

        [Test]
        public void SetupCustomField()
        {
            AdobeConnectProvider provider = GetProvider();
            var field = provider.GetCustomField("esync-mp4-result").Value;

            string recordingId = "44636"; //  User Meetings >  sergeyi@esynctraining.com >  isa test mp4 >  recUNO
            // User Content > sergeyi@esynctraining.com > MP4 Recordings > isa test mp4 > recUNO
            var info = new
            {
                mp4 = "85704",
                vtt = "85705",
            };
            string fieldValue = Newtonsoft.Json.JsonConvert.SerializeObject(info);
            StatusInfo setupResult = provider.UpdateAclField(new List<AclFieldUpdateTrio>
                {
                    new AclFieldUpdateTrio
                    {
                        AclId = recordingId,
                        FieldId = field.FieldId,
                        Value = fieldValue,
                    },
                });

            FieldResult originalName = provider.GetAclField(recordingId, field.FieldId);
        }


        private AdobeConnectProvider GetProvider()
        {
            string apiUrl = "http://connectdev.esynctraining.com/api/xml";
            string login = "sergeyi@esynctraining.com";
            string pwd = "e$ync123";
            string accountId = null;

            var connectionDetails = new ConnectionDetails(apiUrl);
            var provider = new AdobeConnectProvider(connectionDetails);
            var loginResult = provider.Login(new UserCredentials(login, pwd, accountId));

            return provider;
        }

    }

}
