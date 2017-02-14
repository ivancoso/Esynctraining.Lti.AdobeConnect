using System;
using System.Collections.Generic;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect
{
    public interface IContentService
    {
        IEnumerable<ScoShortcut> GetShortcuts(IEnumerable<ScoShortcutType> typesToReturn);

        [Obsolete("Use GetShortcuts + GetFolderContent")]
        IEnumerable<ScoContent> GetMyContent();

        IEnumerable<ScoContent> GetUserContent(string userLogin);

        [Obsolete("Use GetSharedContent + GetFolderContent")]
        IEnumerable<ScoContent> GetSharedContent();

        IEnumerable<ScoContent> GetFolderContent(string folderScoId);
        IEnumerable<ScoContent> GetFolderContent(string folderScoId, PageOptions pageOptions, SortOptions sortOptions, string filter);

        IEnumerable<ScoContent> GetFolderContent(string folderScoId, string sourceScoId);

        string GetDownloadAsZipLink(string scoId, string breezeToken);

    }

}
