![Firebase Logo](https://gregnz.com/images/firebase_logo.png)

# Firebase Token Authorization for C#

Caches certificate based on max-age in the Cache-Control header of the response to reduce latency and unneccessary web requests.

More info on verifying Firebase tokens with 3rd party libraries can be found here:
https://firebase.google.com/docs/auth/admin/verify-id-tokens

## Requirements

.NET Core >= 2.0


## Installation Instructions

1. Get the Nuget package from here: https://www.nuget.org/packages/FirebaseAuth/

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

3. On the client side you will need to add an "Authorize" header to your requests. Prepend the token with "Bearer ".

```javascript
this.unsubscribe = firebase.auth().onAuthStateChanged(function (user) {
    if (user) {
        user.getIdToken(true).then(function (token) {
        const headers = { Authorization: `Bearer ${token}` };
          
        // add the above header to your request
      }
    });
```

4. For all actions/controllers, you can now start guarding with the **[Authorize]** filter. 

5. That's it. If you have any questions, please ask.


## Tips

You can access claims from User.Claims object.

![Firebase Auth Claims Example](https://gregnz.com/images/firebase_auth_claims_v2.png)

There is an extension method of ClaimsPrincipal for getting the Firebase User Id claim. 

```csharp
using FirebaseAuth;

var userId = User.GetFirebaseUserId();
```
