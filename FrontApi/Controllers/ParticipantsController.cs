using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MassTransit;
using Entities.Models;
using Entities.Commands;

namespace FrontApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ParticipantsController : ControllerBase
    {
        readonly IBus _bus;
        readonly ILogger<ParticipantsController> _logger;

        public ParticipantsController(IBus bus, ILogger<ParticipantsController> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        [HttpPost]
        public async Task Post(PermitRequest permitRequest, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"FrontApi -> ParticipantsController: send " +
                $"{(permitRequest.Documents.All(i => i.IsValidDocument) ? "valid" : "invalid")}-CreatePermitRequest command " +
                "with the participants: {participantsNames}" +
                "and the documents: {documentsNames}",
                permitRequest.Participants.Select(i => $"{i.Name}, "),
                permitRequest.Documents.Select(i => $"{i.Name}, "));

            permitRequest.PermitRequestId = NewId.NextGuid();

            await _bus.Publish(new CreatePermitRequest { PermitRequest = permitRequest }, cancellationToken);
        }
    }
}