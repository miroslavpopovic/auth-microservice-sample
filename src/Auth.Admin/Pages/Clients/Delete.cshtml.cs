using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.Admin.Pages.Clients
{
    public class DeleteModel : PageModel
    {
        private readonly ConfigurationDbContext _dbContext;

        public DeleteModel(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string ClientId { get; set; }
        public string ClientName { get; set; }

        [BindProperty]
        public int Id { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = await _dbContext.Clients.FindAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            Id = id;
            ClientId = client.ClientId;
            ClientName = client.ClientName;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = await _dbContext.Clients.FindAsync(Id);

            if (client == null)
            {
                return NotFound();
            }

            _dbContext.Clients.Remove(client);
            await _dbContext.SaveChangesAsync();

            return RedirectToPage("/Clients/Index");
        }
    }
}
