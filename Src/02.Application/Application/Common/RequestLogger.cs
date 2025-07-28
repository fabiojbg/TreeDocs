using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Auth.Domain.Services;
using Domain.Shared;

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
            var response = await next();

            try
            {
                var name = typeof(TRequest).Name;

                _logger.LogWarning("Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@Request}",
                    name, _timer.ElapsedMilliseconds, _currentUserService.GetLoggedUserId(), request);
            }
            catch { }

            return response;
        }
    }
}
