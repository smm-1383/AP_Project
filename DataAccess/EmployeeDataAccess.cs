using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataAccess
{
	public class EmployeesDataAccess
	{
		string address = string.Join("\\", Environment.CurrentDirectory.Split("\\").SkipLast(4)) + "\\DataAccess";
		public static ObservableCollection<Employee> employees { get; set; } = new ObservableCollection<Employee>();
		public EmployeesDataAccess()
		{
			try
			{
				ReadEmployees();
			}
			catch { }
		}
		public void ReadEmployees()
		{
			var kk = JsonSerializer.Deserialize<ObservableCollection<Employee>>(File.ReadAllText(address + @"\Employees.json"));
			if (kk == null)
			{
				// error
			}
			else
			{
				employees = kk;
			}
		}
	}
}