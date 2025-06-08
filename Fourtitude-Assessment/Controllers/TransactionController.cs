using Application.Command;
using Application.Model;
using Application.Request;
using Application.Respone;
using log4net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.Json;

namespace Fourtitude_Assessment.Controllers
{
    [Route("/")]
    public class TransactionController : ControllerBase
    {
        private readonly IMediator _mediator;

        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType);

        public TransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("api/submittrxmessage")]
        public async Task<IActionResult> SubmitTrxMessage([FromBody] TransactionRequest request)
        {

            try
            {
                _logger.Info($"Request : {JsonSerializer.Serialize(request)}");

                var command = new TransactionCommand
                {
                    PartnerKey = request.PartnerKey,
                    PartnerrefNo = request.PartnerrefNo,
                    PartnerPassword = request.PartnerPassword,
                    TotalAmount = request.TotalAmount,
                    TimeStamp = request.TimeStamp,
                    Sig = request.Sig,
                    Items = request.Items?.Select(i => new TransactionItem
                    {
                        PartnerItemRef = i.PartnerItemRef,
                        Name = i.Name,
                        Qty = i.Qty,
                        UnitPrice = i.UnitPrice
                    }).ToList()
                };

                var result = await _mediator.Send(command);
                _logger.Info($"Response : {JsonSerializer.Serialize(result)}");

                return Ok(result);
            }
            catch(Exception ex)
            {
                var result = new TransactionRespone{

                    Result=0,
                    ResultMessage = ex.Message
                    };

                _logger.Info($"Response : {JsonSerializer.Serialize(result)}");
                return BadRequest(result);
            }
        }
    }
}
