using System.Threading.Tasks;
using Auth.Admin.Mappers;
using Auth.Admin.Models;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.Admin.Pages.Clients
{
    public class EditModel : PageModel
    {
        private readonly ConfigurationDbContext _dbContext;

        public int? Id { get; set; }
        public ClientModel Client { get; set; }

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
                var client = await _dbContext.Clients.FindAsync(id.Value);

                if (client == null)
                {
                    return NotFound();
                }

                Client = client.ToModel();
            }

            return Page();
        }
    }
}