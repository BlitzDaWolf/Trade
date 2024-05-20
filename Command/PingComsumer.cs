using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Command
{
    public class PingComsumer : IConsumer<Ping>
    {
        private readonly ILogger<PingComsumer> _logger;
        public PingComsumer(ILogger<PingComsumer> logger)
        {
            _logger = logger;
        }

        Task IConsumer<Ping>.Consume(ConsumeContext<Ping> context)
        {
            var btn = context.Message.Button;
            _logger.LogInformation("Button pressed {button}", btn);
            return Task.CompletedTask;
        }
    }
}
