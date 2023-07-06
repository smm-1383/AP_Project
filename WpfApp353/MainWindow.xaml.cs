using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
    }
}
