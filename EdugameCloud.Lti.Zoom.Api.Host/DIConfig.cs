using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Castle.Core.Resource;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Edugamecloud.Lti.Zoom.Services;
using EdugameCloud.Lti.API.BlackBoard;
using EdugameCloud.Lti.API.Moodle;
using EdugameCloud.Lti.API.Sakai;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Persistence;
using Esynctraining.CastleLog4Net;
using Esynctraining.Core.Providers;
using Esynctraining.Windsor;
using Esynctraining.Zoom.ApiWrapper;
using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace EdugameCloud.Lti.Zoom.Api.Host
{
    public class FakeMoodleApi: IEGCEnabledMoodleApi {
        public Task<(IEnumerable<LmsQuizInfoDTO> Data, string Error)> GetItemsInfoForUserAsync(LmsUserParameters lmsUserParameters, bool isSurvey)
        {
            throw new System.NotImplementedException();
        }

        public Task<(IEnumerable<LmsQuizDTO> Data, string Error)> GetItemsForUserAsync(LmsUserParameters lmsUserParameters, bool isSurvey, IEnumerable<int> quizIds)
        {
            throw new System.NotImplementedException();
        }

        public Task SendAnswersAsync(LmsUserParameters lmsUserParameters, string json, bool isSurvey, string[] answers = null)
        {
            throw new System.NotImplementedException();
        }

        public void PublishQuiz(LmsUserParameters lmsUserParameters, int courseId, int quizId)
        {
            throw new System.NotImplementedException();
        }
    }
    public class FakeBBApi: IEGCEnabledBlackBoardApi {
        public Task<(IEnumerable<LmsQuizInfoDTO> Data, string Error)> GetItemsInfoForUserAsync(LmsUserParameters lmsUserParameters, bool isSurvey)
        {
            throw new System.NotImplementedException();
        }

        public Task<(IEnumerable<LmsQuizDTO> Data, string Error)> GetItemsForUserAsync(LmsUserParameters lmsUserParameters, bool isSurvey, IEnumerable<int> quizIds)
        {
            throw new System.NotImplementedException();
        }

        public Task SendAnswersAsync(LmsUserParameters lmsUserParameters, string json, bool isSurvey, string[] answers = null)
        {
            throw new System.NotImplementedException();
        }

        public void PublishQuiz(LmsUserParameters lmsUserParameters, int courseId, int quizId)
        {
            throw new System.NotImplementedException();
        }
    }
    public class FakeSakaiApi: IEGCEnabledSakaiApi {
        public Task<(IEnumerable<LmsQuizInfoDTO> Data, string Error)> GetItemsInfoForUserAsync(LmsUserParameters lmsUserParameters, bool isSurvey)
        {
            throw new System.NotImplementedException();
        }

        public Task<(IEnumerable<LmsQuizDTO> Data, string Error)> GetItemsForUserAsync(LmsUserParameters lmsUserParameters, bool isSurvey, IEnumerable<int> quizIds)
        {
            throw new System.NotImplementedException();
        }

        public Task SendAnswersAsync(LmsUserParameters lmsUserParameters, string json, bool isSurvey, string[] answers = null)
        {
            throw new System.NotImplementedException();
        }

        public void PublishQuiz(LmsUserParameters lmsUserParameters, int courseId, int quizId)
        {
            throw new System.NotImplementedException();
        }
    }
    internal static class DIConfig
    {
        public static WindsorContainer ConfigureWindsor(IConfiguration Configuration)
        {
            var container = new WindsorContainer();

            container.Register(Component.For<ISessionSource>().ImplementedBy<AspNetCoreNHibernateSessionSource>().LifeStyle.Scoped());

            WindsorIoC.Initialize(container);
            container.RegisterComponents();

            var configurationSection = Configuration.GetSection("AppSettings");
            var collection = new NameValueCollection();
            foreach (var appSetting in configurationSection.GetChildren())
            {
                collection.Add(appSetting.Key, appSetting.Value);
            }

            container.Register(Component.For<ApplicationSettingsProvider>().ImplementedBy<ApplicationSettingsProvider>()
                .DynamicParameters((k, d) => d.Add("collection", collection))
                .LifeStyle.Singleton);

            container.Install(new LoggerWindsorInstaller());
            container.Install(new EdugameCloud.Core.Logging.LoggerWindsorInstaller());

            RegisterLtiComponents(container);
            //todo: per client
            container.Register(Component.For<ZoomApiWrapper>().ImplementedBy<ZoomApiWrapper>().DynamicParameters(
                    (k, d) => d.Add("options", new ZoomApiOptions
                    {
                        ZoomApiKey = "zc69A0RpTBKIpQbxknTs1Q",
                        ZoomApiSecret = "kTWs9t1G0pPcS2CSZ31SysWu2VxFgzs2uATL"
                    }))
                .LifeStyle.Singleton);
            /*
             IEGCEnabledMoodleApi moodleApi, IEGCEnabledBlackBoardApi blackboardApi, IEGCEnabledSakaiApi sakaiApi
             */
            container.Register(Component.For<IEGCEnabledMoodleApi>().ImplementedBy<FakeMoodleApi>().LifestyleSingleton());
            container.Register(Component.For<IEGCEnabledBlackBoardApi>().ImplementedBy<FakeBBApi>().LifestyleSingleton());
            container.Register(Component.For<IEGCEnabledSakaiApi>().ImplementedBy<FakeSakaiApi>().LifestyleSingleton());
            container.Register(Component.For<ZoomUserService>().ImplementedBy<ZoomUserService>().LifestyleSingleton());
            container.Register(Component.For<ZoomRecordingService>().ImplementedBy<ZoomRecordingService>().LifestyleSingleton());
            container.Register(Component.For<ZoomMeetingService>().ImplementedBy<ZoomMeetingService>().LifestyleSingleton());
            container.Register(Component.For<ZoomReportService>().ImplementedBy<ZoomReportService>().LifestyleSingleton());

            return container;
        }

        private static void RegisterLtiComponents(WindsorContainer container)
        {
            container.Install(
                Castle.Windsor.Installer.Configuration.FromXml(new AssemblyResource("assembly://EdugameCloud.Lti.Canvas/EdugameCloud.Lti.Canvas.Windsor.xml"))
            );

            container.Install(new LtiWindsorInstaller());

            container.Register(Classes.FromAssemblyNamed("EdugameCloud.Lti").BasedOn(typeof(IValidator<>)).WithService.Base().LifestyleTransient());
        }

    }

}
