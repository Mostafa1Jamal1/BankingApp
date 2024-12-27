using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;
using BankingApp.Models;
using BankingApp.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        [HttpGet(Name = "GetAllAccounts")]
        // GET api/accounts
        public ActionResult<IEnumerable<Account>> GetAllAccounts()
        {
            return Ok(BankRepositry.Accounts);
        }

        [HttpGet("{id:int}", Name = "GetAccountById")]
        // GET api/accounts/{id}
        public ActionResult<Account> GetAccountById(int id)
        {
            // There is no negative id - Client Error
            if (id <= 0)
                return BadRequest("Id can not be negative");
            var account = BankRepositry.Accounts.Where(n => n.Id == id).FirstOrDefault();
            // If account not found - Client Error
            if (account == null)
                return NotFound($"No account found with Id: {id}");
            // Returned Successfully
            return Ok(account);
        }

        [HttpGet("{id:int}/balance", Name = "GetBalanceById")]
        // GET api/accounts/{id}/balance
        public ActionResult<string> GetBalanceById(int id)
        {
            // There is no negative id - Client Error
            if (id <= 0)
                return BadRequest("Id can not be negative");
            var account = BankRepositry.Accounts.Where(n => n.Id == id).FirstOrDefault();
            // If account not found - Client Error
            if (account == null)
                return NotFound($"No account found with Id: {id}");
            // Returned Successfully
            return Ok($"Balance: {account.Balance}");
        }

        [HttpPost("create", Name = "CreateAccount")]
        // POST api/accounts/create
        public ActionResult<Account> CreateAccount([FromBody] AccountDTO data)
        {
            if (data == null)
                return BadRequest("Error: Need to provide account data!");
            // No need for this as long as [APIController] attribute exist
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);
            var ExistAccount = BankRepositry.Accounts.Where(a => a.AccountNumber == data.AccountNumber).FirstOrDefault();
            if (ExistAccount != null) return BadRequest($"Can bot use this Account number: {data.AccountNumber}");

            int NewId = BankRepositry.Accounts.LastOrDefault().Id + 1;
            var NewAccount = new Account
            {
                Id = NewId,
                AccountNumber = data.AccountNumber,
                Balance = data.Balance
            };
            BankRepositry.Accounts.Add(NewAccount);
            return CreatedAtRoute("GetAccountById", new { id = NewAccount.Id }, NewAccount);
        }

        [HttpPut("{id:int}/update", Name = "UpdateAccount")]
        // PUT api/accounts/{id}/update
        public ActionResult UpdateAccount(int id, [FromBody] AccountDTO data)
        {
            if (id <= 0)
                return BadRequest("Id can not be negative");
            if (data == null)
                return BadRequest("No data provided to update");
            var account = BankRepositry.Accounts.Where(n => n.Id == id).FirstOrDefault();
            if (account == null)
                return NotFound($"No account found with Id: {id}");
            account.Balance = data.Balance;
            account.AccountNumber = data.AccountNumber;
            account.Name = data.Name;
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteAccountById")]
        // DELETE api/accounts/{id}
        public ActionResult DeleteAccountById(int id)
        {
            // There is no negative id - Client Error
            if (id <= 0)
                return BadRequest("Id can not be negative");
            var account = BankRepositry.Accounts.Where(n => n.Id == id).FirstOrDefault();
            // If account not found - Client Error
            if (account == null)
                return NotFound($"No account found with Id: {id}");
            // Removed Successfully
            BankRepositry.Accounts.Remove(account);
            return Ok($"Account with id: {id} deleted successfuly");
        }
    }
}
