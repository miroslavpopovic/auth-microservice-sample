using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.Admin.Pages.ApiResources
{
    public class DeleteSecretModel : PageModel
    {
        private readonly ConfigurationDbContext _dbContext;

        public DeleteSecretModel(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int ApiResourceId { get; set; }
        public string ApiResourceName { get; set; }

        public string Description { get; set; }

        [BindProperty]
        public int Id { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var secret = await _dbContext.FindAsync<ApiSecret>(id);

            if (secret == null)
            {
                return NotFound();
            }

            Id = id;
            Description = secret.Description;
            Type = secret.Type;
            Value = secret.Value;

            var apiResource = await _dbContext.ApiResources.FindAsync(secret.ApiResourceId);
            ApiResourceId = apiResource.Id;
            ApiResourceName = string.IsNullOrWhiteSpace(apiResource.DisplayName)
                ? apiResource.Name
                : apiResource.DisplayName;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var secret = await _dbContext.FindAsync<ApiSecret>(Id);

            if (secret == null)
            {
                return NotFound();
            }

            _dbContext.Remove(secret);
            await _dbContext.SaveChangesAsync();

            return RedirectToPage("/ApiResources/Secrets", new { id = secret.ApiResourceId });
        }
    }
}
