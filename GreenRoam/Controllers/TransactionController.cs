using BusinessObject.Model;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Response.Transactions;

namespace GreenRoam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionsService;
        public TransactionController(ITransactionService transactionsService)
        {
            _transactionsService = transactionsService;
        }

        [HttpGet]
        [Route("GetTransactionByID/{transactionID}")]
        public async Task<ActionResult<BaseResponse<TransactionResponse?>>> GetTransactionById(string transactionID)
        {
            var transaction = await _transactionsService.GetTransactionById(transactionID);
            return Ok(transaction);
        }

        [HttpGet]
        [Route("GetTransactionByHomeStay/{homeStayID}")]
        public async Task<ActionResult<BaseResponse<IEnumerable<TransactionResponse>>>> GetTransactionsByHomeStayId(int homeStayID)
        {
            var transactions = await _transactionsService.GetTransactionsByHomeStayId(homeStayID);
            return Ok(transactions);
        }

        [HttpGet]
        [Route("GetTransactionByAccountID/{accountId}")]
        public async Task<ActionResult<BaseResponse<IEnumerable<TransactionResponse>>>> GetTransactionsByAccountId(string accountId)
        {
            var transactions = await _transactionsService.GetTransactionsByAccountId(accountId);
            return Ok(transactions);
        }
    }
}
