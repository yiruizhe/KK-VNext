using IdentityService.Domain;
using Microsoft.Extensions.Logging;

namespace IdentityService.Infrastructure
{
    public class MockSmsSender : ISmsSender
    {
        private readonly ILogger<MockSmsSender> logger;

        public MockSmsSender(ILogger<MockSmsSender> logger)
        {
            this.logger = logger;
        }

        public Task SendAsync(string phoneNum, params string[] args)
        {
            logger.LogInformation($"向{phoneNum}发送短信{string.Join(',', args)}");
            return Task.CompletedTask;
        }
    }
}
