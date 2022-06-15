using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MassTransit;
using Entities.Commands;
using Entities.Events;

namespace DocumentsService.Consumers
{
    public class AddDocumentsConsumer : IConsumer<AddDocuments>
    {
        readonly ILogger<AddDocuments> _logger;

        public AddDocumentsConsumer(ILogger<AddDocuments> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<AddDocuments> context)
        {
            _logger.LogInformation("DocumentsService -> AddDocumentsConsumer: Got AddDocuments command, correlation id: {id}",
                context.CorrelationId);

            if (context.Message.PermitRequest.Documents.All(i => i.IsValidDocument))
            {
                _logger.LogInformation("DocumentsService -> AddDocumentsConsumer: publish DocumentAdded event, correlation id: {id}",
                    context.CorrelationId);

                await context.Publish(new DocumentsAdded { PermitRequest = context.Message.PermitRequest });
            }
            else
            {
                _logger.LogInformation("DocumentsService -> AddDocumentsConsumer: Found INVALID Document so publish DocumentRejected event, correlation id: {id}",
                    context.CorrelationId);

                await context.Publish(new DocumentRejected { PermitRequest = context.Message.PermitRequest });
            }
        }
    }
}
