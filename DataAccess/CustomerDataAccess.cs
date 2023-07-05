using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class CustomerDataAccess
    {
        public static ObservableCollection<Customer> customers = new();
        public CustomerDataAccess()
        {
            ReadCustomers();
        }
        void ReadCustomers()
        {
            var c1 = new Customer()
            {
                FirstName = "Saeed",
                LastName = "Nourian",
                ID = 1,
                Email = "Saeedvft@gmail.com",
                Username = "SeSaNou",
                Password = "ssmkh",
                Phone = "09395523711",
                Wallet = 30000
            };
            var c2 = new Customer()
            {
                FirstName = "Mahziar",
                LastName = "MirAzimi",
                ID = 2,
                Email = "mahziar@gmail.com",
                Username = "SeMahMir",
                Password = "ssmokh",
                Phone = "09302020126",
                Wallet = 50000
            };
            var c3 = new Customer()
            {
                FirstName = "roham",
                LastName = "Izadi",
                ID = 3,
                Email = "doost@gmail.com",
                Username = "RID",
                Password = "icpc",
                Phone = "09210921335"
            };
            var c4 = new Customer()
            {
                FirstName = "Amir",
                LastName = "ferferi",
                ID = 4,
                Email = "amirkhede@gmail.com",
                Username = "amiferfer",
                Password = "mooofeerrr",
                Phone = "09934554123"
            };
            customers.Add(c1);
            customers.Add(c2);
            customers.Add(c3);
            customers.Add(c4);
        }
    }
}