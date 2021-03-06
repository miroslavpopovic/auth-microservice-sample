﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth.Admin.Extensions;
using Auth.Admin.Mappers;
using Auth.Admin.Models;
using Auth.Admin.Services;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace Auth.Admin.Pages.Clients
{
    public class EditModel : PageModel
    {
        private readonly ConfigurationDbContext _dbContext;

        public EditModel(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int? Id { get; set; }

        [BindProperty]
        public ClientModel Client { get; set; }

        public IEnumerable<SelectListItem> AccessTokenTypes { get; set; }
        public IEnumerable<string> GrantTypes { get; set; }
        public IEnumerable<SelectListItem> ProtocolTypes { get; set; }
        public IEnumerable<SelectListItem> RefreshTokenExpirations { get; set; }
        public IEnumerable<SelectListItem> RefreshTokenUsages { get; set; }
        public IEnumerable<string> Scopes { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Id = id;

            if (!id.HasValue)
            {
                Client = new ClientModel();
            }
            else
            {
                var client = await LoadClient(id);

                if (client == null)
                {
                    return NotFound();
                }

                Client = client.ToModel();
            }

            await LoadLookups();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadLookups();
                return Page();
            }

            Client client;
            var isNew = Client.Id == 0;

            if (isNew)
            {
                PrepareClientTypeForNewClient(Client);
                client = Client.ToEntity();
                await _dbContext.Clients.AddAsync(client);
            }
            else
            {
                client = await LoadClient(Client.Id);
                Client.ToEntity(client);
            }

            await _dbContext.SaveChangesAsync();

            return isNew
                ? RedirectToPage("/Clients/Edit", new {id = client.Id})
                : RedirectToPage("/Clients/Index");
        }

        private static IEnumerable<SelectListItem> GetProtocolTypes()
        {
            return new[] {new SelectListItem("oidc", "OpenID Connect")};
        }

        private async Task<IEnumerable<string>> GetScopes()
        {
            var apiScopes = await _dbContext.ApiResources
                .SelectMany(x => x.Scopes.Select(s => s.Scope)).ToListAsync();
            var identityScopes = await _dbContext.IdentityResources.Select(x => x.Name).ToListAsync();

            return identityScopes.Concat(apiScopes);
        }

        private async Task<Client> LoadClient(int? id)
        {
            return await _dbContext.Clients
                .Include(x => x.AllowedGrantTypes)
                .Include(x => x.AllowedScopes)
                .Include(x => x.IdentityProviderRestrictions)
                .Include(x => x.PostLogoutRedirectUris)
                .Include(x => x.RedirectUris)
                .SingleOrDefaultAsync(x => x.Id == id.Value);
        }

        private async Task LoadLookups()
        {
            AccessTokenTypes = EnumExtensions.ToSelectList<AccessTokenType>();
            ProtocolTypes = GetProtocolTypes();
            RefreshTokenExpirations = EnumExtensions.ToSelectList<TokenExpiration>();
            RefreshTokenUsages = EnumExtensions.ToSelectList<TokenUsage>();
            Scopes = await GetScopes();
            GrantTypes = PredefinedData.GetGrantTypes();
        }

        private void PrepareClientTypeForNewClient(ClientModel client)
        {
            switch (client.ClientType)
            {
                case ClientType.Empty:
                    break;
                case ClientType.WebHybrid:
                    client.AllowedGrantTypes.AddRange(IdentityServer4.Models.GrantTypes.Hybrid);
                    break;
                case ClientType.Spa:
                    client.AllowedGrantTypes.AddRange(IdentityServer4.Models.GrantTypes.Code);
                    client.RequirePkce = true;
                    client.RequireClientSecret = false;
                    break;
                case ClientType.Native:
                    client.AllowedGrantTypes.AddRange(IdentityServer4.Models.GrantTypes.Hybrid);
                    break;
                case ClientType.Machine:
                    client.AllowedGrantTypes.AddRange(IdentityServer4.Models.GrantTypes.ResourceOwnerPasswordAndClientCredentials);
                    break;
                case ClientType.Device:
                    client.AllowedGrantTypes.AddRange(IdentityServer4.Models.GrantTypes.DeviceFlow);
                    client.RequireClientSecret = false;
                    client.AllowOfflineAccess = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
