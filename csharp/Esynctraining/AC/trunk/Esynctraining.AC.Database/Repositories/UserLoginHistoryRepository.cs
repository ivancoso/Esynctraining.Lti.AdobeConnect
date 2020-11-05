using eSyncTraining.Database.Entities;

namespace eSyncTraining.Database.Repositories
{
    internal class UserLoginHistoryRepository
    {
        public int Create(UserLoginHistoryEntity entity)
        {
            using (var session = NHibernateHelper.OpenSession())
            using (var tran = session.BeginTransaction())
            {
                var id = (int) session.Save(entity);

                tran.Commit();

                return id;
            }
        }
    }
}
