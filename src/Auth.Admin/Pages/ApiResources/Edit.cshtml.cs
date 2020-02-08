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

namespace Auth.Admin.Pages.ApiResources
{
    public class EditModel : PageModel
    {
        private readonly ConfigurationDbContext _dbContext;

        public int? Id { get; set; }

        [BindProperty]
        public ApiResourceModel ApiResource { get; set; }

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
                ApiResource = new ApiResourceModel();
            }
            else
            {
                var apiResource = await LoadApiResource(id);

                if (apiResource == null)
                {
                    return NotFound();
                }

                ApiResource = apiResource.ToModel();
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

            ApiResource apiResource;
            var isNew = ApiResource.Id == 0;

            if (isNew)
            {
                apiResource = ApiResource.ToEntity();
                await _dbContext.ApiResources.AddAsync(apiResource);
            }
            else
            {
                apiResource = await LoadApiResource(ApiResource.Id);
                ApiResource.ToEntity(apiResource);
            }

            await _dbContext.SaveChangesAsync();

            return isNew
                ? RedirectToPage("/ApiResources/Edit", new { id = apiResource.Id })
                : RedirectToPage("/ApiResources/Index");
        }

        private async Task<ApiResource> LoadApiResource(int? id)
        {
            return await _dbContext.ApiResources
                .Include(x => x.UserClaims)
                .SingleOrDefaultAsync(x => x.Id == id.Value);
        }

        private void LoadLookups()
        {
            Claims = PredefinedData.GetClaimTypes();
        }
    }
}
