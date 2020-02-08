using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.Admin.Pages.IdentityResources
{
    public class DeleteModel : PageModel
    {
        private readonly ConfigurationDbContext _dbContext;

        public DeleteModel(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string Name { get; set; }

        [BindProperty]
        public int Id { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var identityResource = await _dbContext.IdentityResources.FindAsync(id);

            if (identityResource == null)
            {
                return NotFound();
            }

            Id = id;
            Name = string.IsNullOrWhiteSpace(identityResource.DisplayName) ? identityResource.Name : identityResource.DisplayName;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var identityResource = await _dbContext.IdentityResources.FindAsync(Id);

            if (identityResource == null)
            {
                return NotFound();
            }

            _dbContext.IdentityResources.Remove(identityResource);
            await _dbContext.SaveChangesAsync();

            return RedirectToPage("/IdentityResources/Index");
        }
    }
}
