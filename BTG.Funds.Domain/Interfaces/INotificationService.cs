using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTG.Funds.Domain.Models;

namespace BTG.Funds.Domain.Interfaces
{
    public interface INotificationService
    {
        Task SendAsync(NotificationPreference pref, string message);
    }
}
