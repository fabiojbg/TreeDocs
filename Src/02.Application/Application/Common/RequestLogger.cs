using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Auth.Domain.Services;
using Domain.Shared;
using Auth.Domain.RequestsResponses;

namespace Application.Common.Behaviours
{
    public class RequestLoggerBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly Stopwatch _timer;
        private readonly ILogger<TRequest> _logger;
        private readonly IAppUserService _currentUserService;

        public RequestLoggerBehaviour(ILogger<TRequest> logger, IAppUserService currentUserService)
        {
            _timer = new Stopwatch();

            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                var name = typeof(TRequest).Name;

                if (request is AuthenticateUserRequest)
                {  // avoid to log the user passoword.
                    var authenticateRequest = request as AuthenticateUserRequest;
                    _logger.LogInformation("Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@Request}",
                        name, _timer.ElapsedMilliseconds, _currentUserService.GetLoggedUserId(), 
                        new { UserMail = authenticateRequest.UserEmail, UserIp = authenticateRequest.UserIP});
                }
                else
                    _logger.LogInformation("Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@Request}",
                        name, _timer.ElapsedMilliseconds, _currentUserService.GetLoggedUserId(), request);
            }
            catch { }

            var response = await next();

            return response;
        }
    }
}
