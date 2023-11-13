using Application.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class BankAccountController : BaseApiController
    {
        [Authorize]
        [HttpGet("BalanceInquiry")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BalanceInquiry()
        {
            return HandleResult(await Mediator.Send(new BalanceInquiry.Query { }));
        }

        [Authorize]
        [HttpPut("Deposit")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Deposit(int amount)
        {
            return HandleResult(await Mediator.Send(new Deposit.Command { Amount = amount }));
        }

        [Authorize]
        [HttpPut("Withdraw")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Withdraw(int amount)
        {
            return HandleResult(await Mediator.Send(new Withdraw.Command { Amount = amount }));
        }

        [Authorize]
        [HttpPut("TransferCash")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> TransferCash(Guid AccountId, int amount)
        {
            return HandleResult(await Mediator.Send(new TransferCash.Command { ReceiverAccountId = AccountId, Amount = amount }));
        }
    }
}
