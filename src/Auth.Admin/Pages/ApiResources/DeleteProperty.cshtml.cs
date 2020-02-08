using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.Admin.Pages.ApiResources
{
    public class DeletePropertyModel : PageModel
    {
        private readonly ConfigurationDbContext _dbContext;

        public DeletePropertyModel(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int ApiResourceId { get; set; }
        public string ApiResourceName { get; set; }

        [BindProperty]
        public int Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var property = await _dbContext.FindAsync<ApiResourceProperty>(id);

            if (property == null)
            {
                return NotFound();
            }

            Id = id;
            Key = property.Key;
            Value = property.Value;

            var apiResource = await _dbContext.ApiResources.FindAsync(property.ApiResourceId);
            ApiResourceId = apiResource.Id;
            ApiResourceName = string.IsNullOrWhiteSpace(apiResource.DisplayName)
                ? apiResource.Name
                : apiResource.DisplayName;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var property = await _dbContext.FindAsync<ApiResourceProperty>(Id);

            if (property == null)
            {
                return NotFound();
            }

            _dbContext.Remove(property);
            await _dbContext.SaveChangesAsync();

            return RedirectToPage("/ApiResources/Properties", new { id = property.ApiResourceId });
        }
    }
}
