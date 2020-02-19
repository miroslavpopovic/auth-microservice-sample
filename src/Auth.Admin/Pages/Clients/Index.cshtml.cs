using System;
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

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public string Search { get; set; }

        public int TotalPages { get; set; }

        public IndexModel(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> OnGetAsync(string search, int p = 1, int size = 10)
        {
            var clientCount = await _dbContext.Clients
                .CountAsync(x => search == null || x.ClientId.Contains(search) || x.ClientName.Contains(search));

            var clients = await _dbContext.Clients
                .Where(x => search == null || x.ClientId.Contains(search) || x.ClientName.Contains(search))
                .Skip((p - 1) * size)
                .Take(size)
                .ToListAsync();

            Clients = clients.Select(ClientMappers.ToModel);

            CurrentPage = p;
            PageSize = size;
            Search = search;
            TotalPages = (int) Math.Ceiling((decimal) clientCount / PageSize);

            return Page();
        }
    }
}