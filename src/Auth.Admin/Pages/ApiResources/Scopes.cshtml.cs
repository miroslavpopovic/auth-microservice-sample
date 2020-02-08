using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ScopesModel : PageModel
    {
        private readonly ConfigurationDbContext _dbContext;

        public ScopesModel(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public int ApiResourceId { get; set; }

        [BindProperty]
        public string ApiResourceName { get; set; }

        public IEnumerable<string> Claims { get; set; }

        [BindProperty]
        public ApiResourceScopeModel Scope { get; set; }

        public List<ApiResourceScopeModel> Scopes { get; set; }

        public async Task<IActionResult> OnGetAsync(int id, int? scope)
        {
            var apiResource = await LoadApiResource(id);

            if (apiResource == null)
            {
                return NotFound();
            }

            if (scope.HasValue)
            {
                var apiScope = apiResource.Scopes.FirstOrDefault(x => x.Id == scope.Value);

                if (apiScope == null)
                {
                    return NotFound();
                }

                Scope = apiScope.ToModel();
            }
            else
            {
                Scope = new ApiResourceScopeModel();
            }

            ApiResourceId = id;
            ApiResourceName = string.IsNullOrWhiteSpace(apiResource.DisplayName)
                ? apiResource.Name
                : apiResource.DisplayName;

            LoadLookups(apiResource);

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
                LoadLookups(apiResource);
                return Page();
            }

            ApiScope scope;
            if (Scope.Id == 0)
            {
                scope = Scope.ToEntity();
                apiResource.Scopes.Add(scope);
            }
            else
            {
                scope = apiResource.Scopes.FirstOrDefault(x => x.Id == Scope.Id);
                Scope.ToEntity(scope);
            }

            await _dbContext.SaveChangesAsync();

            return RedirectToPage("/ApiResources/Scopes", new { id = apiResource.Id });
        }

        private async Task<ApiResource> LoadApiResource(int? id)
        {
            return await _dbContext.ApiResources
                .Include(x => x.Scopes)
                .ThenInclude(x => x.UserClaims)
                .SingleOrDefaultAsync(x => x.Id == id.Value);
        }

        private void LoadLookups(ApiResource apiResource)
        {
            Claims = PredefinedData.GetClaimTypes();
            Scopes = apiResource.Scopes.Select(ApiResourceMappers.ToModel).ToList();
        }
    }
}
