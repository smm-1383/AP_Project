using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
	public class Customer:IPerson
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int ID { get; set; }
		public string Email { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string Phone { get; set; }
		public double Wallet { get; set; }
	}
}
