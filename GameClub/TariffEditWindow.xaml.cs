using System;
using System.Globalization;
using System.Windows;

namespace GameClub
{
    public partial class TariffEditWindow : Window
    {
        private readonly Tariff _tariff;
        private readonly bool _isNew;

        public TariffEditWindow(Tariff tariff)
        {
            InitializeComponent();

            if (tariff == null)
            {
                _isNew = true;
                _tariff = new Tariff { IsActive = true };
                TbTitle.Text = "Новый тариф";
            }
            else
            {
                _isNew = false;
                _tariff = tariff;
                TbTitle.Text = "Редактирование тарифа";

                TbName.Text = _tariff.Name;
                TbPrice.Text = _tariff.PricePerHour.ToString("0.00", CultureInfo.InvariantCulture);
                TbDescription.Text = _tariff.Description;
                CbIsActive.IsChecked = _tariff.IsActive;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string name = TbName.Text.Trim();
            string priceText = TbPrice.Text.Trim().Replace(',', '.');
            string description = TbDescription.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Поле «Название» обязательно для заполнения.",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                TbName.Focus();
                return;
            }

            if (!decimal.TryParse(priceText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
            {
                MessageBox.Show("Поле «Цена за час» должно быть числом.",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                TbPrice.Focus();
                return;
            }

            if (price < 0)
            {
                MessageBox.Show("Цена за час не может быть отрицательной.",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                TbPrice.Focus();
                return;
            }

            try
            {
                _tariff.Name = name;
                _tariff.PricePerHour = price;
                _tariff.Description = string.IsNullOrEmpty(description) ? null : description;
                _tariff.IsActive = CbIsActive.IsChecked == true;

                if (_isNew)
                {
                    App.Context.Tariffs.Add(_tariff);
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