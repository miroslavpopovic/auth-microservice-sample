using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.Admin.Pages.ApiResources
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
            var apiResource = await _dbContext.ApiResources.FindAsync(id);

            if (apiResource == null)
            {
                return NotFound();
            }

            Id = id;
            Name = string.IsNullOrWhiteSpace(apiResource.DisplayName) ? apiResource.Name : apiResource.DisplayName;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var apiResource = await _dbContext.ApiResources.FindAsync(Id);

            if (apiResource == null)
            {
                return NotFound();
            }

            _dbContext.ApiResources.Remove(apiResource);
            await _dbContext.SaveChangesAsync();

            return RedirectToPage("/ApiResources/Index");
        }
    }
}
