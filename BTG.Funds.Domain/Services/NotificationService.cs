using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTG.Funds.Domain.Interfaces;
using BTG.Funds.Domain.Models;

namespace BTG.Funds.Domain.Services
{
    public class NotificationService : INotificationService
    {
        public async Task SendAsync(NotificationPreference pref, string message)
        {
            await Task.Delay(200);
            Console.WriteLine($"[{pref.Type}] Enviado a {pref.Destination}: {message}");
        }
    }
}
