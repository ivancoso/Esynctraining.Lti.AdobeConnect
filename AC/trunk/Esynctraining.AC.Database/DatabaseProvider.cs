using eSyncTraining.Database.DataObjects;
using eSyncTraining.Database.DataObjects.Extensions;
using eSyncTraining.Database.Repositories;

namespace eSyncTraining.Database
{
    public class DatabaseProvider
    {
        public void UserLoggedIn(UserDataObject user)
        {
            var repository = new UserLoginHistoryRepository();

            repository.Create(user.ToEntity());
        }
    }
}
