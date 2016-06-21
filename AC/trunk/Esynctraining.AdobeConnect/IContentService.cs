using System.Collections.Generic;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect
{
    public interface IContentService
    {
        IEnumerable<ScoContent> GetMyContent();

        IEnumerable<ScoContent> GetUserContent(string userLogin);

        IEnumerable<ScoContent> GetSharedContent();

        IEnumerable<ScoContent> GetFolderContent(string folderScoId);

    }

}
