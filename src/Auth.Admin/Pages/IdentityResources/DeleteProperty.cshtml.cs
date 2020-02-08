using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.Admin.Pages.IdentityResources
{
    public class DeletePropertyModel : PageModel
    {
        private readonly ConfigurationDbContext _dbContext;

        public DeletePropertyModel(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int IdentityResourceId { get; set; }
        public string IdentityResourceName { get; set; }

        [BindProperty]
        public int Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var property = await _dbContext.FindAsync<IdentityResourceProperty>(id);

            if (property == null)
            {
                return NotFound();
            }

            Id = id;
            Key = property.Key;
            Value = property.Value;

            var identityResource = await _dbContext.IdentityResources.FindAsync(property.IdentityResourceId);
            IdentityResourceId = identityResource.Id;
            IdentityResourceName = string.IsNullOrWhiteSpace(identityResource.DisplayName)
                ? identityResource.Name
                : identityResource.DisplayName;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var property = await _dbContext.FindAsync<IdentityResourceProperty>(Id);

            if (property == null)
            {
                return NotFound();
            }

            _dbContext.Remove(property);
            await _dbContext.SaveChangesAsync();

            return RedirectToPage("/IdentityResources/Properties", new { id = property.IdentityResourceId });
        }
    }
}
