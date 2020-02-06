using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth.Admin.Extensions;
using Auth.Admin.Mappers;
using Auth.Admin.Models;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Auth.Admin.Pages.Clients
{
    public class EditModel : PageModel
    {
        private readonly ConfigurationDbContext _dbContext;

        public int? Id { get; set; }
        public ClientModel Client { get; set; }
        public IEnumerable<SelectListItem> AccessTokenTypes { get; set; }
        public IEnumerable<string> GrantTypes { get; set; }
        public IEnumerable<SelectListItem> ProtocolTypes { get; set; }
        public IEnumerable<SelectListItem> RefreshTokenExpirations { get; set; }
        public IEnumerable<SelectListItem> RefreshTokenUsages { get; set; }
        public IEnumerable<string> Scopes { get; set; }

        public EditModel(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Id = id;

            if (!id.HasValue)
            {
                Client = new ClientModel();
            }
            else
            {
                var client = await _dbContext.Clients
                    .Include(x => x.AllowedGrantTypes)
                    .Include(x => x.AllowedScopes)
                    .Include(x => x.IdentityProviderRestrictions)
                    .Include(x => x.PostLogoutRedirectUris)
                    .Include(x => x.RedirectUris)
                    .SingleOrDefaultAsync(x => x.Id == id.Value);

                if (client == null)
                {
                    return NotFound();
                }

                AccessTokenTypes = EnumExtensions.ToSelectList<AccessTokenType>();
                ProtocolTypes = GetProtocolTypes();
                RefreshTokenExpirations = EnumExtensions.ToSelectList<TokenExpiration>();
                RefreshTokenUsages = EnumExtensions.ToSelectList<TokenUsage>();
                Scopes = await GetScopes();
                GrantTypes = GetGrantTypes();

                Client = client.ToModel();
            }

            return Page();
        }

        private static IEnumerable<string> GetGrantTypes()
        {
            return new List<string>
            {
                "implicit",
                "client_credentials",
                "authorization_code",
                "hybrid",
                "password",
                "urn:ietf:params:oauth:grant-type:device_code"
            };
        }

        private static IEnumerable<SelectListItem> GetProtocolTypes()
        {
            return new[] {new SelectListItem("oidc", "OpenID Connect")};
        }

        private async Task<IEnumerable<string>> GetScopes()
        {
            var apiScopes = await _dbContext.ApiResources.SelectMany(x => x.Scopes.Select(s => s.Name)).ToListAsync();
            var identityScopes = await _dbContext.IdentityResources.Select(x => x.Name).ToListAsync();

            return identityScopes.Concat(apiScopes);
        }
    }
}