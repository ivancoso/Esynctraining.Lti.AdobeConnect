using System;
using Esynctraining.AC.Provider.Entities;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public interface IFolderBuilder
    {
        string GetMeetingFolder(Principal user);

    }

}
