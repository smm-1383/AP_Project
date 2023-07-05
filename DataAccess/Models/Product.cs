using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
	public enum Status
	{
		Registered,
		Ready_to_send,
		Sending,
		Delivered
	}
	public enum Kind
	{
		Object,
		Doc,
		Breakable
	}
	public enum Post
	{
		Regular,
		Express
	}
	public class Product
	{
		public int ID { get; set; }
		public Customer sender { get; set; }
		public string Saddress { get; set; }
		public string Daddress { get; set; }
		public Kind kind { get; set; }
		public bool expensive { get; set; } = false;
		public double Weight { get; set; }
		public Post post { get; set; }
		public string phone_number { get; set; }
		public Status status { get; set; }
		double _P { get; set; }
		public double Price { get { return _P; } }
		public string feedback { get; set; } = "";
		public override string ToString()
		{
			return "" + ID + ", " +
				sender.FirstName.Replace(",", "-") + " " + sender.LastName.Replace(",", "-") + ", " +
				Saddress.Replace(",", "-") + ", " +
				Daddress.Replace(",", "-") + ", " +
				kind + ", " +
				(expensive ? "Yes" : "No") + ", " +
				Weight + ", " +
				post + ", " +
				phone_number + ", " +
				status + ", " +
				Price + ", " +
				feedback.Replace(",", "-").Replace("\n", " ");
		}
		public double Calc_Price()
		{
			double p = 10_000;
			if (kind == Kind.Doc)
			{
				p *= 1.5;
			}
			else if (kind == Kind.Breakable)
			{
				p *= 2;
			}
			if (expensive)
			{
				p *= 2;
			}
			if (Weight > 0.5)
			{
				p *= Math.Pow(1.2, Math.Round((Weight - 0.5) * 2));
			}
			if (post == Post.Express)
			{
				p *= 1.5;
			}
			_P = p;
			return p;
		}

	}
}
