using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTG.Funds.Domain.Interfaces;
using BTG.Funds.Domain.Models;

namespace BTG.Funds.Application.Services
{
    public class FundService
    {
        private readonly IRepository<Fund> _fundRepo;
        private readonly IRepository<UserAccount> _userRepo;
        private readonly IRepository<Transaction> _txRepo;
        private readonly INotificationService _notificationService;

        public FundService(IRepository<Fund> fundRepo,
                           IRepository<UserAccount> userRepo,
                           IRepository<Transaction> txRepo,
                           INotificationService notificationService)
        {
            _fundRepo = fundRepo;
            _userRepo = userRepo;
            _txRepo = txRepo;
            _notificationService = notificationService;
        }

        public async Task<List<Fund>> GetFundsAsync() => await _fundRepo.GetAllAsync();

        public async Task<string> SubscribeAsync(string fundId, NotificationPreference pref)
        {
            var fund = await _fundRepo.GetByIdAsync(fundId)
                       ?? throw new Exception("Fondo no encontrado.");

            var user = (await _userRepo.GetAllAsync()).FirstOrDefault() ?? new UserAccount();
            if (user.Balance < fund.MinimumAmount)
                throw new Exception($"No tiene saldo disponible para vincularse al fondo {fund.Name}");

            user.Balance -= fund.MinimumAmount;
            user.SubscribedFunds.Add(fund.Id.ToString());
            await _userRepo.UpdateAsync(user.Id, user);

            await _txRepo.AddAsync(new Transaction
            {
                FundId = fund.Id,
                FundName = fund.Name,
                Type = "Apertura",
                Amount = fund.MinimumAmount
            });

            await _notificationService.SendAsync(pref, $"Suscripción exitosa al fondo {fund.Name}");
            return "Suscripción exitosa.";
        }

        public async Task<string> CancelAsync(string fundId)
        {
            var user = (await _userRepo.GetAllAsync()).FirstOrDefault() ?? throw new Exception("Usuario no encontrado.");
            var fund = await _fundRepo.GetByIdAsync(fundId) ?? throw new Exception("Fondo no encontrado.");

            if (!user.SubscribedFunds.Contains(fundId))
                throw new Exception($"No está vinculado al fondo {fund.Name}");

            user.SubscribedFunds.Remove(fundId);
            user.Balance += fund.MinimumAmount;
            await _userRepo.UpdateAsync(user.Id, user);

            await _txRepo.AddAsync(new Transaction
            {
                FundId = fundId,
                FundName = fund.Name,
                Type = "Cancelación",
                Amount = fund.MinimumAmount
            });

            return "Cancelación exitosa.";
        }

        public async Task<List<Transaction>> GetHistoryAsync() => await _txRepo.GetAllAsync();
    }
}
