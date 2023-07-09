using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using System.Xml.Linq;
using DataAccess;
using DataAccess.Models;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Windows.Media.Media3D;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.IO;
using Org.BouncyCastle.Utilities.Collections;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Net.Mail;
using System.Text.Json;
using System.Net;
using System.Text.Json.Serialization;
using System.DirectoryServices.ActiveDirectory;

namespace WpfApp343
{
	public partial class MainWindow:Window
	{
		string address = string.Join("\\", Environment.CurrentDirectory.Split("\\").SkipLast(4)) + @"\DataAccess";

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
			fillData();
			EmployeesGrid.ItemsSource = Products;
			Usernametxt.Focus();
		}
		private void fillData()
		{
			Employees = EmployeesDataAccess.employees;
			Customers = CustomerDataAccess.customers;
			Products = ProductDataAccess.products;
			foreach (var x in Products)
			{
				x.sender = Customers.Where(C => C.ID == x.sender.ID && C.Username == x.sender.Username).First();
				x.Calc_Price();
			}
			Dump();
		}
		private void Dump()
		{
			// customers, products, employees
			using (var w = File.CreateText(address + @"\Customers.json"))
			{
				var options = new JsonSerializerOptions { WriteIndented = true };
				w.WriteLine(JsonSerializer.Serialize(Customers, options));
			}
			using (var w = File.CreateText(address + @"\Products.json"))
			{
				var options = new JsonSerializerOptions { WriteIndented = true };
				w.WriteLine(JsonSerializer.Serialize(Products, options));
			}
			using (var w = File.CreateText(address + @"\Employees.json"))
			{
				var options = new JsonSerializerOptions { WriteIndented = true };
				w.WriteLine(JsonSerializer.Serialize(Employees, options));
			}
		}

		private void btnSignup_Click(object sender, RoutedEventArgs e)
		{
			SignupPage.Visibility = Visibility.Visible;
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
					EmployeePanelGrid.Visibility = Visibility.Visible;
					EmployeePanel.Visibility = Visibility.Visible;
					MainPage.Visibility = Visibility.Collapsed;
					SignupPage.Visibility = Visibility.Collapsed;
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
					CustomerPanelGrid.Visibility = Visibility.Visible;
					CustomerPanel.Visibility = Visibility.Visible;
					EmployeePanel.Visibility = Visibility.Collapsed;
					MainPage.Visibility = Visibility.Collapsed;
					SignupPage.Visibility = Visibility.Collapsed;
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

		private void SubmitPckgStatusbtn_Click(object sender, RoutedEventArgs e)
		{
			Product product = Products.First(x => x.ID == int.Parse(OrderIdtxt.Text));
			switch (Paktypefilteredcombo_emp.Text)
			{
				case "Registered":
					product.status = Status.Registered;
					MessageBox.Show("Submited");
					break;
				case "Ready To Send":
					product.status = Status.Ready_to_send;
					MessageBox.Show("Submited");
					break;
				case "Sending":
					product.status = Status.Sending;
					MessageBox.Show("Submited");
					break;
				case "Delivered":
					product.status = Status.Delivered;
					Paktypefilteredcombo_emp.IsEnabled = false;

					try
					{
						var from = "SaeedMahziar@gmail.com";
						var frompass = "iapuanrgjophwizf";
						var Message = new MailMessage();
						Message.From = new MailAddress(from);
						Message.Subject = "Subj";
						Message.To.Add(new MailAddress(product.sender.Email));
						Message.Body = $"<html><Body> Hi!<br>Your package with id: ( {product.ID} ) Delivered!.<br>Sender: {product.sender.FirstName} {product.sender.LastName} <br>You can submit your feedback! </body><html>";
						Message.IsBodyHtml = true;
						var smtpcl = new SmtpClient("smtp.gmail.com")
						{
							Port = 587,
							Credentials = new NetworkCredential(from, frompass),
							EnableSsl = true,
						};
						smtpcl.Send(Message);
					}
					catch
					{
						MessageBox.Show("The email didn't send successfully.\nif you're sure about your email's correction, check internet and try again later.", "Signup Failed!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
					}

					MessageBox.Show("Submited");
					break;
				default:
					MessageBox.Show("Please select an item!");
					break;
			}
			Dump();
		}

		private void ShowOrderbtn_Click(object sender, RoutedEventArgs e)
		{
			EmployeePanelGrid.Visibility = Visibility.Visible;
			GetOrderIDPage.Visibility = Visibility.Visible;
			ShowOrderPage.Visibility = Visibility.Collapsed;
			MainPage.Visibility = Visibility.Collapsed;
			SignupPage.Visibility = Visibility.Collapsed;
			EmployeePanel.Visibility = Visibility.Collapsed;
		}

		private void OrderReportbtn_Click(object sender, RoutedEventArgs e)
		{
			EmployeePanelGrid.Visibility = Visibility.Visible;
			OrderReportPage.Visibility = Visibility.Visible;
			GetOrderIDPage.Visibility = Visibility.Collapsed;
			MainPage.Visibility = Visibility.Collapsed;
			SignupPage.Visibility = Visibility.Collapsed;
			EmployeePanel.Visibility = Visibility.Collapsed;
		}

		private void EmployeesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}
		private void AddCustomerbtn_Click(object sender, RoutedEventArgs e)
		{
			AddCustomer_LastNametxt.Focus();
			AddCustomerPage.Visibility = Visibility.Visible;
			CustomerPanelGrid.Visibility = Visibility.Collapsed;
			WalletPage.Visibility = Visibility.Collapsed;
			MainPage.Visibility = Visibility.Collapsed;
			GetOrderIDPage.Visibility = Visibility.Collapsed;
			SignupPage.Visibility = Visibility.Collapsed;
			EmployeePanel.Visibility = Visibility.Collapsed;
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
				GetOrderIDPage.Visibility = Visibility.Collapsed;
				SignupPage.Visibility = Visibility.Collapsed;
				EmployeePanel.Visibility = Visibility.Collapsed;
				Dump();
			}
		}

		private void btnSignupCustomer_Click(object sender, RoutedEventArgs e)
		{
			string[] entry = new string[] {
				AddCustomer_FirstNametxt.Text,
				AddCustomer_LastNametxt.Text,
				AddCustomer_IDtxt.Text,
				AddCustomer_Emailtxt.Text,
				AddCustomer_Phonetxt.Text,
			};

			if (!Regex.IsMatch(entry[0], "^[a-zA-Z]{3,32}$"))
			{
				MessageBoxResult result = MessageBox.Show("Name Format is not correct!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
			}
			else if (!Regex.IsMatch(entry[1], "^[a-zA-Z]{3,32}$"))
			{
				MessageBoxResult result = MessageBox.Show("Last Name Format is not correct!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
			}
			else if (!Regex.IsMatch(entry[2], @"^[0]{2}\d{8}"))
			{
				MessageBoxResult result = MessageBox.Show("ID Format is not correct!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
			}
			else if (!Regex.IsMatch(entry[3], "^\\w{3,32}@\\w{3,32}\\.\\w{2,3}$"))
			{
				MessageBoxResult result = MessageBox.Show("Email Format is not correct!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
			}
			else if (!Regex.IsMatch(entry[4], @"^[0]{1}[9]{1}\d{9}"))
			{
				MessageBoxResult result = MessageBox.Show("Phone Number Format is not correct!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
			}
			else
			{
				Random r = new Random();
				int f = r.Next(10_000);
				while (Customers.Select(x => x.Username).Where(x => x == ("user" + f)).Any())
				{
					f = r.Next(10_000);
				}
				var rand_u = "user" + f;
				var rand_p = r.Next(100_000_000).ToString();
				Customer cust1 = new Customer()
				{
					FirstName = entry[0],
					LastName = entry[1],
					ID = entry[2],
					Email = entry[3],
					Phone = entry[4],
					Username = rand_u,
					Password = rand_p
				};
				try
				{
					Email_Sender(entry[3], rand_u, rand_p);
				}
				catch
				{
					MessageBox.Show("The email didn't send successfully.\nif you're sure about your email's correction, check internet and try again later.", "Signup Failed!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
					return;
				}

				MessageBoxResult result = MessageBox.Show("Customer signup done!\nyou can find your user, pass in email\nCheck spam folder.", "Signup Success!", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.Yes);
				Customers.Add(cust1);
				Dump();


				EmployeePanelGrid.Visibility = Visibility.Visible;
				EmployeePanel.Visibility = Visibility.Visible;
				CustomerPanelGrid.Visibility = Visibility.Collapsed;
				WalletPage.Visibility = Visibility.Collapsed;
				MainPage.Visibility = Visibility.Collapsed;
				GetOrderIDPage.Visibility = Visibility.Collapsed;
				SignupPage.Visibility = Visibility.Collapsed;
				AddCustomerPage.Visibility = Visibility.Collapsed;
			}
		}

		private static bool LUHN(string Card)
		{
			int s = 0, f;
			for (int i = Card.Length - 1; i >= 0; i--)
			{
				f = int.Parse(Card[i].ToString()) * (i % 2 + 1);
				if (f > 9)
					f -= 9;
				s += f;
			}
			if (s % 10 != 0)
				return false;
			return true;
		}
		private static bool Date(string Y, string M)
		{
			try
			{
				if (int.Parse(M) > 12 || int.Parse(M) < 0 || int.Parse(Y) <= 2023)
				{
					return false;
				}
			}
			catch
			{
				return false;
			}
			return true;
		}

		private void RegisterOrderbtn_Click(object sender, RoutedEventArgs e)
		{
			bool hasCustomer = Customers.Any(x => x.ID == RegisterOrder_CustomerIdtxt.Text);
			if (hasCustomer)
			{
				FoundCust_Order_reg = Customers.First(x => x.ID == RegisterOrder_CustomerIdtxt.Text);
				ScrollBar.Visibility = Visibility.Visible;
				EmployeePanelGrid.Visibility = Visibility.Visible;
				RegisterOrderPanel.Visibility = Visibility.Visible;
				EmployeePanel.Visibility = Visibility.Collapsed;
				RegisterOrderPage.Visibility = Visibility.Collapsed;
				AddCustomerPage.Visibility = Visibility.Collapsed;
			}
			else
			{
				MessageBox.Show("Customer not found!\nFirst signup this customer!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
				AddCustomerPage.Visibility = Visibility.Visible;
				ScrollBar.Visibility = Visibility.Collapsed;
				RegisterOrderPage.Visibility = Visibility.Collapsed;
				CustomerPanelGrid.Visibility = Visibility.Collapsed;
				WalletPage.Visibility = Visibility.Collapsed;
				MainPage.Visibility = Visibility.Collapsed;
				GetOrderIDPage.Visibility = Visibility.Collapsed;
				SignupPage.Visibility = Visibility.Collapsed;
				EmployeePanel.Visibility = Visibility.Collapsed;
			}
		}

		private void Chargebtn_Click(object sender, RoutedEventArgs e)
		{
			if (Regex.IsMatch(Cardtxt.Text, "^\\d{10,17}$") && !LUHN(Cardtxt.Text))
			{
				MessageBox.Show("CardNumber Format is not correct!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
				return;
			}
			else if (!Date(Yeartxt.Text, Monthtxt.Text))
			{
				MessageBoxResult result = MessageBox.Show("Expiration Date Format is not correct!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
				return;
			}
			else if (!Regex.IsMatch(CVVtxt.Text, "^\\d\\d\\d\\d?$"))
			{
				MessageBoxResult result = MessageBox.Show("CVV Format is not correct!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
				return;
			}
			else if (!Regex.IsMatch(Chargetxt.Text, "^\\d+\\.?\\d*$"))
			{
				MessageBoxResult result = MessageBox.Show("Charge Amount Format is not correct!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
				return;
			}
			found_cust.Wallet += double.Parse(Chargetxt.Text);
			CurrentAmountlbl.Content = found_cust.Wallet.ToString();
			Dump();
			MessageBox.Show("Your Charge updated successfully.", "Done!", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.Yes);
			MessageBoxResult bill_result = MessageBox.Show("Do you want your Bill in PDF?", "Bill?!", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.Yes);
			switch (bill_result)
			{
				case MessageBoxResult.Yes:
					addPDF(found_cust, double.Parse(Chargetxt.Text));
					ShowPDF(found_cust);
					break;
				case MessageBoxResult.No:
					break;
			}
		}

		private void ShowPDF(Customer customer)
		{
			PDFWindow pdfwindow = new PDFWindow();
			pdfwindow.pdfWebViewer.Navigate(new Uri("about:blank"));
			pdfwindow.pdfWebViewer.Navigate(address + $"\\{customer.FirstName}_{customer.LastName}_bill.PDF");
			pdfwindow.ShowDialog();
		}

		private void OrderInformationbtn_Click(object sender, RoutedEventArgs e)
		{

		}

		private void Cardtxt_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!Regex.IsMatch(Cardtxt.Text, "^\\d*$"))
			{
				if (Cardtxt.Text.Length > 1)
				{
					Cardtxt.Text = Regex.Match(Cardtxt.Text, "\\d+").Value;
				}
				else
				{
					Cardtxt.Text = "";
				}
			}
			Cardtxt.SelectionStart = Cardtxt.Text.Length;
			Cardtxt.SelectionLength = 0;
		}

		private void Yeartxt_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (Yeartxt.Text.Length > 4)
			{
				Yeartxt.Text = Yeartxt.Text.Remove(Yeartxt.Text.Length - 1);
			}
			if (!Regex.IsMatch(Yeartxt.Text, "^\\d*$"))
			{
				if (Yeartxt.Text.Length > 1)
				{
					Yeartxt.Text = Regex.Match(Yeartxt.Text, "\\d+").Value;
				}
				else
				{
					Yeartxt.Text = "";
				}
			}
			Yeartxt.SelectionStart = Yeartxt.Text.Length;
			Yeartxt.SelectionLength = 0;
		}

		private void Monthtxt_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (Monthtxt.Text.Length > 2)
			{
				Monthtxt.Text = Monthtxt.Text.Remove(Monthtxt.Text.Length - 1);
			}
			if (!Regex.IsMatch(Monthtxt.Text, "^\\d*$"))
			{
				if (Monthtxt.Text.Length > 1)
				{
					Monthtxt.Text = Regex.Match(Monthtxt.Text, "\\d+").Value;
				}
				else
				{
					Monthtxt.Text = "";
				}
			}
			Monthtxt.SelectionStart = Monthtxt.Text.Length;
			Monthtxt.SelectionLength = 0;
		}

		private void CVVtxt_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (CVVtxt.Text.Length > 4)
			{
				CVVtxt.Text = CVVtxt.Text.Remove(CVVtxt.Text.Length - 1);
			}
			if (!Regex.IsMatch(CVVtxt.Text, "^\\d*$"))
			{
				if (CVVtxt.Text.Length > 1)
				{
					CVVtxt.Text = Regex.Match(CVVtxt.Text, "\\d+").Value;
				}
				else
				{
					CVVtxt.Text = "";
				}
			}
			CVVtxt.SelectionStart = CVVtxt.Text.Length;
			CVVtxt.SelectionLength = 0;
		}

		private void Chargetxt_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!Regex.IsMatch(Chargetxt.Text, "^\\d*\\.?\\d*$"))
			{
				if (Chargetxt.Text.Length > 1)
				{
					Chargetxt.Text = Regex.Match(Chargetxt.Text, "\\d+\\.?\\d+").Value;
				}
				else
				{
					Chargetxt.Text = "";
				}
			}
			Chargetxt.SelectionStart = Chargetxt.Text.Length;
			Chargetxt.SelectionLength = 0;
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

		private void Search_Filteredbtn_Click(object sender, RoutedEventArgs e)
		{
			if (found_cust == null && CurrentEmployee != null)
			{
				var f = Products.Where(x => true);
			}
			else if (found_cust != null)
			{
				var f = Products.Where(x => x.sender == found_cust);
			}
			Status s;
			Post p;

			if (UserIdfilteredtxt.Text != "")
			{
				f = f.Where(x => x.ID == int.Parse(UserIdfilteredtxt.Text));
			}
			if (WeightFilteredtxt.Text != "")
			{
				f = f.Where(x => x.Weight <= double.Parse(WeightFilteredtxt.Text) + 1 && x.Weight >= double.Parse(WeightFilteredtxt.Text) - 1);
			}
			if (Pricefilteredtxt.Text != "")
			{
				f = f.Where(x => x.Price <= double.Parse(Pricefilteredtxt.Text) + 10000 && x.Price >= double.Parse(Pricefilteredtxt.Text) - 10000);
			}
			if (Enum.TryParse(Paktypefilteredcombo.Text, out s))
			{
				f = f.Where(x => x.status == s);
			}
			if (Enum.TryParse(postypefilteredcombo.Text, out p))
			{
				f = f.Where(x => x.post == p);
			}
			string t = "ID, SenderFullName, SAddress, DAddress, kind, IsExpensive?, Weight, Post, Phone, Status, Price, feedback\n";
			foreach (var pro in f)
			{
				t += pro.ToString().Replace("\n", " ").Replace("\r", " ") + "\n";
			}
			File.WriteAllText(address + "\\Filtered_Search.csv", t);
			MessageBox.Show("Sucess", "The filtered search result is in csv file.", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.Yes);
		}

		private void OrderRegisterbtn_Click(object sender, RoutedEventArgs e)
		{
			RegisterOrder_CustomerIdtxt.Focus();
			RegisterOrderPage.Visibility = Visibility.Visible;
			ScrollBar.Visibility = Visibility.Collapsed;
			CustomerPanelGrid.Visibility = Visibility.Collapsed;
			WalletPage.Visibility = Visibility.Collapsed;
			MainPage.Visibility = Visibility.Collapsed;
			GetOrderIDPage.Visibility = Visibility.Collapsed;
			SignupPage.Visibility = Visibility.Collapsed;
			EmployeePanel.Visibility = Visibility.Collapsed;
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
				Dump();
			}
			else
			{
				MessageBox.Show("Your Wallet cash is not enough!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
			}
		}

		private void Email_Sender(string ToMail, string username, string password)
		{
			var from = "SaeedMahziar@gmail.com";
			var frompass = "iapuanrgjophwizf";
			var Message = new MailMessage();
			Message.From = new MailAddress(from);
			Message.Subject = "Subj";
			Message.To.Add(new MailAddress(ToMail));
			Message.Body = $"<html><Body> Hi!<br>This is your generated username and password:<strong>user:{username} | pass:{password}</strong>.<br>Use it to Log in. </body><html>";
			Message.IsBodyHtml = true;
			var smtpcl = new SmtpClient("smtp.gmail.com")
			{
				Port = 587,
				Credentials = new NetworkCredential(from, frompass),
				EnableSsl = true,
			};
			smtpcl.Send(Message);
		}

		private void addPDF(Customer customer, double charge_amount)
		{
			Document doctum = new Document(iTextSharp.text.PageSize.A6, 10, 10, 42, 35);
			PdfWriter writer = PdfWriter.GetInstance(doctum, new FileStream(address + $@"\{customer.FirstName}_{customer.LastName}_bill.PDF", FileMode.Create));
			doctum.Open();
			doctum.SetMargins(10, 10, 10, 10);
			iTextSharp.text.Paragraph paragraph2 = new iTextSharp.text.Paragraph($"Wallet increase bill\n\nUser ( {customer.FirstName} {customer.LastName} )\n{DateTime.Now}\nYou charged your account for {charge_amount} $\n" +
				$"Your current wallet is : {customer.Wallet}");
			doctum.Add(paragraph2);
			doctum.Close();
		}

		private void EmployeeMainPanel_Click(object sender, RoutedEventArgs e)
		{
			EmployeePanelGrid.Visibility = Visibility.Visible;
			EmployeePanel.Visibility = Visibility.Visible;
			GetOrderIDPage.Visibility = Visibility.Collapsed;
			ShowOrderPage.Visibility = Visibility.Collapsed;
			OrderReportPage.Visibility = Visibility.Collapsed;
			RegisterOrderPanel.Visibility = Visibility.Collapsed;
			GetOrderIDPage.Visibility = Visibility.Collapsed;
			MainPage.Visibility = Visibility.Collapsed;
			SignupPage.Visibility = Visibility.Collapsed;
			RegisterOrderPage.Visibility = Visibility.Collapsed;
			AddCustomerPage.Visibility = Visibility.Collapsed;
			ScrollBar.Visibility = Visibility.Collapsed;
		}

		private void CustomerMainPanel_Click(object sender, RoutedEventArgs e)
		{
			CustomerPanelGrid.Visibility = Visibility.Visible;
			CustomerPanel.Visibility = Visibility.Visible;
			OrderReportPage.Visibility = Visibility.Collapsed;
			GetOrderIDPage.Visibility = Visibility.Collapsed;
			MainPage.Visibility = Visibility.Collapsed;
			SignupPage.Visibility = Visibility.Collapsed;
			RegisterOrderPage.Visibility = Visibility.Collapsed;
			AddCustomerPage.Visibility = Visibility.Collapsed;
			WalletPage.Visibility = Visibility.Collapsed;
			RegisterOrderPanel.Visibility = Visibility.Collapsed;
			ChgPanel.Visibility = Visibility.Collapsed;
		}
		private void OrderReportbtn_cust_Click(object sender, RoutedEventArgs e)
		{
			CustomerPanelGrid.Visibility = Visibility.Visible;
			OrderReportPage_cust.Visibility = Visibility.Visible;
			WalletPage.Visibility = Visibility.Collapsed;
			MainPage.Visibility = Visibility.Collapsed;
			GetOrderIDPage.Visibility = Visibility.Collapsed;
			SignupPage.Visibility = Visibility.Collapsed;
			EmployeePanel.Visibility = Visibility.Collapsed;
			CustomerPanel.Visibility = Visibility.Collapsed;
		}

		private void Walletbtn_Click(object sender, RoutedEventArgs e)
		{
			CustomerPanelGrid.Visibility = Visibility.Visible;
			WalletPage.Visibility = Visibility.Visible;
			MainPage.Visibility = Visibility.Collapsed;
			GetOrderIDPage.Visibility = Visibility.Collapsed;
			SignupPage.Visibility = Visibility.Collapsed;
			EmployeePanel.Visibility = Visibility.Collapsed;
			CustomerPanel.Visibility = Visibility.Collapsed;
			CurrentAmountlbl.Content = found_cust.Wallet.ToString();
		}

		private void CustomerMaiPanel_Click(object sender, RoutedEventArgs e)
		{
			CustomerPanel.Visibility = Visibility.Visible;
			feedbackGrid.Visibility = Visibility.Collapsed;
			GetOrderIDPage_cust.Visibility = Visibility.Collapsed;
			ShowOrderPage_cust.Visibility = Visibility.Collapsed;
			OrderReportPage_cust.Visibility = Visibility.Collapsed;
			WalletPage.Visibility = Visibility.Collapsed;
			MainPage.Visibility = Visibility.Collapsed;
			GetOrderIDPage.Visibility = Visibility.Collapsed;
			SignupPage.Visibility = Visibility.Collapsed;
			EmployeePanel.Visibility = Visibility.Collapsed;
		}

		private void ShowOrderbtn_cust_Click(object sender, RoutedEventArgs e)
		{
			GetOrderIDPage_cust.Visibility = Visibility.Visible;
			ShowOrderPage_cust.Visibility = Visibility.Collapsed;
			GetOrderIDPage.Visibility = Visibility.Collapsed;
			CustomerPanel.Visibility = Visibility.Collapsed;
			MainPage.Visibility = Visibility.Collapsed;
			SignupPage.Visibility = Visibility.Collapsed;
			EmployeePanel.Visibility = Visibility.Collapsed;
		}

		private void ShowOrderInfobtn_cust_Click(object sender, RoutedEventArgs e)
		{
			if (Products.Where(x => x.sender == found_cust && x.ID == int.Parse(OrderIdtxt_cust.Text)).Count() != 0)
			{
				SingleProductGrid_cust.ItemsSource = Products.Where(x => x.sender == found_cust).Where(x => x.ID == int.Parse(OrderIdtxt_cust.Text));
				//###### sender name in Grid has error ######//
				Product product = Products.First(x => x.ID == int.Parse(OrderIdtxt_cust.Text));
				if (product.status == Status.Delivered)
				{
					feedbackGrid.Visibility = Visibility.Visible;
				}
				ShowOrderPage_cust.Visibility = Visibility.Visible;
				GetOrderIDPage.Visibility = Visibility.Collapsed;
				CustomerPanel.Visibility = Visibility.Collapsed;
				MainPage.Visibility = Visibility.Collapsed;
				SignupPage.Visibility = Visibility.Collapsed;
				EmployeePanel.Visibility = Visibility.Collapsed;
			}
			else
			{
				MessageBox.Show("Order not found in your feed!");
			}
		}
		private void chngbtn_Click(object sender, RoutedEventArgs e)
		{
			ChgPanel.Visibility = Visibility.Visible;
			GetOrderIDPage.Visibility = Visibility.Collapsed;
			CustomerPanel.Visibility = Visibility.Collapsed;
			MainPage.Visibility = Visibility.Collapsed;
			SignupPage.Visibility = Visibility.Collapsed;
			EmployeePanel.Visibility = Visibility.Collapsed;
			ShowOrderPage_cust.Visibility = Visibility.Collapsed;
		}

		private void Submitfeedbackbtn_Click(object sender, RoutedEventArgs e)
		{
			Product product = Products.First(x => x.ID == int.Parse(OrderIdtxt_cust.Text));
			product.feedback = feedbacktxt.Text;
			MessageBox.Show("Feedback Submited!");
		}

		private void LoginPagebtn_cust_Click(object sender, RoutedEventArgs e)
		{
			MainPage.Visibility = Visibility.Visible;
			CustomerPanel.Visibility = Visibility.Collapsed;
			EmployeePanel.Visibility = Visibility.Collapsed;
			EmployeePanelGrid.Visibility = Visibility.Collapsed;
			CustomerPanelGrid.Visibility = Visibility.Collapsed;
			FilteredItemsPanel.Visibility = Visibility.Collapsed;
			GetOrderIDPage.Visibility = Visibility.Collapsed;
			ShowOrderPage.Visibility = Visibility.Collapsed;
			OrderReportPage.Visibility = Visibility.Collapsed;
			RegisterOrderPanel.Visibility = Visibility.Collapsed;
			SignupPage.Visibility = Visibility.Collapsed;
			AddCustomerPage.Visibility = Visibility.Collapsed;
			ScrollBar.Visibility = Visibility.Collapsed;
			RegisterOrderPage.Visibility = Visibility.Collapsed;
			WalletPage.Visibility = Visibility.Collapsed;
			ChgPanel.Visibility = Visibility.Collapsed;
			CurrentEmployee = null;
			found_cust = null;
		}

		private void chngupbtn_Click(object sender, RoutedEventArgs e)
		{
			string u = NewUsernametxt.Text, p1 = NewPasstxt.Text, p2 = RepPasstxt.Text;
			if (Customers.Select(x => x.Username).Where(x => x == u).Count() > 1)
			{
				MessageBoxResult result = MessageBox.Show("This username is already taken!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
				return;
			}
			else if (Customers.Select(x => x.Username).Where(x => x == u).Count() == 1 && (found_cust.Username != u))
			{
				MessageBoxResult result = MessageBox.Show("This username is already taken!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
				return;
			}
			if (p1 != p2)
			{
				MessageBoxResult result = MessageBox.Show("Passwords doesn't match!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
				return;
			}
			if (!Regex.IsMatch(p1, "^(?=^\\w{8,32}$)(?=\\w*[a-z])(?=\\w*[A-Z])(?=\\w*[0-9])\\w*$"))
			{
				MessageBoxResult result = MessageBox.Show("Password doesn't match with policy!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
				return;
			}
			found_cust.Username = u;
			found_cust.Password = p1;
			Dump();
			MessageBox.Show("Your changed.", "Success!");
			CustomerPanelGrid.Visibility = Visibility.Visible;
			CustomerPanel.Visibility = Visibility.Visible;
			OrderReportPage.Visibility = Visibility.Collapsed;
			GetOrderIDPage.Visibility = Visibility.Collapsed;
			MainPage.Visibility = Visibility.Collapsed;
			SignupPage.Visibility = Visibility.Collapsed;
			RegisterOrderPage.Visibility = Visibility.Collapsed;
			AddCustomerPage.Visibility = Visibility.Collapsed;
			WalletPage.Visibility = Visibility.Collapsed;
			RegisterOrderPanel.Visibility = Visibility.Collapsed;
			ChgPanel.Visibility = Visibility.Collapsed;
		}
	}
}


//### Extra generated files like csv and pdf should be in another path
//### all text boxes should reset after work