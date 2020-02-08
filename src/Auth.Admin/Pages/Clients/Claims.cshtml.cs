using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth.Admin.Mappers;
using Auth.Admin.Models;
using Auth.Admin.Services;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Auth.Admin.Pages.Clients
{
    public class ClaimsModel : PageModel
    {
        private readonly ConfigurationDbContext _dbContext;

        public ClaimsModel(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public int ClientId { get; set; }

        [BindProperty]
        public string ClientName { get; set; }

        public List<ClientClaimModel> Claims { get; set; }

        [BindProperty]
        public string Type { get; set; }

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

            client.Claims.Add(
                new ClientClaim
                {
                    Type = Type,
                    Value = Value
                });

            await _dbContext.SaveChangesAsync();

            return RedirectToPage("/Clients/Claims", new { id = client.Id });
        }

        private async Task<Client> LoadClient(int? id)
        {
            return await _dbContext.Clients
                .Include(x => x.Claims)
                .SingleOrDefaultAsync(x => x.Id == id.Value);
        }

        private void LoadLookups(Client client)
        {
            Types = new[] {new SelectListItem("<Custom>", string.Empty)}
                .Union(PredefinedData.GetClaimTypes().Select(x => new SelectListItem(x, x)));
            Claims = client.Claims.Select(ClientMappers.ToModel).ToList();
        }
    }
}
