using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
	public class Employee:IPerson
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int Personal_ID { get; set; }
		public string Email { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
	}
}
