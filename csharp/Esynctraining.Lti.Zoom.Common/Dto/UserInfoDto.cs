namespace Esynctraining.Lti.Zoom.Common.Dto
{
    public class UserInfoDto : UserDto
    {
        public bool Verified { get; set; }
        public string Timezone { get; set; }
        public string SubAccountId { get; set; }
        public string Code { get; set; }
        /*"pmi": "string",
    "timezone": "string",
    "dept": "string",
    "created_at": "string [date-time]",
    "last_login_time": "string [date-time]",
    "last_client_version": "string",
    "vanity_url": "string",
    "pic_url": "string"*/
    }
}