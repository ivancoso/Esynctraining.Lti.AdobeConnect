using System;

namespace eSyncTraining.Database.Entities
{
    public class UserLoginHistoryEntity
    {
        public virtual int Id { get; set; }

        public virtual string Login { get; set; }

        public virtual string Name { get; set; }

        public virtual DateTime Timestamp { get; set; }
    }
}
