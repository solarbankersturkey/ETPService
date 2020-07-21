using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Dto {
    public class RegisterModel {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
