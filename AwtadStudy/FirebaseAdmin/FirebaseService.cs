using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace AwtadStudy.FirebaseAdmin;

public class FirebaseService
{
    public FirebaseService(IConfiguration configuration)
    {
        // Configure the FirebaseApp Admin SDK.
        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromFile(configuration.GetConnectionString("CredentialPathFile")),
            ProjectId = configuration.GetConnectionString("ProjectId"),
        });

        //Here we can add more firebase apps, e.g Firebase-Databse configuration...
    }
}
