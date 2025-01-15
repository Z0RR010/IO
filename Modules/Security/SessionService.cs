using Microsoft.AspNetCore.Components.Authorization;

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

	}
}
