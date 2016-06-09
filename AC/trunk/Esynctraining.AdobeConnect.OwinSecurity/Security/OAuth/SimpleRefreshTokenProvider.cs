using System;
using System.Threading.Tasks;
using Esynctraining.AdobeConnect.OwinSecurity.Security.Tokens;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Infrastructure;

namespace Esynctraining.AdobeConnect.OwinSecurity.Security.OAuth
{
    public class SimpleRefreshTokenProvider : IAuthenticationTokenProvider
    {
        private readonly ITokenService _tokenService;
        public SimpleRefreshTokenProvider(ITokenService tokenService)
        {
            this._tokenService = tokenService;
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            CreateAsync(context).ConfigureAwait(false);
        }

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var refreshTokenId = Guid.NewGuid();
            var identity = context.Ticket.Identity;
            var id = identity.GetUserId();
            var domain = identity.FindFirst(x => x.Type == "ac_domain");
            var companyToken = identity.FindFirst(x => x.Type == "c_token");

            var protectedTicket = context.SerializeTicket();
            var expires = DateTime.UtcNow.AddMinutes(20);
            var saved = await _tokenService.SaveTokenAsync(id, companyToken.Value, domain.Value, refreshTokenId, expires, protectedTicket);
            if (saved)
            {
                context.SetToken(refreshTokenId.ToString("n"));
            }
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            ReceiveAsync(context).ConfigureAwait(false);
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            var token = new Guid(context.Token);
            var protectedTicket = await _tokenService.GetProtectedTicket(token);
            if (!string.IsNullOrEmpty(protectedTicket))
            {
                context.DeserializeTicket(protectedTicket);
                await _tokenService.Delete(token);
            }
        }
    }
}