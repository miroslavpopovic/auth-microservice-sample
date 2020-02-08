using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.Admin.Pages.Clients
{
    public class DeleteClaimModel : PageModel
    {
        private readonly ConfigurationDbContext _dbContext;

        public DeleteClaimModel(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int ClientId { get; set; }
        public string ClientName { get; set; }

        [BindProperty]
        public int Id { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var clientClaim = await _dbContext.FindAsync<ClientClaim>(id);

            if (clientClaim == null)
            {
                return NotFound();
            }

            Id = id;
            Type = clientClaim.Type;
            Value = clientClaim.Value;

            var client = await _dbContext.Clients.FindAsync(clientClaim.ClientId);
            ClientId = client.Id;
            ClientName = string.IsNullOrWhiteSpace(client.ClientName) ? client.ClientId : client.ClientName;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var clientClaim = await _dbContext.FindAsync<ClientClaim>(Id);

            if (clientClaim == null)
            {
                return NotFound();
            }

            _dbContext.Remove(clientClaim);
            await _dbContext.SaveChangesAsync();

            return RedirectToPage("/Clients/Claims", new { id = clientClaim.ClientId });
        }
    }
}
