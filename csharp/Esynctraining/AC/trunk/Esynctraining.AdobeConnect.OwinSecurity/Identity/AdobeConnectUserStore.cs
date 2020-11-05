using System;
using System.Threading.Tasks;
using Esynctraining.AdobeConnect.Security.Abstractions.Identity;
using Microsoft.AspNet.Identity;

namespace Esynctraining.AdobeConnect.OwinSecurity.Identity
{
    public sealed class AdobeConnectUserStore<T> : IUserStore<T>, IUserPasswordStore<T> where T : AdobeConnectUser, new()
    {
        //private UserModel UserModel
        //{
        //    get { return IoC.Resolve<UserModel>(); }
        //}

        //private ILogger Logger
        //{
        //    get { return IoC.Resolve<ILogger>(); }
        //}

        void IDisposable.Dispose()
        {
            // throw new NotImplementedException(); 

        }

        public Task CreateAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task<T> FindByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<T> FindByNameAsync(string userName)
        {
            //User user = UserModel.GetOneByEmail(userName).Value;
            
            //if (user == null)
            //{
            //    // NOTE: do nothing, just to skip user=null situation
            //}
            //else if (user.Status != UserStatus.Active)
            //{
            //    user = null;
            //    Logger.ErrorFormat("API: user {0} has {1} status.", userName, user.Status);
                
            //    //throw new FaultException<Error>(error, error.errorMessage);
            //}
            //else if (!(user.IsAdministrator() || user.IsSuperAdmin()))
            //{
            //    Logger.ErrorFormat("API: user {0} is not Administrator.", userName);
            //    user = null;
            //}
            //else if (!user.Company.IsActive())
            //{
            //    Logger.ErrorFormat("API: User {0} has its company not active.", userName);
            //    user = null;
            //}
            //else if (!user.Company.CurrentLicense.HasApi)
            //{
            //    Logger.ErrorFormat("API: Company.CurrentLicense.HasApi==false. User {0}. CurrentLicense ID: {1}.", userName, user.Company.CurrentLicense.Id);
            //    user = null;
            //}

            T result = null;
            //if (user != null)
            //{
            //    result = new T
            //    {
            //        Id = user.Id,
            //        UserName = user.Email,
            //    };
            //}
            return Task.FromResult(result);
        }


        public Task SetPasswordHashAsync(T user, string passwordHash)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(T user)
        {
            //return Task.FromResult(user.Password);
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(T user)
        {
            throw new NotImplementedException();
        }

    }

}

