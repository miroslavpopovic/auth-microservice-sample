using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth.Admin.Extensions;
using Auth.Admin.Mappers;
using Auth.Admin.Models;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace Auth.Admin.Pages.Clients
{
    public class SecretsModel : PageModel
    {
        private readonly ConfigurationDbContext _dbContext;

        public SecretsModel(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public int ClientId { get; set; }

        [BindProperty]
        public string ClientName { get; set; }

        public List<ClientSecretModel> ClientSecrets { get; set; }

        [BindProperty]
        public string Description { get; set; }

        [BindProperty]
        public DateTime? Expiration { get; set; }

        [BindProperty]
        public string HashType { get; set; }

        public IEnumerable<SelectListItem> HashTypes { get; set; }


        [BindProperty]
        public string Type { get; set; } = "SharedSecret";

        public IEnumerable<SelectListItem> Types { get; set; }

        [BindProperty]
        public string Value { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = await LoadClient(id);

            if (client == null)
            {
                return NotFound();
            }

            ClientId = id;
            ClientName = string.IsNullOrWhiteSpace(client.ClientName) ? client.ClientId : client.ClientName;

            LoadLookups(client);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = await LoadClient(ClientId);

            if (client == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                LoadLookups(client);
                return Page();
            }

            client.ClientSecrets.Add(
                new ClientSecret
                {
                    Created = DateTime.UtcNow,
                    Description = Description,
                    Expiration = Expiration,
                    Type = Type,
                    Value = HashClientSharedSecret(Type, Value)
                });

            await _dbContext.SaveChangesAsync();

            return RedirectToPage("/Clients/Secrets", new {id = client.Id});
        }

        private static List<string> GetSecretTypes()
        {
            var secretTypes = new List<string>
            {
                "SharedSecret",
                "X509Thumbprint",
                "X509Name",
                "X509CertificateBase64"
            };

            return secretTypes;
        }

        private string HashClientSharedSecret(string type, string value)
        {
            if (type != "SharedSecret") return value;

            if (HashType == ((int) Models.HashType.Sha256).ToString())
            {
                return value.Sha256();
            }

            if (HashType == ((int) Models.HashType.Sha512).ToString())
            {
                return value.Sha512();
            }

            return value;
        }

        private async Task<Client> LoadClient(int? id)
        {
            return await _dbContext.Clients
                .Include(x => x.ClientSecrets)
                .SingleOrDefaultAsync(x => x.Id == id.Value);
        }

        private void LoadLookups(Client client)
        {
            HashTypes = EnumExtensions.ToSelectList<HashType>();
            Types = GetSecretTypes().Select(x => new SelectListItem(x, x));
            ClientSecrets = client.ClientSecrets.Select(ClientMappers.ToModel).ToList();
        }
    }
}
