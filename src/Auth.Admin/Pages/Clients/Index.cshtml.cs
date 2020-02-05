using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth.Admin.Mappers;
using Auth.Admin.Models;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Auth.Admin.Pages.Clients
{
    public class IndexModel : PageModel
    {
        private readonly ConfigurationDbContext _dbContext;

        [BindProperty]
        public IEnumerable<ClientModel> Clients { get; set; }

        public IndexModel(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var clients = await _dbContext.Clients.ToListAsync();

            Clients = clients.Select(ClientMappers.ToModel);

            return Page();
        }
    }
}