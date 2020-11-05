using System;
using eSyncTraining.Database.Entities;

namespace eSyncTraining.Database.DataObjects.Extensions
{
    internal static class UserExtension
    {
        public static UserLoginHistoryEntity ToEntity(this UserDataObject dataObject)
        {
            if (dataObject == null)
            {
                return null;
            }

            return new UserLoginHistoryEntity
                {
                    Login = dataObject.Login,
                    Name = dataObject.Name,
                    Timestamp = DateTime.Now
                };
        }

        public static UserDataObject ToDataObject(this UserLoginHistoryEntity entity)
        {
            if (entity == null)
            {
                return null;
            }

            return new UserDataObject
                {
                    Login = entity.Login,
                    Name = entity.Name
                };
        }
    }
}
