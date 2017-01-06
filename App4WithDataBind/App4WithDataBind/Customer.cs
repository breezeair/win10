using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App4WithDataBind
{
    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }

        public Customer(string firstName, string lastName, string address)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Address = address;
        }
    }
}
