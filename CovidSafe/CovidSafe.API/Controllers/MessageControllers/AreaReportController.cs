﻿using System.Threading;
using System.Threading.Tasks;

using CovidSafe.DAL.Services;
using CovidSafe.Entities.Protos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CovidSafe.API.Controllers.MessageControllers
{
    /// <summary>
    /// Handles requests for infected clients volunteering <see cref="AreaMatch"/> identifiers
    /// </summary>
    [Route("api/Messages/[controller]")]
    [ApiController]
    public class AreaReportController : ControllerBase
    {
        /// <summary>
        /// <see cref="MatchMessage"/> service layer
        /// </summary>
        private IMessageService _messageService;

        /// <summary>
        /// Creates a new <see cref="AreaReportController"/> instance
        /// </summary>
        /// <param name="messageService"><see cref="MatchMessage"/> service layer</param>
        public AreaReportController(IMessageService messageService)
        {
            // Assign local values
            this._messageService = messageService;
        }

        /// <summary>
        /// Publish a <see cref="AreaMatch"/> for distribution among devices
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /Messages/AreaReport
        ///     {
        ///         "areaMatches": [{
        ///             "userMessage": "Monitor symptoms for one week.",
        ///             "areas": [{
        ///                 "location": {
        ///                     "latitude": 74.12345,
        ///                     "longitude": -39.12345
        ///                 },
        ///                 "radiusMeters": 100,
        ///                 "beginTime": 1586083599,
        ///                 "endTime": 1586085189
        ///             }]
        ///         }]
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Query matched Trace results</response>
        /// <response code="400">Malformed or invalid query provided</response>
        /// <param name="request"><see cref="AreaMatch"/> to be stored</param>
        [HttpPut]
        [Consumes("application/x-protobuf", "application/json")]
        [Produces("application/x-protobuf", "application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult> PutAsync(AreaMatch request)
        {
            CancellationToken ct = new CancellationToken();

            // Validate inputs
            if (request == null)
            {
                return BadRequest();
            }

            await this._messageService.PublishAreaAsync(request, ct);

            return Ok();
        }
    }
}