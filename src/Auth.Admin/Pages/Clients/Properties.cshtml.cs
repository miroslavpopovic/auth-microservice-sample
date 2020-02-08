using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth.Admin.Mappers;
using Auth.Admin.Models;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Auth.Admin.Pages.Clients
{
    public class PropertiesModel : PageModel
    {
        private readonly ConfigurationDbContext _dbContext;

        public PropertiesModel(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public int ClientId { get; set; }

        [BindProperty]
        public string ClientName { get; set; }

        [BindProperty]
        public string Key { get; set; }

        public IEnumerable<ClientPropertyModel> Properties { get; set; }

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
            Properties = client.Properties.Select(ClientMappers.ToModel);

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
                return Page();
            }

            client.Properties.Add(
                new ClientProperty
                {
                    Key = Key,
                    Value = Value
                });

            await _dbContext.SaveChangesAsync();

            return RedirectToPage("/Clients/Properties", new { id = client.Id });
        }

        private Task<Client> LoadClient(int id)
        {
            return _dbContext.Clients
                .Include(x => x.Properties)
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}
