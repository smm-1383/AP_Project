using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataAccess;
using DataAccess.Models;

namespace WpfApp353
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow:Window
	{
        Customer found_cust;
        Customer FoundCust_Order_reg;

        EmployeesDataAccess employeesDataAccess = new EmployeesDataAccess();
        CustomerDataAccess customerDataAccess = new CustomerDataAccess();
        ProductDataAccess productDataAccess = new ProductDataAccess();

        ObservableCollection<Employee> Employees = new ObservableCollection<Employee>();
        ObservableCollection<Customer> Customers = new ObservableCollection<Customer>();
        ObservableCollection<Product> Products = new ObservableCollection<Product>();

        public Employee CurrentEmployee { get; set; } = new Employee();

        public MainWindow()
		{
			InitializeComponent();
		}

        private void btnSignup_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Visibility = Visibility.Collapsed;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            bool hasEmployees = Employees.Any(x => x.Username == Usernametxt.Text);
            bool hasCustomer = Customers.Any(x => x.Username == Usernametxt.Text);
            if (hasEmployees)
            {
                Employee found_emp = Employees.First(x => x.Username == Usernametxt.Text);
                if (found_emp.Password == Passwordtxt.Password)
                {
                    MessageBoxResult result = MessageBox.Show("Loged in as Employee", "Login Success!", MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.Yes);
                    MainPage.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Password is not correct!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
                }
            }
            else if (hasCustomer)
            {
                found_cust = Customers.First(x => x.Username == Usernametxt.Text);
                if (found_cust.Password == Passwordtxt.Password)
                {
                    MessageBoxResult result = MessageBox.Show("Loged in as Customer", "Login Success!", MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.Yes);
                    MainPage.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Password is not correct!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
                }
            }
            else
            {
                string messageBoxText = "User not found!";
                string caption = "Error!";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;
                MessageBoxResult result;

                result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
            }
        }

        private void btnSignupEmployee_Click(object sender, RoutedEventArgs e)
        {
            string[] entry = new string[] {
                FirstNametxt.Text,
                LastNametxt.Text,
                PersonalIDtxt.Text,
                Emailtxt.Text,
                SignupUsernametxt.Text,
                SignupPasswordtxt.Password,
                SignupPasswordtxt2.Password,
            };

            if (!Regex.IsMatch(entry[0], "^[a-zA-Z]{3,32}$"))
            {
                MessageBoxResult result = MessageBox.Show("Name Format is not correct!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
            }
            else if (!Regex.IsMatch(entry[1], "^[a-zA-Z]{3,32}$"))
            {
                MessageBoxResult result = MessageBox.Show("Last Name Format is not correct!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
            }
            else if (!Regex.IsMatch(entry[2], "^\\d\\d9\\d\\d$"))
            {
                MessageBoxResult result = MessageBox.Show("Personal ID Format is not correct!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
            }
            else if (!Regex.IsMatch(entry[3], "^\\w{3,32}@\\w{3,32}\\.\\w{2,3}$"))
            {
                MessageBoxResult result = MessageBox.Show("Email Format is not correct!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
            }
            else if (!Regex.IsMatch(entry[5], "^(?=^\\w{8,32}$)(?=\\w*[a-z])(?=\\w*[A-Z])(?=\\w*[0-9])\\w*$"))
            {
                MessageBoxResult result = MessageBox.Show("Password Format is not correct!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
            }
            else if (entry[5] != entry[6])
            {
                MessageBoxResult result = MessageBox.Show("Password doesn't match!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Employee signup done!!", "Signup Success!", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.Yes);
                Employee emp1 = new Employee()
                {
                    FirstName = entry[0],
                    LastName = entry[1],
                    Personal_ID = int.Parse(entry[2]),
                    Email = entry[3],
                    Username = entry[4],
                    Password = entry[5]
                };
                Employees.Add(emp1);
                MainPage.Visibility = Visibility.Visible;
                SignupPage.Visibility = Visibility.Collapsed;
            }
        }

        private void AddCustomerbtn_Click(object sender, RoutedEventArgs e)
        {
            AddCustomer_LastNametxt.Focus();
            AddCustomerPage.Visibility = Visibility.Visible;
            MainPage.Visibility = Visibility.Collapsed;
            SignupPage.Visibility = Visibility.Collapsed;
            EmployeePanel.Visibility = Visibility.Collapsed;
        }
    }
}
