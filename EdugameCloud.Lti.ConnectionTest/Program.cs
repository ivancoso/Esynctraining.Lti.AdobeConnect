using System;
using Castle.Core.Logging;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Core.DTO;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.ConnectionTest
{
    class Program
    {
        private const string OkMessage = "Connected successfully";

        private static TestConnectionService _testConnectionService;


        private static TestConnectionService TestConnectionService
        {
            get { return _testConnectionService; }
        }


        static void Main(string[] args)
        {
            IoCStart.Init();

            _testConnectionService = IoC.Resolve<TestConnectionService>();
            ILogger logger = IoC.Resolve<ILogger>();
            try
            {
                logger.InfoFormat("===== ConnectionTest Starts. DateTime:{0} =====", DateTime.Now);
                
                var lmsCompanyModel = IoC.Resolve<LmsCompanyModel>();
                var licenses = lmsCompanyModel.GetAll();
                foreach (var lmsCompany in licenses)
                {
                    try
                    {
                        var dto = new CompanyLmsDTO(lmsCompany);
                        Test(lmsCompany, dto);
                        lmsCompanyModel.RegisterSave(lmsCompany);
                    }
                    catch (Exception ex)
                    {
                        logger.ErrorFormat(ex, "Unexpected error during execution for LmsCompanyId: {0}.", lmsCompany.Id);
                    }
                }

                lmsCompanyModel.Flush();
            }
            catch (Exception ex)
            {
                string msg = "Unexpected error during execution ConnectionTest with message: " + ex.Message;
                logger.Error(msg, ex);
            }
            finally
            {
                logger.InfoFormat("===== ConnectionTest stops. DateTime:{0} =====", DateTime.Now);
            }
        }

        private static void Test(LmsCompany entity, CompanyLmsDTO resultDto)
        {
            bool isTransient = false;
            string lmsPassword = resultDto.lmsAdminPassword;
            if (!isTransient && string.IsNullOrWhiteSpace(resultDto.lmsAdminPassword))
            {
                if ((entity.LmsProvider.Id == (int)LmsProviderEnum.Moodle)
                || ((entity.LmsProvider.Id == (int)LmsProviderEnum.Blackboard) && !resultDto.enableProxyToolMode))
                {
                    lmsPassword = entity.AdminUser.Password;
                }
                else if ((entity.LmsProvider.Id == (int)LmsProviderEnum.Blackboard) && resultDto.enableProxyToolMode)
                {
                    lmsPassword = resultDto.proxyToolPassword;
                }
            }

            ConnectionInfoDTO lmsConnectionTest = TestConnectionService.TestConnection(new ConnectionTestDTO
            {
                domain = resultDto.lmsDomain,
                enableProxyToolMode = resultDto.enableProxyToolMode,
                login = resultDto.lmsAdmin,
                password = lmsPassword,
                type = resultDto.lmsProvider,
            });

            string acPassword = (isTransient || !string.IsNullOrWhiteSpace(resultDto.acPassword))
                ? resultDto.acPassword
                : entity.AcPassword;

            string acConnectionInfo;
            bool loginSameAsEmail;
            bool acConnectionTest = TestConnectionService.TestACConnection(new ConnectionTestDTO
            {
                domain = resultDto.acServer,
                enableProxyToolMode = resultDto.enableProxyToolMode,
                login = resultDto.acUsername,
                password = acPassword,
                type = "ac",
            }, out acConnectionInfo, out loginSameAsEmail);

            // NOTE: always use setting from AC not UI
            entity.ACUsesEmailAsLogin = loginSameAsEmail;
            entity.IsActive = lmsConnectionTest.status == OkMessage && acConnectionTest;
        }

    }

}
