using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Model
{
    public class Detail
    {
        public Address Address { get; set; }
        public string Phone_No { get; set; }
        public string Identity_No { get; set; }
        public string Invoice_No { get; set; }
        public DateTime Birth_Date { get; set; }
        public string Short_Location { get; set; }
        public string Language { get; set; }
    }
}
