using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTG.Funds.Domain.Models
{
    public class NotificationPreference
    {
        public string Type { get; set; } = "Email";
        public string Destination { get; set; } = string.Empty;
    }
}
