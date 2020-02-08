using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.Admin.Pages.ApiResources
{
    public class DeleteScopeModel : PageModel
    {
        private readonly ConfigurationDbContext _dbContext;

        public DeleteScopeModel(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int ApiResourceId { get; set; }
        public string ApiResourceName { get; set; }

        [BindProperty]
        public int Id { get; set; }

        public string Name { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var scope = await _dbContext.FindAsync<ApiScope>(id);

            if (scope == null)
            {
                return NotFound();
            }

            Id = id;
            Name = scope.Name;

            var apiResource = await _dbContext.ApiResources.FindAsync(scope.ApiResourceId);
            ApiResourceId = apiResource.Id;
            ApiResourceName = string.IsNullOrWhiteSpace(apiResource.DisplayName)
                ? apiResource.Name
                : apiResource.DisplayName;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var scope = await _dbContext.FindAsync<ApiScope>(Id);

            if (scope == null)
            {
                return NotFound();
            }

            _dbContext.Remove(scope);
            await _dbContext.SaveChangesAsync();

            return RedirectToPage("/ApiResources/Scopes", new { id = scope.ApiResourceId });
        }
    }
}
