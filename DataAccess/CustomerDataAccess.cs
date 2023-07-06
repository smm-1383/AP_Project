using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
	public class CustomerDataAccess
	{
		string address = string.Join("\\", Environment.CurrentDirectory.Split("\\").SkipLast(4)) + "\\DataAccess";
		public static ObservableCollection<Customer> customers = new ObservableCollection<Customer>();
		public CustomerDataAccess()
		{
			try
			{
				ReadCustomers();
			}
			catch { }
		}
		void ReadCustomers()
		{

			var kk = JsonSerializer.Deserialize<ObservableCollection<Customer>>(File.ReadAllText(address + @"\Customers.json"));
			if (kk == null)
			{
				// error
			}
			else
			{
				customers = kk;
			}
		}
	}
}
