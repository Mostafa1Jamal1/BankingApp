using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;
using api.Models;
using api.Models.DTOs;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/v1/accounts")]
    [ApiController]
    public class AccountsCRUDController : ControllerBase
    {
        private readonly BankingDbContext _db;

        public AccountsCRUDController(BankingDbContext db)
        {
            _db = db;
        }
        [HttpGet(Name = "GetAllAccounts")]
        // GET api/v1/accounts
        public async Task<ActionResult<IEnumerable<AccountDTO>>> GetAllAccountsAsync()
        {
            var accounts = await _db.Accounts.Select(a => new AccountDTO
            {
                AccountNumber = a.AccountNumber,
                AccountType = a.GetType().Name,
                Name = a.Name,
                Balance = a.Balance,
                Currency = a.Currency,
                Status = a.Status,
            }).ToListAsync();
            return Ok(accounts);
        }

        [HttpGet("{id:int}", Name = "GetAccountById")]
        // GET api/v1/accounts/{id}
        public async Task<ActionResult<AccountDTO>> GetAccountByIdAsync(int id)
        {
            // There is no negative id - Client Error
            if (id <= 0)
                return BadRequest("Id can not be negative");
            var account = await _db.Accounts.Where(n => n.Id == id).FirstOrDefaultAsync();
            // If account not found - Client Error
            if (account == null)
                return NotFound($"No account found with Id: {id}");
            // Returned Successfully
            var accountDTO = new AccountDTO
            {
                AccountNumber = account.AccountNumber,
                AccountType = account.GetType().Name,
                Name = account.Name,
                Balance = account.Balance,
                Currency = account.Currency,
                Status = account.Status,
            };
            return Ok(accountDTO);
        }

        [HttpPost(Name = "CreateAccount")]
        // POST api/v1/accounts/
        public async Task<ActionResult> CreateAccountAsync([FromBody] AccountDTO data)
        {
            if (data == null)
                return BadRequest("Error: Need to provide account data!");
            // No need for this as long as [APIController] attribute exist
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);
            var ExistAccount = await _db.Accounts.Where(a => a.AccountNumber == data.AccountNumber).FirstOrDefaultAsync();
            if (ExistAccount != null) return BadRequest($"Can bot use this Account number: {data.AccountNumber}");
            Account NewAccount;
            switch (data.AccountType.ToLower())
            {
                case "savings":
                    NewAccount = new SavingsAccount
                    {
                        Name = data.Name,
                        AccountNumber = data.AccountNumber,
                        Balance = data.Balance,
                        Currency = data.Currency,
                        Status = data.Status,
                        InterestRate = 0.15
                    };
                    break;

                case "checking":
                    NewAccount = new CheckingAccount
                    {
                        Name = data.Name,
                        AccountNumber = data.AccountNumber,
                        Balance = data.Balance,
                        Currency = data.Currency,
                        Status = data.Status,
                        OverdraftLimit = 500m
                    };
                    break;

                default:
                    throw new ArgumentException($"Invalid account type: {data.AccountType}");
            }
            await _db.Accounts.AddAsync(NewAccount);
            await _db.SaveChangesAsync();
            return Ok("Account created Successfuly");
        }

        [HttpPut("{id:int}", Name = "UpdateAccount")]
        // PUT api/v1/accounts/{id}
        public async Task<ActionResult> UpdateAccountAsync(int id, [FromBody] AccountDTO data)
        {
            if (id <= 0)
                return BadRequest("Id can not be negative");
            if (data == null)
                return BadRequest("No data provided to update");
            var account = await _db.Accounts.Where(n => n.Id == id).FirstOrDefaultAsync();
            if (account == null)
                return NotFound($"No account found with Id: {id}");
            account.Balance = data.Balance;
            account.AccountNumber = data.AccountNumber;
            account.Name = data.Name;
            account.Status = data.Status;
            account.Currency = data.Currency;
            account.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteAccountById")]
        // DELETE api/v1/accounts/{id}
        public async Task<ActionResult> DeleteAccountByIdAsync(int id)
        {
            // There is no negative id - Client Error
            if (id <= 0)
                return BadRequest("Id can not be negative");
            var account = await _db.Accounts.Where(n => n.Id == id).FirstOrDefaultAsync();
            // If account not found - Client Error
            if (account == null)
                return NotFound($"No account found with Id: {id}");
            // Removed Successfully
            _db.Accounts.Remove(account);
            await _db.SaveChangesAsync();
            return Ok($"Account with id: {id} deleted successfuly");
        }
    }
}
