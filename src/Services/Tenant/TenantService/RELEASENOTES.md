
# Tenant Service - Release Notes

Update: databox 1.1 release 11/05/23 
v0.103 - updates and signalr admin
v0.101 admin auth
v0.100 - with admin
v0.99 - missed notes
v0.98
 authZ and sustainability
v0.93
 updated memory
 removed deprecated KeyVault 
v0.92
 spin cycle 
v0.91
 cloud signal r & invocation

v0.90
 working validation cycle

## May 22 - Tenant Boot Process

We need to have a Tenant and Client defined to allow the TenantService to provision Service Tickets

On Startup the service the TenantRepository** is loaded from the seed file in **TenantService/deploy** !! Moved since last release

 - seed.json - this configures the Resources, Scopes and Identity
   related system defaults
   
 - seed.development.json - only loads in a development build to preload 
   out development tenant and default clients

 for a release build the initial tenant can be provisioned by posting to /api/init a json document which is basically the same as the seed.development.json entries.

 To get a new Tenant ID, the file does not specify the ID and as such one will be created and used for the clients as well as being returned on creation.


 You should define a default tenant id via the appsettings

       "DefaultTenant": "<new tenant Id>",

 Running local put it in appsettings.development.json, otherwise the main appsettings and Restart the service
 
 If you test with Postman, clear your tokens and cookies before trying to get your first Bearer

 Sample Seed file:  

     {
      "tenants": [
        {
          "Name": "Siccar.Net",
          "Authority": "https://login.microsoftonline.com/a2b9ca5b-54e5-437e-866e-bd48bfa6159a/",
          "ClientId": "53cb40d8-edef-47b9-8836-f2162fcf3e62",
          "CallbackPath": "/signin-ad",
          "SignedOutCallbackPath": "/signout-callback-ad",
          "RemoteSignOutPath": "/signout-ad",
          "ResponseType": "id_token",
          "TokenValidationParameters": {
            "AuthenticationType":  "oidc",
            "NameClaimType": "name",
            "RoleClaimType": "role"
          },
          "Registers": [],
          "AdminEmail": "dev@siccar.net"
        }
      ],
      "clients": [
        {
          "ClientId": "siccar-client",
          "ClientSecrets": [
            {
              "Description": null,
              "Type": "SharedSecret",
              "Value": "K7gNU3sdo+OL0wNhqoVWhr3g6s1xYv72ol/pe/Unols=",
              "Expiration": null
            }
          ],
          "AllowedGrantTypes": [
            "authorization_code",
            "client_credentials"
          ],
          "RedirectUris": [ "https://localhost:6004/authentication/login-callback" ],
          "PostLogoutRedirectUris": [ "https://localhost:6004/signout-callback-oidc" ],
          "AllowOfflineAccess": true,
          "AllowedCorsOrigins": [ "https://localhost:6004" ],
          "AllowedScopes": [
            "openId",
            "profile",
            "api",
            "wallet.read",
            "wallet.update",
            "wallet.create",
            "wallet.admin"
          ]
        },
        {
          "ClientId": "siccar-admin-ui-client",
          "RequireClientSecret": false,
          "AllowedGrantTypes": [
            "authorization_code",
            "client_credentials"
          ],
          "RedirectUris": [
            "https://localhost:6004/authentication/login-callback",
            "https://localhost:5101/authentication/login-callback",
            "http://localhost:6004"
          ],
          "PostLogoutRedirectUris": [
            "https://localhost:6004/signout-callback-oidc",
            "https://localhost:5101/authentication/logout-callback"
          ],
          "AllowOfflineAccess": true,
          "AllowedCorsOrigins": [
            "https://localhost:6004",
            "https://localhost:5101",
            "http://localhost:6004"
          ],
          "AllowedScopes": [
            "openid",
            "profile",
            "api",
            "wallet.read",
            "wallet.update",
            "wallet.create",
            "wallet.admin",
            "installation.admin"
          ]
        }
      ]
    }
