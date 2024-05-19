using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace AwtadStudy.FirebaseAdmin
{
	public class FirebaseService
    {
        public FirebaseService(IConfiguration configuration)
		{
            // Configure the FirebaseApp Admin SDK.
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("/Users/kabha/Projects/service-account-file.json"),
                ProjectId = "study-with-awtad",
            });

            //Here we can add more firebase apps, e.g Firebase-Databse configuration...
        }
	}
}

