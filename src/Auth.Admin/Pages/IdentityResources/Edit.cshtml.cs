using System.Collections.Generic;
using System.Threading.Tasks;
using Auth.Admin.Mappers;
using Auth.Admin.Models;
using Auth.Admin.Services;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Auth.Admin.Pages.IdentityResources
{
    public class EditModel : PageModel
    {
        private readonly ConfigurationDbContext _dbContext;

        public int? Id { get; set; }

        [BindProperty]
        public IdentityResourceModel IdentityResource { get; set; }

        public IEnumerable<string> Claims { get; set; }

        public EditModel(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Id = id;

            if (!id.HasValue)
            {
                IdentityResource = new IdentityResourceModel();
            }
            else
            {
                var identityResource = await LoadIdentityResource(id);

                if (identityResource == null)
                {
                    return NotFound();
                }

                IdentityResource = identityResource.ToModel();
            }

            LoadLookups();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                LoadLookups();
                return Page();
            }

            IdentityResource identityResource;
            var isNew = IdentityResource.Id == 0;

            if (isNew)
            {
                identityResource = IdentityResource.ToEntity();
                await _dbContext.IdentityResources.AddAsync(identityResource);
            }
            else
            {
                identityResource = await LoadIdentityResource(IdentityResource.Id);
                IdentityResource.ToEntity(identityResource);
            }

            await _dbContext.SaveChangesAsync();

            return isNew
                ? RedirectToPage("/IdentityResources/Edit", new { id = identityResource.Id })
                : RedirectToPage("/IdentityResources/Index");
        }

        private async Task<IdentityResource> LoadIdentityResource(int? id)
        {
            return await _dbContext.IdentityResources
                .Include(x => x.UserClaims)
                .SingleOrDefaultAsync(x => x.Id == id.Value);
        }

        private void LoadLookups()
        {
            Claims = PredefinedData.GetClaimTypes();
        }
    }
}
