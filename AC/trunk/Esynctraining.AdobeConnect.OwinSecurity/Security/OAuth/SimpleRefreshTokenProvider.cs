using System;
using System.Threading.Tasks;
using Esynctraining.AdobeConnect.OwinSecurity.Security.Tokens;
using Esynctraining.Core.Logging;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Infrastructure;

namespace Esynctraining.AdobeConnect.OwinSecurity.Security.OAuth
{
    public class SimpleRefreshTokenProvider : IAuthenticationTokenProvider
    {
        private readonly ITokenService _tokenService;
        private readonly ILogger _logger;


        public SimpleRefreshTokenProvider(ITokenService tokenService, ILogger logger)
        {
            if (tokenService == null)
                throw new ArgumentNullException(nameof(tokenService));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _tokenService = tokenService;
            _logger = logger;
        }


        public void Create(AuthenticationTokenCreateContext context)
        {
            Task.Run(async () => { await CreateAsync(context); }).Wait();
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
//                _logger.Info($"[TokenProvider.CreateAsync] Id={id}, SetToken={refreshTokenId}");
            }
            else
            {
                _logger.Warn($"[TokenProvider.CreateAsync] Token not set. Id={id}, Token={refreshTokenId}");
            }
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            Task.Run(async () => { await ReceiveAsync(context); }).Wait();
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
//            _logger.Info($"[TokenProvider.ReceiveAsync] Incoming token={context.Token}");
            var token = new Guid(context.Token);
            var protectedTicket = await _tokenService.GetProtectedTicket(token);
            if (!string.IsNullOrEmpty(protectedTicket))
            {
                context.DeserializeTicket(protectedTicket);
//                _logger.Info($"[TokenRefresh] ACSession={context.Ticket.Identity.FindFirst(x => x.Type == "ac_session")}");
                await _tokenService.Delete(token);
            }
        }
    }
}