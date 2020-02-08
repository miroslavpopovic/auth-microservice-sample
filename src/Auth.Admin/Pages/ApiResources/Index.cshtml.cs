using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth.Admin.Mappers;
using Auth.Admin.Models;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Auth.Admin.Pages.ApiResources
{
    public class IndexModel : PageModel
    {
        private readonly ConfigurationDbContext _dbContext;

        [BindProperty]
        public IEnumerable<ApiResourceModel> ApiResources { get; set; }

        public IndexModel(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var clients = await _dbContext.ApiResources.ToListAsync();

            ApiResources = clients.Select(ApiResourceMappers.ToModel);

            return Page();
        }
    }
}
