using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MassTransit;
using Entities.Commands;
using Entities.Events;

namespace ParticipantsService.Consumers
{
    public class AddParticipantsConsumer : IConsumer<AddParticipants>
    {
        readonly ILogger<AddParticipants> _logger;

        public AddParticipantsConsumer(ILogger<AddParticipants> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<AddParticipants> context)
        {
            _logger.LogInformation("ParticipantsService -> AddParticipantsConsumer: Got AddParticipants command, correlation id: {id}",
                context.CorrelationId);

            // Add participant logic here...

            // After adding the participants, publish the ParticipantsAdded event
            _logger.LogInformation("ParticipantsService -> AddParticipantConsumer: publish ParticipantsAdded event, correlation id: {id}",
                context.CorrelationId);

            await context.Publish(new ParticipantsAdded { PermitRequest = context.Message.PermitRequest });
        }
    }
}
