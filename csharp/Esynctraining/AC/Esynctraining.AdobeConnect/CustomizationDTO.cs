using System.Runtime.Serialization;

namespace Esynctraining.AdobeConnect
{
    //javascript:pickcolor('account-header-color')
    [DataContract]
    public class CustomizationDTO
    {
        // FFFFFF
        // account-banner-color
        [DataMember(Name = "accountBannerColor")]
        public string AccountBannerColor { get; set; }

        //666666
        // banner-top-link-color
        [DataMember(Name = "bannerTopLinkColor")]
        public string BannerTopLinkColor { get; set; }

        //666666
        // banner-nav-text-color
        [DataMember(Name = "bannerNavTextColor")]
        public string BannerNavTextColor { get; set; }

        //E9E9E9
        // banner-nav-sel-color
        [DataMember(Name = "bannerNavSelColor")]
        public string BannerNavSelColor { get; set; }

        //A7ACB1
        // account-header-color
        [DataMember(Name = "accountHeaderColor")]
        public string AccountHeaderColor { get; set; }

        [DataMember(Name = "bannerLogoUrl")]
        public string BannerLogoUrl { get; set; }

        public CustomizationDTO()
        {
            AccountBannerColor = "FFFFFF";
            BannerTopLinkColor = "666666";
            BannerNavTextColor = "666666";
            BannerNavSelColor = "E9E9E9";
            AccountHeaderColor = "A7ACB1";
        }

    }

}
