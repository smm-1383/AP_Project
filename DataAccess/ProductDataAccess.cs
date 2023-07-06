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
	public class ProductDataAccess
	{
		string address = string.Join("\\", Environment.CurrentDirectory.Split("\\").SkipLast(4)) + "\\DataAccess";
		public static ObservableCollection<Product> products { get; set; } = new ObservableCollection<Product>();
		public ProductDataAccess()
		{
			try
			{
				ReadProducts();
			}
			catch { }
		}
		void ReadProducts()
		{
			var kk = JsonSerializer.Deserialize<ObservableCollection<Product>>(File.ReadAllText(address + @"\Products.json"));
			if (kk == null)
			{
				// error
			}
			else
			{
				products = kk;
			}
		}
		public int GetID()
		{
			return products.Any() ? products.Max(x => x.ID) + 1 : 1;
		}
	}
}
