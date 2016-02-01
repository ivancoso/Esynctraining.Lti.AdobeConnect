using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AC.Provider.DataObjects.Results
{
    public class CommonInfoResult : ResultBase
    {
        public CommonInfoResult(StatusInfo status):base(status)
        {
            
        }

        public CommonInfoResult(StatusInfo status, CommonInfo commonInfo):base(status)
        {
            this.CommonInfo = commonInfo;
        }

        public CommonInfo CommonInfo { get; private set; }

    }

}
