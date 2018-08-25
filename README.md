![Firebase Logo](https://gregnz.com/images/firebase_logo.png)

# Firebase Token Generation and Authorization for C#

More info on verifying Firebase tokens with 3rd party libraries can be found here:
https://firebase.google.com/docs/auth/admin/verify-id-tokens

## Requirements

.NET Core >= 2.0
Firebase V3.X

## Installation Instructions

### For Authorizing any type of Firebase token:

1. Get the Nuget package from here: https://www.nuget.org/packages/FirebaseAuth/

2. If you're not implementing your own authorization store, you will need your Firebase Project Id (this can be found in Firebase Console). Ask Google if you have any trouble finding it. In Startup.cs add the following configuration.  

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // required for Firebase Auth
    void options(FirebaseAuthenticationOptions o)
    {
        o.FirebaseProjectId = "your project id (can be found in firebase console)";
        o.ExceptionLogger = (Exception ex) =>
        {
            // Set up exception logging here. 
        };
    }

    // Required for Firebase Auth. Place above AddMvc()
    services.AddAuthentication(o =>
    {
        o.DefaultScheme = "Default";
    }).AddScheme<FirebaseAuthenticationOptions, FirebaseAuthenticationHandler>("Default", options);

    // Add memory cache
    services.AddMemoryCache();
    services.AddMvc();
}
        
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    // required for Firebase Auth
    app.UseAuthentication();
    
    app.UseMvc();
}
```
### Generating a token for firebase method signInWithCustomToken (optional)

If your primary authentication store is not Firebase and you want to use some features of Firebase that needs authentication, you can generate Firebase compatible tokens and use the signInWithCustomToken.

1. In Firebase console click the cog in top left corner and then go to "Users and permissions".

2. Click on the "Service Accounts" tab at the top. 

3. Click on "Manage all service accounts".

4. Click "Create service account".

5. Enter a Service account name.

6. For Project role, select "Project -> Editor".

7. Tick the check box "Furnish a new private key".

8. Select the key type "JSON" (it should already be selected).

9. Click save and open the downloaded JSON file. 

10. Take note of the "private_key" and "client_email" values. You might want to put these into appsettings.json. 

11. Generating a token. To generate a token, you will need your private_key and client_email. Also note that the maximum expiry for a custom Firebase token is 60 minutes. You might want to think about a refresh token strategy. 

12. Instantiate a new instance of FirebaseAuthenticationHelper.

Here is example usage...

```csharp
string privateKey = @"-----BEGIN PRIVATE KEY--.... Your private key\n";
string clientEmail = @"your client email";
var firebaseHelper = new FirebaseAuthenticationHelper(privateKey, clientEmail);
var token = firebaseHelper.GenerateToken("testuser");
```

## Authorizing a token server side
On the client side you will need to add an "Authorization" header to your requests. Prepend the token with "Bearer ".

```javascript
firebase.auth().onAuthStateChanged(function (user) {
    if (user) {
        user.getIdToken(true).then(function (token) {
        const headers = { Authorization: `Bearer ${token}` };
          
        // add the above header to your request you want to authenticate
      }
    });
```

For all actions/controllers, you can now start guarding with the **[Authorize]** filter. You can also specify Roles.  

That's it. If you have any questions, please ask.
