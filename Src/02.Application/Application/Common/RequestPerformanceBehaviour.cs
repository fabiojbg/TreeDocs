using Auth.Domain.RequestsResponses;
using Auth.Domain.Services;
using Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Behaviours
{
    public class RequestPerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly Stopwatch _timer;
        private readonly ILogger<TRequest> _logger;
        private readonly IAppUserService _currentUserService;

        public RequestPerformanceBehaviour(ILogger<TRequest> logger, IAppUserService currentUserService)
        {
            _timer = new Stopwatch();

            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            if (_timer.ElapsedMilliseconds > 500)
            {
                var name = typeof(TRequest).Name;

                if (request is AuthenticateUserRequest)
                {  // avoid the logging of the user password.
                    var authenticateRequest = request as AuthenticateUserRequest;
                    _logger.LogWarning("Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@Request}",
                        name, _timer.ElapsedMilliseconds, _currentUserService.GetLoggedUserId(),
                        new { UserMail = authenticateRequest.UserEmail, UserIp = authenticateRequest.UserIP });
                }
                else
                    _logger.LogWarning("Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@Request}",
                        name, _timer.ElapsedMilliseconds, _currentUserService.GetLoggedUserId(), request);
            }

            return response;
        }
    }
}
