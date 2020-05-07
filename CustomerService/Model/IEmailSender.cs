using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Model
{
    public interface IEmailSender
    {
        void Send(string toAddress, string subject, string body, bool sendAsync = true);
    }
}
