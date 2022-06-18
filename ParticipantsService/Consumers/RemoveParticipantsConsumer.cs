using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MassTransit;
using Entities.Commands;
using Entities.Events;

namespace ParticipantsService.Consumers
{
    public class RemoveParticipantsConsumer : IConsumer<RemoveParticipants>
    {
        readonly ILogger<RemoveParticipants> _logger;

        public RemoveParticipantsConsumer(ILogger<RemoveParticipants> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<RemoveParticipants> context)
        {
            _logger.LogInformation("ParticipantsService -> RemoveParticipantsConsumer: Got RemoveParticipantsConsumer command, correlation id: {id}",
                context.CorrelationId);

            // Remove participant logic here...

            // After removing the participants, publish the ParticipantsRemoved event
            _logger.LogInformation("ParticipantsService -> RemoveParticipantsConsumer: publish ParticipantsRemoved event, correlation id: {id}",
                context.CorrelationId);

            await context.Publish(new ParticipantsRemoved(context.Message.CorrelationId));
        }
    }
}
