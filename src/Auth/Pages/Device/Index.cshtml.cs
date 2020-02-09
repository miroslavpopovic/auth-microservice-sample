using System.Linq;
using System.Threading.Tasks;
using Auth.Models;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Auth.Pages.Device
{
    public class IndexModel : PageModel
    {
        private readonly IDeviceFlowInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IResourceStore _resourceStore;
        private readonly IOptions<IdentityServerOptions> _options;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            IDeviceFlowInteractionService interaction, IClientStore clientStore, IResourceStore resourceStore,
            IOptions<IdentityServerOptions> options, ILogger<IndexModel> logger)
        {
            _interaction = interaction;
            _clientStore = clientStore;
            _resourceStore = resourceStore;
            _options = options;
            _logger = logger;
        }

        [BindProperty]
        public DeviceAuthorizationViewModel Model { get; set; } = new DeviceAuthorizationViewModel();

        public async Task<IActionResult> OnGetAsync()
        {
            var userCodeParamName = _options.Value.UserInteraction.DeviceVerificationUserCodeParameter;
            var userCode = Request.Query[userCodeParamName];

            if (!string.IsNullOrWhiteSpace(userCode))
            {
                Model = await BuildViewModelAsync(userCode);

                if (Model == null)
                {
                    return RedirectToPage("/Error");
                }

                Model.ConfirmUserCode = true;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string userCode)
        {
            Model = await BuildViewModelAsync(userCode);

            if (Model == null)
            {
                return RedirectToPage("/Error");
            }

            return Page();
        }

        private async Task<DeviceAuthorizationViewModel> BuildViewModelAsync(string userCode, DeviceAuthorizationInputModel model = null)
        {
            var request = await _interaction.GetAuthorizationContextAsync(userCode);

            if (request != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(request.ClientId);

                if (client != null)
                {
                    var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);

                    if (resources != null && (resources.IdentityResources.Any() || resources.ApiResources.Any()))
                    {
                        return CreateConsentViewModel(userCode, model, client, resources);
                    }

                    _logger.LogError("No scopes matching: {0}", request.ScopesRequested.Aggregate((x, y) => x + ", " + y));
                }
                else
                {
                    _logger.LogError("Invalid client id: {0}", request.ClientId);
                }
            }

            return null;
        }

        private static DeviceAuthorizationViewModel CreateConsentViewModel(
            string userCode, ConsentInputModel model, Client client, Resources resources)
        {
            var vm = new DeviceAuthorizationViewModel
            {
                UserCode = userCode,

                RememberConsent = model?.RememberConsent ?? true,
                ScopesConsented = model?.ScopesConsented ?? Enumerable.Empty<string>(),

                ClientName = client.ClientName ?? client.ClientId,
                ClientUrl = client.ClientUri,
                ClientLogoUrl = client.LogoUri,
                AllowRememberConsent = client.AllowRememberConsent
            };

            vm.IdentityScopes = resources.IdentityResources
                .Select(x => CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) || model == null))
                .ToArray();
            vm.ResourceScopes = resources.ApiResources
                .SelectMany(x => x.Scopes)
                .Select(x => CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) || model == null))
                .ToArray();

            if (ConsentOptions.EnableOfflineAccess && resources.OfflineAccess)
            {
                vm.ResourceScopes = vm.ResourceScopes.Union(
                    new[]
                    {
                        GetOfflineAccessScope(
                            vm.ScopesConsented.Contains(
                                IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess) || model == null)
                    });
            }

            return vm;
        }

        private static ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check)
        {
            return new ScopeViewModel
            {
                Name = identity.Name,
                DisplayName = identity.DisplayName,
                Description = identity.Description,
                Emphasize = identity.Emphasize,
                Required = identity.Required,
                Checked = check || identity.Required
            };
        }

        private static ScopeViewModel CreateScopeViewModel(Scope scope, bool check)
        {
            return new ScopeViewModel
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                Description = scope.Description,
                Emphasize = scope.Emphasize,
                Required = scope.Required,
                Checked = check || scope.Required
            };
        }
        private static ScopeViewModel GetOfflineAccessScope(bool check)
        {
            return new ScopeViewModel
            {
                Name = IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess,
                DisplayName = ConsentOptions.OfflineAccessDisplayName,
                Description = ConsentOptions.OfflineAccessDescription,
                Emphasize = true,
                Checked = check
            };
        }
    }
}
