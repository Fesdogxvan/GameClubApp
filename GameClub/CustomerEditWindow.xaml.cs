using System;
using System.Windows;

namespace GameClub
{
    public partial class CustomerEditWindow : Window
    {
        private readonly Customer _customer;
        private readonly bool _isNew;

        public CustomerEditWindow(Customer customer)
        {
            InitializeComponent();

            if (customer == null)
            {
                _isNew = true;
                _customer = new Customer
                {
                    RegistrationDate = DateTime.Now,
                    IsActive = true
                };
                TbTitle.Text = "Новый клиент";
            }
            else
            {
                _isNew = false;
                _customer = customer;
                TbTitle.Text = "Редактирование клиента";

                TbFullName.Text = _customer.FullName;
                TbPhone.Text = _customer.Phone;
                CbIsActive.IsChecked = _customer.IsActive;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            string fullName = TbFullName.Text.Trim();
            string phone = TbPhone.Text.Trim();

            if (string.IsNullOrEmpty(fullName))
            {
                MessageBox.Show("Поле «ФИО» обязательно для заполнения.",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                TbFullName.Focus();
                return;
            }

            if (fullName.Length < 3)
            {
                MessageBox.Show("ФИО должно содержать не менее 3 символов.",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                TbFullName.Focus();
                return;
            }

            try
            {
                _customer.FullName = fullName;
                _customer.Phone = string.IsNullOrEmpty(phone) ? null : phone;
                _customer.IsActive = CbIsActive.IsChecked == true;

                if (_isNew)
                {
                    App.Context.Customers.Add(_customer);
                }

                App.Context.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message,
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}