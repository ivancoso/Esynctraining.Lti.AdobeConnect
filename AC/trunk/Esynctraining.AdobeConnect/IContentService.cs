using System.Collections.Generic;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect
{
    public interface IContentService
    {
        IEnumerable<ScoContent> GetUserContent();

        IEnumerable<ScoContent> GetSharedContent();

        IEnumerable<ScoContent> GetFolderContent(string folderScoId);

    }

}
