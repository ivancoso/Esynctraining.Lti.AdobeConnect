using System;
using System.Linq;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public sealed class MeetingFolderBuilder : IFolderBuilder
    {
        private readonly LmsCompany _credentials;
        private readonly IAdobeConnectProxy _provider;
        private readonly bool _useLmsUserEmailForSearch;


        private LmsProviderModel LmsProviderModel
        {
            get { return IoC.Resolve<LmsProviderModel>(); }
        }

        private LmsCompanyModel LmsСompanyModel
        {
            get { return IoC.Resolve<LmsCompanyModel>(); }
        }

        
        public MeetingFolderBuilder(LmsCompany credentials, IAdobeConnectProxy provider, bool useLmsUserEmailForSearch)
        {
            if (credentials == null)
                throw new ArgumentNullException(nameof(credentials));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            _credentials = credentials;
            _provider = provider;
            _useLmsUserEmailForSearch = useLmsUserEmailForSearch;
        }


        public string GetMeetingFolder(Principal user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return GetMeetingFolder(_credentials, _provider, user, _useLmsUserEmailForSearch);
        }


        private string GetMeetingFolder(LmsCompany credentials, IAdobeConnectProxy provider, Principal user, bool useLmsUserEmailForSearch)
        {
            string adobeConnectScoId = null;

            if (credentials.UseUserFolder.GetValueOrDefault() && user != null)
            {
                ////TODO Think about user folders + renaming directory
                adobeConnectScoId = SetupUserMeetingsFolder(credentials, provider, user, useLmsUserEmailForSearch);
            }

            if (adobeConnectScoId == null)
            {
                LmsProvider lmsProvider = LmsProviderModel.GetById(credentials.LmsProviderId);
                SetupSharedMeetingsFolder(credentials, lmsProvider, provider);
                this.LmsСompanyModel.RegisterSave(credentials);
                this.LmsСompanyModel.Flush();
                adobeConnectScoId = credentials.ACScoId;
            }

            return adobeConnectScoId;
        }

        private static string SetupUserMeetingsFolder(LmsCompany credentials, IAdobeConnectProxy provider,
            Principal user, bool useLmsUserEmailForSearch)
        {
            var shortcut = provider.GetShortcutByType("user-meetings");

            var userFolderName = credentials.ACUsesEmailAsLogin.GetValueOrDefault() ? user.Email : user.Login;
            
            //var userFolderName = useLmsUserEmailForSearch ? user.Email : user.Login;
            var meetingsFolderName = string.IsNullOrEmpty(credentials.UserFolderName)
                ? userFolderName
                : credentials.UserFolderName;
            string meetingFolderScoId;

            CreateUserFoldersStructure(shortcut.ScoId, provider, userFolderName,
                meetingsFolderName, out meetingFolderScoId);
            return meetingFolderScoId;
        }
        
        private static void SetupSharedMeetingsFolder(LmsCompany credentials, LmsProvider lmsProvider, IAdobeConnectProxy provider)
        {
            string ltiFolderSco = null;
            string name = credentials.UserFolderName ?? lmsProvider.LmsProviderName;
            name = name.TruncateIfMoreThen(60);
            if (!string.IsNullOrWhiteSpace(credentials.ACScoId))
            {
                ScoInfoResult canvasFolder = provider.GetScoInfo(credentials.ACScoId);
                if (canvasFolder.Success && canvasFolder.ScoInfo != null)
                {
                    if (canvasFolder.ScoInfo.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        ltiFolderSco = canvasFolder.ScoInfo.ScoId;
                    }
                    else
                    {
                        ScoInfoResult updatedSco =
                            provider.UpdateSco(
                                new FolderUpdateItem
                                {
                                    ScoId = canvasFolder.ScoInfo.ScoId,
                                    Name = name,
                                    FolderId = canvasFolder.ScoInfo.FolderId,
                                    Type = ScoType.folder
                                });
                        if (updatedSco.Success && updatedSco.ScoInfo != null)
                        {
                            ltiFolderSco = updatedSco.ScoInfo.ScoId;
                        }
                    }
                }
            }

            if (ltiFolderSco == null)
            {
                ScoContentCollectionResult sharedMeetings = provider.GetContentsByType("meetings");
                if (sharedMeetings.ScoId != null && sharedMeetings.Values != null)
                {
                    ScoContent existingFolder = sharedMeetings.Values.FirstOrDefault(v => v.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && v.IsFolder);
                    if (existingFolder != null)
                    {
                        credentials.ACScoId = existingFolder.ScoId;
                    }
                    else
                    {
                        ScoInfoResult newFolder = provider.CreateSco(new FolderUpdateItem { Name = name, FolderId = sharedMeetings.ScoId, Type = ScoType.folder });
                        if (newFolder.Success && newFolder.ScoInfo != null)
                        {
                            provider.UpdatePublicAccessPermissions(newFolder.ScoInfo.ScoId, SpecialPermissionId.denied);
                            credentials.ACScoId = newFolder.ScoInfo.ScoId;
                        }
                    }
                }
            }
        }

        private static void CreateUserFoldersStructure(string folderScoId, IAdobeConnectProxy provider,
            string userFolderName,
            string userMeetingsFolderName,
            out string innerFolderScoId)
        {
            var folderContent = provider.GetContentsByScoId(folderScoId);
            var userFolder = folderContent.Values.FirstOrDefault(x => x.Name == userFolderName);
            if (userFolder == null)
            {
                var userFolderScoId = CreateFolder(folderScoId, userFolderName, provider);
                var userMeetingsFolderScoId = CreateFolder(userFolderScoId, userMeetingsFolderName, provider);
                innerFolderScoId = userMeetingsFolderScoId;
                return;
            }

            var userFolderContent = provider.GetContentsByScoId(userFolder.ScoId);
            var userMeetingsFolder = userFolderContent.Values.FirstOrDefault(x => x.Name == userMeetingsFolderName);
            if (userMeetingsFolder == null)
            {
                innerFolderScoId = CreateFolder(userFolder.ScoId, userMeetingsFolderName, provider);
                return;
            }

            innerFolderScoId = userMeetingsFolder.ScoId;
        }

        private static string CreateFolder(string folderScoId, string folderName, IAdobeConnectProxy provider)
        {
            var newFolder = provider.CreateSco(new FolderUpdateItem
            {
                Name = folderName.TruncateIfMoreThen(60),
                FolderId = folderScoId,
                Type = ScoType.folder
            });

            if (!newFolder.Success)
            {
                var msg = string.Format("[AdobeConnectProxy Error] CreateSco " + "Parameters: FolderId:{0}, Name:{1}", folderScoId, folderName);
                throw new InvalidOperationException(msg);
            }
            return newFolder.ScoInfo.ScoId;

        }

    }

}
