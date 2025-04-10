using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Response.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface ITransactionService
    {
        Task<BaseResponse<TransactionResponse?>> GetTransactionById(string transactionID);
        Task<BaseResponse<IEnumerable<TransactionResponse>>> GetTransactionsByAccountId(string accountId);
        Task<BaseResponse<IEnumerable<TransactionResponse>>> GetTransactionsByHomeStayId(int homeStayID);
    }
}
