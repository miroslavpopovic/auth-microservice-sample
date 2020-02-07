using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.Admin.Pages.Clients
{
    public class DeleteSecretModel : PageModel
    {
        private readonly ConfigurationDbContext _dbContext;

        public DeleteSecretModel(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string Description { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        [BindProperty]
        public int Id { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var clientSecret = await _dbContext.FindAsync<ClientSecret>(id);

            if (clientSecret == null)
            {
                return NotFound();
            }

            Id = id;
            Description = clientSecret.Description;
            Type = clientSecret.Type;
            Value = clientSecret.Value;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var clientSecret = await _dbContext.FindAsync<ClientSecret>(Id);

            if (clientSecret == null)
            {
                return NotFound();
            }

            _dbContext.Remove(clientSecret);
            await _dbContext.SaveChangesAsync();

            return RedirectToPage("/Clients/Secrets", new { id = clientSecret.ClientId });
        }
    }
}
