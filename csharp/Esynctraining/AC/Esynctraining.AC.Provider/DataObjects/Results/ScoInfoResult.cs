namespace Esynctraining.AC.Provider.DataObjects.Results
{
    using Esynctraining.AC.Provider.Entities;

    public class ScoInfoResult : ResultBase
    {
        public ScoInfo ScoInfo { get; set; }

        public override bool Success
        {
            get { return base.Success && ScoInfo != null; }
        }


        public ScoInfoResult(StatusInfo status) : base(status) { }

        public ScoInfoResult(StatusInfo status, ScoInfo scoInfo)
            : base(status)
        {
            ScoInfo = scoInfo;
        }

    }
    
    public class ScoInfoByUrlResult : ResultBase
    {
        public ScoInfoByUrl ScoInfo { get; set; }

        public override bool Success
        {
            get { return base.Success && ScoInfo != null; }
        }


        public ScoInfoByUrlResult(StatusInfo status) : base(status) { }

        public ScoInfoByUrlResult(StatusInfo status, ScoInfoByUrl scoInfo)
            : base(status)
        {
            ScoInfo = scoInfo;
        }

    }

}
