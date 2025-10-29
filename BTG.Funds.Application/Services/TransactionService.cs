using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTG.Funds.Domain.Interfaces;
using BTG.Funds.Domain.Models;

namespace BTG.Funds.Application.Services
{
    public class TransactionService
    {
        private readonly IRepository<Transaction> _transactionRepo;

        public TransactionService(IRepository<Transaction> transactionRepo)
        {
            _transactionRepo = transactionRepo;
        }

        public async Task<List<Transaction>> GetAllAsync()
        {
            return await _transactionRepo.GetAllAsync();
        }

        public async Task<Transaction?> GetByIdAsync(string id)
        {
            return await _transactionRepo.GetByIdAsync(id);
        }

        public async Task AddAsync(Transaction transaction)
        {
            await _transactionRepo.AddAsync(transaction);
        }

    }
}
