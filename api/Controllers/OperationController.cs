using api.Models.DTOs;
using api.Models;
using BankingAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Services;

namespace api.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class OperationController : Controller
    {
        private readonly BankingDbContext _db;
        private readonly AccountServices _accountService;

        public OperationController(BankingDbContext db, AccountServices accountServices)
        {
            _db = db;
            _accountService = accountServices;
        }

        [HttpGet("{id:int}/balance", Name = "GetBalanceById")]
        // GET api/accounts/{id}/balance
        public async Task<ActionResult<string>> GetBalanceByIdAsync(int id)
        {
            // There is no negative id - Client Error
            if (id <= 0)
                return BadRequest("Id can not be negative");
            var account = await _db.Accounts.Where(n => n.Id == id).FirstOrDefaultAsync();
            // If account not found - Client Error
            if (account == null)
                return NotFound($"No account found with Id: {id}");
            // Returned Successfully
            return Ok($"Balance: {account.Balance}");
        }

        [HttpPost("deposit")]
        // POST api/accounts/deposit
        public async Task<ActionResult> Deposit([FromBody] DepositeWithdrawDTO data)
        {
            if (data == null)
                return BadRequest(new { message = "Invalid deposit data" });

            if (data.Amount <= 0)
                return BadRequest(new { message = "Deposit amount must be greater than zero" });

            // Validate account
            var (accountSuccess, accountMessage, account) =
                await _accountService.ValidateAccount(data.AccountNumber);

            if (!accountSuccess)
                return NotFound(new { message = accountMessage });

            // Process transaction
            var (transactionSuccess, transactionMessage, transaction) =
                await _accountService.ProcessTransaction(
                    fromAccount: null,  // No source account for deposit
                    toAccount: account,
                    data.Amount,
                    "Deposit"
                );

            if (!transactionSuccess)
                return StatusCode(500, new { message = transactionMessage });

            return Ok(new
            {
                message = "Deposit successful",
                transactionId = transaction.Id,
                newBalance = account.Balance
            });
        }

        [HttpPost("withdraw")]
        // POST api/accounts/withdraw
        public async Task<ActionResult> Withdraw([FromBody] DepositeWithdrawDTO data)
        {
            if (data == null || data.Amount <= 0)
                return BadRequest(new { message = "Invalid amount for withdrawal" });

            // Validate account
            var (accountSuccess, accountMessage, account) =
                await _accountService.ValidateAccount(data.AccountNumber);

            if (!accountSuccess)
                return NotFound(new { message = accountMessage });

            // Validate withdrawal
            var (withdrawalSuccess, withdrawalMessage, availableAmount) =
                _accountService.ValidateWithdrawal(account, data.Amount);

            if (!withdrawalSuccess)
                return BadRequest(new { message = withdrawalMessage });

            // Process transaction
            var (transactionSuccess, transactionMessage, transaction) =
                await _accountService.ProcessTransaction(account, null, data.Amount, "Withdrawal");

            if (!transactionSuccess)
                return StatusCode(500, new { message = transactionMessage });

            return Ok(new
            {
                message = "Withdrawal successful",
                transactionId = transaction.Id,
                newBalance = account.Balance
            });
        }

        [HttpPost("transfer")]
        // POST api/accounts/transfer
        public async Task<ActionResult> Transfer([FromBody] TransferDTO data)
        {
            if (data == null || data.Amount <= 0)
                return BadRequest(new { message = "Invalid amount for transfer" });

            // Validate from account
            var (fromAccountSuccess, fromAccountMessage, fromAccount) =
                await _accountService.ValidateAccount(data.FromAccountNumber);

            if (!fromAccountSuccess)
                return NotFound(new { message = fromAccountMessage });

            // Validate to account
            var (toAccountSuccess, toAccountMessage, toAccount) =
                await _accountService.ValidateAccount(data.ToAccountNumber);

            if (!toAccountSuccess)
                return NotFound(new { message = toAccountMessage });

            // Validate withdrawal from source account
            var (withdrawalSuccess, withdrawalMessage, _) =
                _accountService.ValidateWithdrawal(fromAccount, data.Amount);

            if (!withdrawalSuccess)
                return BadRequest(new { message = withdrawalMessage });

            // Process transaction
            var (transactionSuccess, transactionMessage, transaction) =
                await _accountService.ProcessTransaction(fromAccount, toAccount, data.Amount, "Transfer");

            if (!transactionSuccess)
                return StatusCode(500, new { message = transactionMessage });

            return Ok(new
            {
                message = "Transfer successful",
                transactionId = transaction.Id,
                fromBalance = fromAccount.Balance,
                toBalance = toAccount.Balance
            });
        }
    } 
}