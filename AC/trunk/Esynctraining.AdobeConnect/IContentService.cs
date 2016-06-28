using System.Collections.Generic;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect
{
    public interface IContentService
    {
        IEnumerable<ScoShortcut> GetShortcuts(IEnumerable<ScoShortcutType> typesToReturn);

        IEnumerable<ScoContent> GetMyContent();

        IEnumerable<ScoContent> GetUserContent(string userLogin);

        IEnumerable<ScoContent> GetSharedContent();

        IEnumerable<ScoContent> GetFolderContent(string folderScoId);

        string GetDownloadAsZipLink(string scoId, string breezeToken);

    }

}
