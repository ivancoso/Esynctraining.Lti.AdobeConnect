using Esynctraining.AC.Provider.Entities;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public sealed class SeminarFolderBuilder : IFolderBuilder
    {
        private readonly string _seminarLicenseScoId;


        public SeminarFolderBuilder(string seminarLicenseScoId)
        {
            _seminarLicenseScoId = seminarLicenseScoId;
        }


        public string GetMeetingFolder(Principal user)
        {
            return _seminarLicenseScoId;
        }

    }
    
}
