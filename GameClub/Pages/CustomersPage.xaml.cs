using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GameClub.Pages
{
    public partial class CustomersPage : Page
    {
        private List<Customer> _allCustomers;

        public CustomersPage()
        {
            InitializeComponent();
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            _allCustomers = App.Context.Customers
                .OrderBy(c => c.CustomerID)
                .ToList();

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            string query = TbSearch.Text.Trim().ToLower();

            IEnumerable<Customer> filtered = _allCustomers;

            if (!string.IsNullOrEmpty(query))
            {
                filtered = _allCustomers.Where(c =>
                    (c.FullName ?? "").ToLower().Contains(query) ||
                    (c.Phone ?? "").ToLower().Contains(query));
            }

            DgCustomers.ItemsSource = filtered.ToList();
        }

        private Customer GetSelectedCustomer()
        {
            return DgCustomers.SelectedItem as Customer;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new CustomerEditWindow(null);
            if (editWindow.ShowDialog() == true)
            {
                LoadCustomers();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var customer = GetSelectedCustomer();
            if (customer == null)
            {
                MessageBox.Show("Выберите клиента для редактирования.",
                                "Внимание",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            var editWindow = new CustomerEditWindow(customer);
            if (editWindow.ShowDialog() == true)
            {
                LoadCustomers();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var customer = GetSelectedCustomer();
            if (customer == null)
            {
                MessageBox.Show("Выберите клиента для удаления.",
                                "Внимание",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            int sessionsCount = App.Context.Sessions
                .Count(s => s.CustomerID == customer.CustomerID);

            if (sessionsCount > 0)
            {
                MessageBox.Show(
                    $"Невозможно удалить клиента «{customer.FullName}».\n\n" +
                    $"У клиента {sessionsCount} игровых сессий.\n" +
                    $"Удаление клиента приведёт к потере истории посещений.\n\n" +
                    $"Чтобы скрыть клиента из списка, снимите галочку «Активен» " +
                    $"в окне редактирования.",
                    "Удаление невозможно",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Удалить клиента «{customer.FullName}»?",
                                         "Подтверждение удаления",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                App.Context.Customers.Remove(customer);
                App.Context.SaveChanges();
                LoadCustomers();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка удаления: " + ex.Message,
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadCustomers();
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void DgCustomers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BtnEdit_Click(sender, e);
        }
    }
}