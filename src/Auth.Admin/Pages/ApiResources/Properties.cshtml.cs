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

namespace Auth.Admin.Pages.ApiResources
{
    public class PropertiesModel : PageModel
    {
        private readonly ConfigurationDbContext _dbContext;

        public PropertiesModel(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public int ApiResourceId { get; set; }

        [BindProperty]
        public string ApiResourceName { get; set; }

        [BindProperty]
        public string Key { get; set; }

        public IEnumerable<ApiResourcePropertyModel> Properties { get; set; }

        [BindProperty]
        public string Value { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var apiResource = await LoadApiResource(id);

            if (apiResource == null)
            {
                return NotFound();
            }

            ApiResourceId = id;
            ApiResourceName = string.IsNullOrWhiteSpace(apiResource.DisplayName)
                ? apiResource.Name
                : apiResource.DisplayName;
            Properties = apiResource.Properties.Select(ApiResourceMappers.ToModel);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var apiResource = await LoadApiResource(ApiResourceId);

            if (apiResource == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            apiResource.Properties.Add(
                new ApiResourceProperty
                {
                    Key = Key,
                    Value = Value
                });

            await _dbContext.SaveChangesAsync();

            return RedirectToPage("/ApiResources/Properties", new { id = apiResource.Id });
        }

        private Task<ApiResource> LoadApiResource(int id)
        {
            return _dbContext.ApiResources
                .Include(x => x.Properties)
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}
