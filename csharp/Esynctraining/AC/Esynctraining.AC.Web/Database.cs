// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Database.cs" company="eSyncTraining">
//   eSyncTraining
// </copyright>
// <summary>
//   Defines the Database type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace eSyncTraining.Web
{
    using Esynctraining.AC.Provider.Entities;
    using eSyncTraining.Database;
    using eSyncTraining.Database.DataObjects;

    /// <summary>
    /// The database.
    /// </summary>
    public static class Database
    {
        /// <summary>
        /// The write to database.
        /// </summary>
        private const bool WriteToDatabase = false;

        /// <summary>
        /// The _database provider.
        /// </summary>
        private static DatabaseProvider databaseProvider;

        /// <summary>
        /// Gets the provider.
        /// </summary>
        private static DatabaseProvider Provider
        {
            get
            {
                return databaseProvider ?? (databaseProvider = new DatabaseProvider());
            }
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <typeparam name="T">
        /// Data object.
        /// </typeparam>
        public static void Write<T>(T data)
        {
            if (!WriteToDatabase)
            {
                return;
            }

            if (data is UserInfo)
            {
                WriteUserInfo(data as UserInfo);
            }
        }

        /// <summary>
        /// The write user info.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        private static void WriteUserInfo(UserInfo user)
        {
            if (user == null)
            {
                return;
            }

            Provider.UserLoggedIn(new UserDataObject
                {
                    Login = user.Login,
                    Name = user.Name
                });
        }
    }
}