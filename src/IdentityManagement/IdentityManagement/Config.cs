using Duende.IdentityServer.Models;

using IdentityModel;

namespace YourBrand.IdentityManagement;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
              [
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource("tenant", ["tenant_id", "role"])

              // let's include the role claim in the profile
              //new ProfileWithRoleIdentityResource(),
              ];

    public static IEnumerable<ApiResource> ApiResources =>
        [
                // the api requires the role claim
                new ApiResource("myapi", "The Web Api", [JwtClaimTypes.Name, JwtClaimTypes.PreferredUserName, JwtClaimTypes.Email, JwtClaimTypes.Role, "tenant_id"])
                {
                    Scopes = ["myapi"]
                },
                // the api requires the role claim
                new ApiResource("worker", "The Web Api", [JwtClaimTypes.Name, JwtClaimTypes.PreferredUserName, JwtClaimTypes.Email, JwtClaimTypes.Role])
                {
                    Scopes = ["myapi"]
                },
                // the api requires the role claim
                new ApiResource("timereport", "The Web Api", [JwtClaimTypes.Name, JwtClaimTypes.PreferredUserName, JwtClaimTypes.Email, JwtClaimTypes.Role])
                {
                    Scopes = ["myapi"]
                },
                 new ApiResource("catalogapi", "The Catalog API", [JwtClaimTypes.Name, JwtClaimTypes.PreferredUserName, JwtClaimTypes.Email, JwtClaimTypes.Role])
                {
                    Scopes = ["catalogapi"]
                },
                new ApiResource("cartsapi", "The Carts API", [JwtClaimTypes.Name, JwtClaimTypes.PreferredUserName, JwtClaimTypes.Email, JwtClaimTypes.Role])
                {
                    Scopes = ["cartsapi"]
                },
                new ApiResource("salesapi", "The Carts API", [JwtClaimTypes.Name, JwtClaimTypes.PreferredUserName, JwtClaimTypes.Email, JwtClaimTypes.Role, "tenant_id"])
                {
                    Scopes = ["salesapi"]
                }
        ];

    public static IEnumerable<ApiScope> ApiScopes =>
        [
            // the api requires the claims
            new ApiScope("myapi", "Access the api", [ "role", "tenant_id" ]),
            new ApiScope("catalogapi", "Access the Catalog API"),
            new ApiScope("cartsapi", "Access the Carts API"),
            new ApiScope("salesapi", "Access the Carts API")
        ];

    public static IEnumerable<Duende.IdentityServer.Models.Client> Clients =>
            [
            new Duende.IdentityServer.Models.Client
            {
                ClientId = "portal",
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = false,
                AllowedCorsOrigins = [ "https://localhost:5174", "https://yourbrand.local:5174", "https://acme.yourbrand.local:5174" ],
                AllowedScopes = [ "openid", "profile", "email", "myapi", "tenant" ],
                RedirectUris = [ "https://acme.yourbrand.local:5174/authentication/login-callback" ],
                PostLogoutRedirectUris = [ "https://acme.yourbrand.local:5174" ],
                Enabled = true
            },
            new Duende.IdentityServer.Models.Client
            {
                ClientId = "storefront",

                // no interactive user, use the clientid/secret for authentication
                AllowedGrantTypes = GrantTypes.ClientCredentials,

                // secret for authentication
                ClientSecrets =
                [
                    new Secret("foobar123!".Sha256())
                ],

                // scopes that client has access to
                AllowedScopes = [ "profile", "email", "myapi", "catalogapi", "cartsapi", "salesapi" ],

                Claims = [ new ClientClaim("tenant_id", TenantConstants.TenantId) ]
            },
            new Duende.IdentityServer.Models.Client
            {
                ClientId = "appservice",

                // use token exchange
                AllowedGrantTypes = [ OidcConstants.GrantTypes.TokenExchange ],

                // secret for authentication
                ClientSecrets =
                [
                    new Secret("foobar123!".Sha256())
                ],

                // scopes that client has access to
                AllowedScopes = [ "profile", "email", "myapi" ],
            },
            new Duende.IdentityServer.Models.Client
            {
                ClientId = "accountant",

                // no interactive user, use the clientid/secret for authentication
                AllowedGrantTypes = GrantTypes.ClientCredentials,

                // secret for authentication
                ClientSecrets =
                [
                    new Secret("foobar123!".Sha256())
                ],

                // scopes that client has access to
                AllowedScopes = [ "profile", "email", "myapi" ],
            }
        ];
}