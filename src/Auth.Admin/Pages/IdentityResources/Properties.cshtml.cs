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

namespace Auth.Admin.Pages.IdentityResources
{
    public class PropertiesModel : PageModel
    {
        private readonly ConfigurationDbContext _dbContext;

        public PropertiesModel(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public int IdentityResourceId { get; set; }

        [BindProperty]
        public string IdentityResourceName { get; set; }

        [BindProperty]
        public string Key { get; set; }

        public IEnumerable<IdentityResourcePropertyModel> Properties { get; set; }

        [BindProperty]
        public string Value { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var identityResource = await LoadIdentityResource(id);

            if (identityResource == null)
            {
                return NotFound();
            }

            IdentityResourceId = id;
            IdentityResourceName = string.IsNullOrWhiteSpace(identityResource.DisplayName)
                ? identityResource.Name
                : identityResource.DisplayName;
            Properties = identityResource.Properties.Select(IdentityResourceMappers.ToModel);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var identityResource = await LoadIdentityResource(IdentityResourceId);

            if (identityResource == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            identityResource.Properties.Add(
                new IdentityResourceProperty
                {
                    Key = Key,
                    Value = Value
                });

            await _dbContext.SaveChangesAsync();

            return RedirectToPage("/IdentityResources/Properties", new { id = identityResource.Id });
        }

        private Task<IdentityResource> LoadIdentityResource(int id)
        {
            return _dbContext.IdentityResources
                .Include(x => x.Properties)
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}
