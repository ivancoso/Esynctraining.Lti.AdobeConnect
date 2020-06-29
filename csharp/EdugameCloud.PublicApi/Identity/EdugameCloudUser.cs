using System;
using Microsoft.AspNet.Identity;

namespace EdugameCloud.PublicApi.Identity
{
    public class EdugameCloudUser : IUser
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        string IUser<string>.Id
        {
            get
            {
                return Id.ToString();
            }
        }

    }

}
