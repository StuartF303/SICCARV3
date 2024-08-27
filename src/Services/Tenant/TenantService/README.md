# Tenant Service
 Requires a Certificate for the Identity server, copy the '***components/selfsigned.pfx_***' into the TenantService directory as '***default.pfx***' 
 
 In your ***appsettings.Development.json*** use 
 
```
     "IdentityServer": {
        "IssuerName": "https://localhost:8443"
      },
     "Kestrel": {
        "Certificates": {
          "Default": {
            "Path": "default.pfx",
            "Password": "5iccar"
          }
        }
      }
```

You must also remember the "DefaultTenant": "etc in appsettings"

Added copy seeds file in dockerfile