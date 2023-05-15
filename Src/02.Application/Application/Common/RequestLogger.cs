using System.Threading;
using System.Threading.Tasks;
using Auth.Domain.Services;
using Domain.Shared;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Application.Common
{
    public class RequestLogger<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly ILogger _logger;
        private readonly IAppUserService _appUserService;

        public RequestLogger(ILogger<TRequest> logger, IAppUserService currentUserService)
        {
            _logger = logger;
            _appUserService = currentUserService;
        }

        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var name = typeof(TRequest).Name;

            _logger.LogInformation("App Request: {Name} {@UserName} {@Request}", 
                name, _appUserService.GetLoggedUserName(), request);

            return Task.CompletedTask;
        }
    }

}
