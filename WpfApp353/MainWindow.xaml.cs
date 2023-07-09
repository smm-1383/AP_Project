using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

		private void RegisterOrderbtn_Click(object sender, RoutedEventArgs e)
		{
			bool hasCustomer = Customers.Any(x => x.ID == RegisterOrder_CustomerIdtxt.Text);
			if (hasCustomer)
			{
				FoundCust_Order_reg = Customers.First(x => x.ID == RegisterOrder_CustomerIdtxt.Text);
				EmployeePanelGrid.Visibility = Visibility.Visible;
				EmployeePanel.Visibility = Visibility.Collapsed;
				RegisterOrderPage.Visibility = Visibility.Collapsed;
				AddCustomerPage.Visibility = Visibility.Collapsed;
			}
			else
			{
				MessageBox.Show("Customer not found!\nFirst signup this customer!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
				AddCustomerPage.Visibility = Visibility.Visible;
				RegisterOrderPage.Visibility = Visibility.Collapsed;
				MainPage.Visibility = Visibility.Collapsed;
				SignupPage.Visibility = Visibility.Collapsed;
				EmployeePanel.Visibility = Visibility.Collapsed;
			}
		}

		string StringFromRichTextBox(RichTextBox rtb)
		{
			TextRange textRange = new TextRange(
				rtb.Document.ContentStart,
				rtb.Document.ContentEnd
			);
			return textRange.Text;
		}

		private void CalculateCostbtn_Click(object sender, RoutedEventArgs e)
		{
			Kind kind1;
			Post post1;
			bool IsExp = false;
			Enum.TryParse(PackageContentType.Text, out kind1);
			Enum.TryParse(PostType.Text, out post1);
			if (isExpChckBox.IsChecked == true)
			{
				IsExp = true;
			}

			PackageWeighttxt.BorderBrush = Brushes.Gray;
			SenderAddresstxt.BorderBrush = Brushes.Gray;
			RecieverAddresstxt.BorderBrush = Brushes.Gray;

			if (Check_CalculateCostbtn_Validity() == true)
			{
				Product p1 = new Product()
				{
					sender = FoundCust_Order_reg,
					Saddress = StringFromRichTextBox(SenderAddresstxt),
					Daddress = StringFromRichTextBox(RecieverAddresstxt),
					kind = kind1,
					expensive = IsExp,
					Weight = double.Parse(PackageWeighttxt.Text),
					post = post1,
					phone_number = PhoneNumbertxt.Text,
					status = Status.Registered,
					ID = productDataAccess.GetID(),
				};
				MessageBox.Show($"Post Cost is : {p1.Calc_Price()}", "Post Cost!", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.Yes);
				RegisterProductbtn.IsEnabled = true;
			}
		}

		private bool Check_CalculateCostbtn_Validity()
		{
			bool isValid = true;
			Kind kind1;
			Enum.TryParse(PackageContentType.Text, out kind1);
			Customer sender = FoundCust_Order_reg;
			string Saddress = StringFromRichTextBox(SenderAddresstxt);
			string Daddress = StringFromRichTextBox(RecieverAddresstxt);
			bool IsExp = false;
			if (isExpChckBox.IsChecked == true)
			{
				IsExp = true;
			}
			bool expensive = IsExp;
			string Weight = PackageWeighttxt.Text;
			Post post1;
			Enum.TryParse(PostType.Text, out post1);
			string phone_number = PhoneNumbertxt.Text;

			if (Saddress == "\r\n")
			{
				isValid = false;
				MessageBox.Show("Sender Address is not valid!");
				SenderAddresstxt.BorderBrush = Brushes.Red;
			}
			else if (Daddress == "\r\n")
			{
				isValid = false;
				MessageBox.Show("Reciever Address is not valid!");
				RecieverAddresstxt.BorderBrush = Brushes.Red;
			}
			else if (string.IsNullOrEmpty(Weight) || !double.TryParse(Weight, out double p))
			{
				isValid = false;
				MessageBox.Show("Weight is not valid!");
				PackageWeighttxt.BorderBrush = Brushes.Red;
			}
			return isValid;
		}

		private void RegisterProductbtn_Click(object sender, RoutedEventArgs e)
		{
			double price = Products[Products.Count - 1].Calc_Price();
			Kind kind1;
			Post post1;
			bool IsExp = false;
			Enum.TryParse(PackageContentType.Text, out kind1);
			Enum.TryParse(PostType.Text, out post1);
			if (isExpChckBox.IsChecked == true)
			{
				IsExp = true;
			}
			if (FoundCust_Order_reg.Wallet >= price)
			{
				FoundCust_Order_reg.Wallet -= price;
				Product p1 = new Product()
				{
					sender = FoundCust_Order_reg,
					Saddress = StringFromRichTextBox(SenderAddresstxt),
					Daddress = StringFromRichTextBox(RecieverAddresstxt),
					kind = kind1,
					expensive = IsExp,
					Weight = double.Parse(PackageWeighttxt.Text),
					post = post1,
					phone_number = PhoneNumbertxt.Text,
					status = Status.Registered,
					ID = productDataAccess.GetID(),
				};
				p1.Calc_Price();
				Products.Add(p1);
				MessageBox.Show("Product Registered in System!", "Done!", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.Yes);
				//***Dump();
			}
			else
			{
				MessageBox.Show("Your Wallet cash is not enough!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
			}
		}
		private void OrderIdtxt_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (OrderIdtxt.Text.Length > (ProductDataAccess.products.Any() ? ProductDataAccess.products.Select(x => x.ID).Max().ToString().Length : 1))
			{
				OrderIdtxt.Text = OrderIdtxt.Text.Remove(OrderIdtxt.Text.Length - 1);
			}
			if (!Regex.IsMatch(OrderIdtxt.Text, "^\\d*$"))
			{
				if (OrderIdtxt.Text.Length > 1)
				{
					OrderIdtxt.Text = Regex.Match(OrderIdtxt.Text, "\\d+").Value;
				}
				else
				{
					OrderIdtxt.Text = "";
				}
			}
			OrderIdtxt.SelectionStart = OrderIdtxt.Text.Length;
			OrderIdtxt.SelectionLength = 0;
		}
		private void ShowOrderInfobtn_Click(object sender, RoutedEventArgs e)
		{
			if (Products.Any(x => x.ID == int.Parse(OrderIdtxt.Text) == true))
			{
				Product product = Products.First(x => x.ID == int.Parse(OrderIdtxt.Text));
				if (product.status == Status.Delivered)
				{
					Paktypefilteredcombo_emp.IsEnabled = false;
				}
				else
				{
					Paktypefilteredcombo_emp.IsEnabled = true;
				}

				try
				{
					ProductsGrid.ItemsSource = Products.Where(x => x.ID == int.Parse(OrderIdtxt.Text));

				}
				catch
				{
					MessageBox.Show("Action can't be completed!");
				}
				EmployeePanelGrid.Visibility = Visibility.Visible;
				ShowOrderPage.Visibility = Visibility.Visible;
				GetOrderIDPage.Visibility = Visibility.Collapsed;
				MainPage.Visibility = Visibility.Collapsed;
				SignupPage.Visibility = Visibility.Collapsed;
				EmployeePanel.Visibility = Visibility.Collapsed;
			}
			else
			{
				MessageBox.Show("Order not found!");
			}
		}
	}
}
