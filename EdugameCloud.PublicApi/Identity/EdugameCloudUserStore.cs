using System;
using System.Threading.Tasks;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Core.Domain.Entities;
using Esynctraining.Core.Utils;
using Microsoft.AspNet.Identity;

namespace EdugameCloud.PublicApi.Identity
{
    public sealed class EdugameCloudUserStore<T> : IUserStore<T>, IUserPasswordStore<T> where T : EdugameCloudUser, new()
    {
        private UserModel UserModel
        {
            get
            {
                return IoC.Resolve<UserModel>();
            }
        }

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
            User user = UserModel.GetOneByEmail(userName).Value;

            if (user.Status != UserStatus.Active)
            {
                user = null;
                //var error =
                //    new Error(
                //        Errors.CODE_ERRORTYPE_USER_INACTIVE,
                //        ErrorsTexts.AccessError_Subject,
                //        ErrorsTexts.AccessError_UserIsInactive);
                //this.LogError("User.Login", error);
                //throw new FaultException<Error>(error, error.errorMessage);
            }
            else if (!user.IsAdministrator())
            {
                user = null;
            }
            else if (user.Company.IsActive())
            {
                user = null;

                //var error =
                //    new Error(
                //        Errors.CODE_ERRORTYPE_USER_INACTIVE,
                //        ErrorsTexts.AccessError_Subject,
                //        ErrorsTexts.AccessError_CompanyIsInactive);
                //this.LogError("User.Login", error);
                //throw new FaultException<Error>(error, error.errorMessage);

                //var error =
                //    new Error(
                //        Errors.CODE_ERRORTYPE_EXPIRED_LICENSE,
                //        ErrorsTexts.AccessError_Subject,
                //        ErrorsTexts.AccessError_CompanyLicenseIsExpired);
                //this.LogError("User.Login", error);
                //throw new FaultException<Error>(error, error.errorMessage);
            }
            else if (!user.Company.HasApi)
            {
                user = null;
            }

            T result = null;
            if (user != null)
            {
                result = new T
                {
                    Id = user.Id,
                    UserName = user.Email,
                    Password = user.Password,
                };
            }
            return Task.FromResult(result);
        }


        public Task SetPasswordHashAsync(T user, string passwordHash)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(T user)
        {
            return Task.FromResult(user.Password);
        }

        public Task<bool> HasPasswordAsync(T user)
        {
            throw new NotImplementedException();
        }

    }

}

