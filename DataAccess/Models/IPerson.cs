using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
	public interface IPerson
	{
		string FirstName { get; set; }
		string LastName { get; set; }
		string Email { get; set; }
		string Username { get; set; }
		string Password { get; set; }
	}
}
