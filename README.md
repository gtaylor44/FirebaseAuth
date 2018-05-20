![Firebase Logo](https://gregnz.com/images/firebase_logo.png)

# Firebase Token Authorization for C#

In this implementation, verification of token takes place within the Authorization pipeline. Public keys for signature verification are extracted from https://www.googleapis.com/robot/v1/metadata/x509/securetoken@system.gserviceaccount.com as per documenation. The ID token is signed by the private key corresponding to the token's kid claim. The max-age in the Cache-Control header of the response is used to cache the request in memory using an absolute caching policy to reduce latency and unneccessary web requests.

More info on verifying Firebase tokens with 3rd party libraries can be found here:
https://firebase.google.com/docs/auth/admin/verify-id-tokens

## Installation Instructions

1. Get the Nuget package from here: <URL>

2. You will need your Firebase Project Id (this can be found in Firebase Console). Ask Google if you have any trouble finding it. In Startup.cs add the following configuration.  

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // required for Firebase Auth
    void options(FirebaseAuthenticationOptions o)
    {
        o.FirebaseProjectId = "your project id (can be found in firebase console)";
        o.ExceptionLogger = (Exception ex) =>
        {
            // set up exception logging here.
        };
    }

    // Required for Firebase Auth. Place above AddMvc()
    services.AddAuthentication(o =>
    {
        o.DefaultScheme = "Default";
    }).AddScheme<FirebaseAuthenticationOptions, FirebaseAuthenticationHandler>("Default", options);

    services.AddMvc();
}
        
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    // required for Firebase Auth
    app.UseAuthentication();
    
    app.UseMvc();
}
```

3. For all actions/controllers, you can now start using the [Authorize] filter. 

4. On the client side you will need to add an "Authorize" header to your requests. Prepend the token with "Bearer ".

```javascript
this.unsubscribe = firebase.auth().onAuthStateChanged(function (user) {
    if (user) {
        user.getIdToken(true).then(function (token) {
        const headers = { Authorization: `Bearer ${token}` };
          
        // add the above header to your request
      }
    });
```

5. That's it. If you have any questions, please ask.


## Tips

You can access claims from User.Claims object.

![Firebase Auth Claims Example](https://gregnz.com/images/firebase_auth_claims_v2.png)

There is an extension method of HttpContext for getting the Firebase User Id claim. 

```csharp
using FirebaseAuth;

var userId = HttpContext.GetFirebaseUserId();
```
