using System;
using FirebaseAdmin.Auth;

namespace AwtadStudy.FirebaseAdmin

{
	internal class FirebaseAuthService
	{
        // Retrieve user data by their uid
        internal static async ValueTask<UserRecord> GetUserAsync(string uid)
        {
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            return userRecord;
        }

        // Create and authenticate new user, can not update user with this method
        internal static async ValueTask<UserRecord> CreateUserAsync(UserRecordArgs userArgs)
        {
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userArgs);
            return userRecord;
        }

        // Update authenticated user
        internal static async ValueTask<UserRecord> UpdateUserAsync(UserRecordArgs newUserArgs)
        {
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.UpdateUserAsync(newUserArgs);
            return userRecord;
        }

        // Retrieve users data by their uids/emails
        // 100 is the maximum of identifiers to be supplied.
        internal static async ValueTask<IEnumerable<UserRecord>> GetUsersAsync()
        {
            List<UserIdentifier> users = new List<UserIdentifier>{
                //new UidIdentifier("uid1"),
                //new UidIdentifier("uid2"),
                new EmailIdentifier("mukabha99@gmail.com"),
                new EmailIdentifier("mkabha54@gmail.com"),
            };

            GetUsersResult result = await FirebaseAuth.DefaultInstance.GetUsersAsync(users);

            return result.Users;
        }

        // Delete user by uid, if deleted successfuly return the uid
        internal static async ValueTask<string> DeleteUserAsync(string uid)
        {
            await FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);
            return uid;
        }

    }
}

