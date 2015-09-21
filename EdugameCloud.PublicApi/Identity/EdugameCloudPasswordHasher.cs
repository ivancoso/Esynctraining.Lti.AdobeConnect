using System;
using Esynctraining.Core.Utils;
using Microsoft.AspNet.Identity;

namespace EdugameCloud.PublicApi.Identity
{
    public sealed class EdugameCloudPasswordHasher : PasswordHasher
    {
        public override string HashPassword(string password)
        {
            throw new NotImplementedException();
            //return base.HashPassword(password);
        }

        public override PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            string hash = BitConverter.ToString(Cryptographer.GenerateHash(providedPassword)).Replace("-", string.Empty);

            //Here we will place the code of password hashing that is there in our current solucion.This will take cleartext anad hash 
            //Just for demonstration purpose I always return true.     
            if (hashedPassword.Equals(hash, StringComparison.OrdinalIgnoreCase))
            {
                return PasswordVerificationResult.SuccessRehashNeeded;
            }
            else
            {
                return PasswordVerificationResult.Failed;
            }
        }

    }

}
