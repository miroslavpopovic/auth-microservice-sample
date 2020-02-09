using System.Threading.Tasks;
using IdentityServer4.Stores;

namespace Auth.Extensions
{
    public static class IdentityServerExtensions
    {
        /// <summary>
        /// Determines whether the client is configured to use PKCE.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        public static async Task<bool> IsPkceClientAsync(this IClientStore store, string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId)) return false;

            var client = await store.FindEnabledClientByIdAsync(clientId);
            return client?.RequirePkce == true;
        }
    }
}
