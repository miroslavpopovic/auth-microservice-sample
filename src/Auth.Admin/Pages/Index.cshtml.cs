using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Auth.Admin.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ConfigurationDbContext _dbContext;

        public IndexModel(ILogger<IndexModel> logger, ConfigurationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public int ClientCount { get; set; }
        public int ApiResourceCount { get; set; }
        public int IdentityResourceCount { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ClientCount = await _dbContext.Clients.CountAsync();
            ApiResourceCount = await _dbContext.ApiResources.CountAsync();
            IdentityResourceCount = await _dbContext.IdentityResources.CountAsync();

            return Page();
        }
    }
}
