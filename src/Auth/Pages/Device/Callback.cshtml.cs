using System.Linq;
using System.Threading.Tasks;
using Auth.Models;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Auth.Pages.Device
{
    public class CallbackModel : PageModel
    {
        private readonly IDeviceFlowInteractionService _interaction;
        private readonly IEventService _events;
        private readonly ILogger<IndexModel> _logger;

        public CallbackModel(
            IDeviceFlowInteractionService interaction, IEventService events, ILogger<IndexModel> logger)
        {
            _interaction = interaction;
            _events = events;
            _logger = logger;
        }

        [BindProperty]
        public DeviceAuthorizationInputModel Model { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var result = await ProcessConsent(Model);

            if (result.HasValidationError)
            {
                return RedirectToPage("/Error");
            }

            if (result.RedisplayConsentUI)
            {
                return RedirectToPage("/Device/Index", new {userCode = Model.UserCode});
            }

            return Page();
        }

        private async Task<ProcessConsentResult> ProcessConsent(DeviceAuthorizationInputModel model)
        {
            var result = new ProcessConsentResult();

            var request = await _interaction.GetAuthorizationContextAsync(model.UserCode);
            if (request == null) return result;

            ConsentResponse grantedConsent = null;

            // user clicked 'no' - send back the standard 'access_denied' response
            if (model.Button == "no")
            {
                grantedConsent = ConsentResponse.Denied;

                // emit event
                await _events.RaiseAsync(
                    new ConsentDeniedEvent(User.GetSubjectId(), request.ClientId, request.ScopesRequested));
            }
            // user clicked 'yes' - validate the data
            else if (model.Button == "yes")
            {
                // if the user consented to some scope, build the response model
                if (model.ScopesConsented != null && model.ScopesConsented.Any())
                {
                    var scopes = model.ScopesConsented;

                    if (ConsentOptions.EnableOfflineAccess == false)
                    {
                        scopes = scopes.Where(x => x != IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess);
                    }

                    grantedConsent = new ConsentResponse
                    {
                        RememberConsent = model.RememberConsent,
                        ScopesConsented = scopes.ToArray()
                    };

                    // emit event
                    await _events.RaiseAsync(
                        new ConsentGrantedEvent(
                            User.GetSubjectId(), request.ClientId, request.ScopesRequested,
                            grantedConsent.ScopesConsented, grantedConsent.RememberConsent));
                }
                else
                {
                    result.ValidationError = ConsentOptions.MustChooseOneErrorMessage;
                }
            }
            else
            {
                result.ValidationError = ConsentOptions.InvalidSelectionErrorMessage;
            }

            if (grantedConsent != null)
            {
                // communicate outcome of consent back to IdentityServer
                await _interaction.HandleRequestAsync(model.UserCode, grantedConsent);

                // indicate that's it ok to redirect back to authorization endpoint
                result.RedirectUri = model.ReturnUrl;
                result.ClientId = request.ClientId;
            }
            else
            {
                // we need to redisplay the consent UI
                result.RedisplayConsentUI = true;
            }

            return result;
        }
    }
}
