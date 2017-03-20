using System;
using System.Linq;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.AC.Provider.DataObjects.Results;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.Utils;
using Esynctraining.AdobeConnect;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public sealed class MeetingFolderBuilder : IFolderBuilder
    {
        private readonly LmsCompany _lmsCompany;
        private readonly IAdobeConnectProxy _provider;
        private readonly bool _useLmsUserEmailForSearch;
        private readonly LmsMeetingType _lmsMeetingType;

        private LmsProviderModel LmsProviderModel
        {
            get { return IoC.Resolve<LmsProviderModel>(); }
        }

        private LmsCompanyModel LmsСompanyModel
        {
            get { return IoC.Resolve<LmsCompanyModel>(); }
        }

        
        public MeetingFolderBuilder(LmsCompany lmsCompany, IAdobeConnectProxy provider, bool useLmsUserEmailForSearch, LmsMeetingType lmsMeetingType = LmsMeetingType.Meeting)
        {
            if (lmsCompany == null)
                throw new ArgumentNullException(nameof(lmsCompany));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            _lmsCompany = lmsCompany;
            _provider = provider;
            _useLmsUserEmailForSearch = useLmsUserEmailForSearch;
            _lmsMeetingType = lmsMeetingType;
        }


        public string GetMeetingFolder(Principal user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return GetMeetingFolder(_lmsCompany, _provider, user, _useLmsUserEmailForSearch);
        }


        private string GetMeetingFolder(LmsCompany lmsCompany, IAdobeConnectProxy provider, Principal user, bool useLmsUserEmailForSearch)
        {
            string adobeConnectScoId = null;

            if (lmsCompany.UseUserFolder.GetValueOrDefault() && user != null)
            {
                ////TODO Think about user folders + renaming directory
                adobeConnectScoId = SetupUserMeetingsFolder(lmsCompany, provider, user, useLmsUserEmailForSearch);
            }

            if (adobeConnectScoId == null)
            {
                LmsProvider lmsProvider = LmsProviderModel.GetById(lmsCompany.LmsProviderId);
                adobeConnectScoId = SetupSharedMeetingsFolder(lmsCompany, lmsProvider, provider);
                this.LmsСompanyModel.RegisterSave(lmsCompany);
                this.LmsСompanyModel.Flush();
            }

            return adobeConnectScoId;
        }

        private string SetupUserMeetingsFolder(LmsCompany lmsCompany, IAdobeConnectProxy provider,
            Principal user, bool useLmsUserEmailForSearch)
        {
            var shortcutName = MeetingTypeFactory.GetMeetingFolderShortcut(_lmsMeetingType, true).GetACEnum();
            var shortcut = provider.GetShortcutByType(shortcutName);

            var userFolderName = lmsCompany.ACUsesEmailAsLogin.GetValueOrDefault() ? user.Email : user.Login;
            
            //var userFolderName = useLmsUserEmailForSearch ? user.Email : user.Login;
            var meetingsFolderName = string.IsNullOrEmpty(lmsCompany.UserFolderName)
                ? userFolderName
                : lmsCompany.UserFolderName;
            string meetingFolderScoId;

            CreateUserFoldersStructure(shortcut.ScoId, provider, userFolderName,
                meetingsFolderName, out meetingFolderScoId);
            return meetingFolderScoId;
        }
        
        private string SetupSharedMeetingsFolder(LmsCompany lmsCompany, LmsProvider lmsProvider, IAdobeConnectProxy provider)
        {
            string ltiFolderSco = null;
            string name = lmsCompany.UserFolderName ?? lmsProvider.LmsProviderName;
            name = name.TruncateIfMoreThen(60);
            bool useDbFolderId = MeetingTypeFactory.UseDbMeetingFolderId(_lmsMeetingType);
            if (useDbFolderId && !string.IsNullOrWhiteSpace(lmsCompany.ACScoId))
            {
                ScoInfoResult meetingsFolder = provider.GetScoInfo(lmsCompany.ACScoId);
                if (meetingsFolder.Success && meetingsFolder.ScoInfo != null)
                {
                    if (meetingsFolder.ScoInfo.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        ltiFolderSco = meetingsFolder.ScoInfo.ScoId;
                    }
                    else
                    {
                        ScoInfoResult updatedSco =
                            provider.UpdateSco(
                                new FolderUpdateItem
                                {
                                    ScoId = meetingsFolder.ScoInfo.ScoId,
                                    Name = name,
                                    FolderId = meetingsFolder.ScoInfo.FolderId,
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
                var shortcutName = MeetingTypeFactory.GetMeetingFolderShortcut(_lmsMeetingType, false).GetACEnum();
                ScoContentCollectionResult sharedMeetings = provider.GetContentsByType(shortcutName);
                if (sharedMeetings.ScoId != null && sharedMeetings.Values != null)
                {
                    ScoContent existingFolder = sharedMeetings.Values.FirstOrDefault(v => v.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && v.IsFolder);
                    if (existingFolder != null && useDbFolderId)
                    {
                        ltiFolderSco = existingFolder.ScoId;
                    }
                    else
                    {
                        ScoInfoResult newFolder = provider.CreateSco(new FolderUpdateItem { Name = name, FolderId = sharedMeetings.ScoId, Type = ScoType.folder });
                        if (newFolder.Success && newFolder.ScoInfo != null)
                        {
                            provider.UpdatePublicAccessPermissions(newFolder.ScoInfo.ScoId, SpecialPermissionId.denied);
                            ltiFolderSco = newFolder.ScoInfo.ScoId;
                        }
                    }
                }
                if (ltiFolderSco != null && useDbFolderId)
                {
                    lmsCompany.ACScoId = ltiFolderSco;
                }
            }

            return ltiFolderSco;
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
