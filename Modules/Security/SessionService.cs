using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace IO.Modules.Security
{
	public class SessionService
	{

		private readonly AuthenticationStateProvider _authenticationStateProvider;

		public SessionService(AuthenticationStateProvider authenticationStateProvider)
		{
			_authenticationStateProvider = authenticationStateProvider;
		}

		public async Task<string> GetUserNameAsync()
		{
			var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
			var user = authState.User;

			if (user.Identity != null && user.Identity.IsAuthenticated)
			{
				return user.Identity.Name;
			}

			return "Anonymous";
		}

        public async Task<User> GetCurrentUserAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var claimsPrincipal = authState.User;

            if (claimsPrincipal.Identity != null && claimsPrincipal.Identity.IsAuthenticated)
            {
                var email = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
                var name = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
                var phone = claimsPrincipal.FindFirst(ClaimTypes.MobilePhone)?.Value;
                var address = claimsPrincipal.FindFirst("address")?.Value;
                var isActive = bool.Parse(claimsPrincipal.FindFirst("isActive")?.Value ?? "false");

                return new User(email, name, phone, address, isActive);
            }

            return null;
        }

    }
}
