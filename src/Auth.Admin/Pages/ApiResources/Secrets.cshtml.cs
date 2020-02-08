using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth.Admin.Extensions;
using Auth.Admin.Mappers;
using Auth.Admin.Models;
using Auth.Admin.Services;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApiResource = IdentityServer4.EntityFramework.Entities.ApiResource;

namespace Auth.Admin.Pages.ApiResources
{
    public class SecretsModel : PageModel
    {
        private readonly ConfigurationDbContext _dbContext;

        public SecretsModel(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public int ApiResourceId { get; set; }

        [BindProperty]
        public string ApiResourceName { get; set; }

        public List<ApiResourceSecretModel> Secrets { get; set; }

        [BindProperty]
        public string Description { get; set; }

        [BindProperty]
        public DateTime? Expiration { get; set; }

        [BindProperty]
        public string HashType { get; set; }

        public IEnumerable<SelectListItem> HashTypes { get; set; }


        [BindProperty]
        public string Type { get; set; } = "SharedSecret";

        public IEnumerable<SelectListItem> Types { get; set; }

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
            ApiResourceName = string.IsNullOrWhiteSpace(apiResource.DisplayName) ? apiResource.Name : apiResource.DisplayName;

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

            apiResource.Secrets.Add(
                new ApiSecret
                {
                    Created = DateTime.UtcNow,
                    Description = Description,
                    Expiration = Expiration,
                    Type = Type,
                    Value = HashClientSharedSecret(Type, Value)
                });

            await _dbContext.SaveChangesAsync();

            return RedirectToPage("/ApiResources/Secrets", new {id = apiResource.Id});
        }

        private string HashClientSharedSecret(string type, string value)
        {
            if (type != "SharedSecret") return value;

            if (HashType == ((int) Models.HashType.Sha256).ToString())
            {
                return value.Sha256();
            }

            if (HashType == ((int) Models.HashType.Sha512).ToString())
            {
                return value.Sha512();
            }

            return value;
        }

        private async Task<ApiResource> LoadApiResource(int? id)
        {
            return await _dbContext.ApiResources
                .Include(x => x.Secrets)
                .SingleOrDefaultAsync(x => x.Id == id.Value);
        }

        private void LoadLookups(ApiResource apiResource)
        {
            HashTypes = EnumExtensions.ToSelectList<HashType>();
            Types = PredefinedData.GetSecretTypes().Select(x => new SelectListItem(x, x));
            Secrets = apiResource.Secrets.Select(ApiResourceMappers.ToModel).ToList();
        }
    }
}
