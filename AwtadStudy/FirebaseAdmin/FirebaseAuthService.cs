using System;
using AwtadStudy.Interfaces;
using FirebaseAdmin.Auth;


namespace AwtadStudy.FirebaseAdmin

{
	internal sealed class FirebaseAuthService : IFirebaseAuth
	{
        // Retrieve user data by their uid
        public async Task<UserRecord> GetUserAsync(string uid)
        {
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            return userRecord;
        }

        // Create and authenticate new user, can not update user with this method
        public async Task<UserRecord> CreateUserAsync(UserRecordArgs userArgs)
        {
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userArgs);
            return userRecord;
        }

        // Update authenticated user
        public async Task<UserRecord> UpdateUserAsync(UserRecordArgs newUserArgs)
        {
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.UpdateUserAsync(newUserArgs);
            return userRecord;
        }

        // Retrieve users data by their uids/emails
        // 100 is the maximum of identifiers to be supplied.
        public async Task<IEnumerable<UserRecord>> GetUsersAsync()
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
        public async Task<string> DeleteUserAsync(string uid)
        {
            await FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);
            return uid;
        }

        //These tokens expire after one hour, and return JWT token
        public async Task<string> CreateCustomToken(string uid)
        {
            string customToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(uid);
            // Send token back to client
            return customToken;
        }

    }
}

