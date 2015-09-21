using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace EdugameCloud.PublicApi.Identity
{
    public sealed class EdugameCloudUserManager : UserManager<EdugameCloudUser>
    {
        public EdugameCloudUserManager() : base(new EdugameCloudUserStore<EdugameCloudUser>())
        {
            //We can retrieve Old System Hash Password and can encypt or decrypt old password using custom approach. 
            //When we want to reuse old system password as it would be difficult for all users to initiate pwd change as per Idnetity Core hashing. 
            this.PasswordHasher = new EdugameCloudPasswordHasher();
        }

        //public override System.Threading.Tasks.Task<EdugameCloudUser> FindAsync(string userName, string password)
        //{
        //    Task<EdugameCloudUser> taskInvoke = Task<EdugameCloudUser>.Factory.StartNew(() =>
        //    {
        //        //First Verify Password... 
        //        PasswordVerificationResult result = this.PasswordHasher.VerifyHashedPassword(userName, password);
        //        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        //        {
        //            //Return User Profile Object... 
        //            //So this data object will come from Database we can write custom ADO.net to retrieve details/ 
        //            var applicationUser = new EdugameCloudUser
        //            {
        //                UserName = userName,
        //            };
        //            return applicationUser;
        //        }
        //        return null;
        //    });
        //    return taskInvoke;
        //}
    }
}
