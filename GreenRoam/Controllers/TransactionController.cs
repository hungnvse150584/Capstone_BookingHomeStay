using BusinessObject.Model;
using Microsoft.AspNetCore.Authorization;
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

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("GetAllTransactions")]
        public async Task<ActionResult<BaseResponse<IEnumerable<TransactionResponse>>>> GetAllTransactions()
        {
            var transactions = await _transactionsService.GetAllTransactions();
            return Ok(transactions);
        }

        //[Authorize(Roles = "Admin, Owner, Staff, Customer")]
        [HttpGet]
        [Route("GetTransactionByID/{transactionID}")]
        public async Task<ActionResult<BaseResponse<TransactionResponse?>>> GetTransactionById(string transactionID)
        {
            var transaction = await _transactionsService.GetTransactionById(transactionID);
            return Ok(transaction);
        }

        //[Authorize(Roles = "Admin, Owner, Staff")]
        [HttpGet]
        [Route("GetTransactionByHomeStay/{homeStayID}")]
        public async Task<ActionResult<BaseResponse<IEnumerable<TransactionResponse>>>> GetTransactionsByHomeStayId(int homeStayID)
        {
            var transactions = await _transactionsService.GetTransactionsByHomeStayId(homeStayID);
            return Ok(transactions);
        }

        //[Authorize(Roles = "Owner, Staff, Customer")]
        [HttpGet]
        [Route("GetTransactionByAccountID/{accountId}")]
        public async Task<ActionResult<BaseResponse<IEnumerable<TransactionResponse>>>> GetTransactionsByAccountId(string accountId)
        {
            var transactions = await _transactionsService.GetTransactionsByAccountId(accountId);
            return Ok(transactions);
        }
    }
}
