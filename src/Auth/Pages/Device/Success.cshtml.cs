using System.Linq;
using System.Threading.Tasks;
using Auth.Models;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Pages.Device
{
    public class SuccessModel : DeviceAuthorizationPageModel
    {
        private readonly IDeviceFlowInteractionService _interaction;
        private readonly IEventService _events;

        public SuccessModel(IDeviceFlowInteractionService interaction, IEventService events)
            : base(interaction)
        {
            _interaction = interaction;
            _events = events;
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
                grantedConsent = new ConsentResponse{Error = AuthorizationError.AccessDenied};

                // emit event
                await _events.RaiseAsync(
                    new ConsentDeniedEvent(User.GetSubjectId(),
                        request.Client.ClientId,
                        request.ValidatedResources.RawScopeValues));
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
                        ScopesValuesConsented = scopes.ToArray(),
                        Description = model.Description
                    };

                    // emit event
                    await _events.RaiseAsync(
                        new ConsentGrantedEvent(
                            User.GetSubjectId(),
                            request.Client.ClientId,
                            request.ValidatedResources.RawScopeValues,
                            grantedConsent.ScopesValuesConsented,
                            grantedConsent.RememberConsent));
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
                result.Client = request.Client;
            }
            else
            {
                // we need to redisplay the consent UI
                result.ViewModel = await BuildViewModelAsync(model.UserCode, model);
            }

            return result;
        }
    }
}
