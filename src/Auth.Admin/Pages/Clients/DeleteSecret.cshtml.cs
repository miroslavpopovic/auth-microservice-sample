﻿using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.Admin.Pages.Clients
{
    public class DeleteSecretModel : PageModel
    {
        private readonly ConfigurationDbContext _dbContext;

        public DeleteSecretModel(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int ClientId { get; set; }
        public string ClientName { get; set; }

        public string Description { get; set; }

        [BindProperty]
        public int Id { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var clientSecret = await _dbContext.FindAsync<ClientSecret>(id);

            if (clientSecret == null)
            {
                return NotFound();
            }

            Id = id;
            Description = clientSecret.Description;
            Type = clientSecret.Type;
            Value = clientSecret.Value;

            var client = await _dbContext.Clients.FindAsync(clientSecret.ClientId);
            ClientId = client.Id;
            ClientName = string.IsNullOrWhiteSpace(client.ClientName) ? client.ClientId : client.ClientName;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var clientSecret = await _dbContext.FindAsync<ClientSecret>(Id);

            if (clientSecret == null)
            {
                return NotFound();
            }

            _dbContext.Remove(clientSecret);
            await _dbContext.SaveChangesAsync();

            return RedirectToPage("/Clients/Secrets", new { id = clientSecret.ClientId });
        }
    }
}
