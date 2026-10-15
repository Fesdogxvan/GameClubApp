using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace GameClub
{
    public partial class ComponentEditWindow : Window
    {
        private readonly Component _component;
        private readonly bool _isNew;

        public ComponentEditWindow(Component component)
        {
            InitializeComponent();

            var types = App.Context.ComponentTypes
                .OrderBy(t => t.TypeID)
                .ToList();

            CbType.ItemsSource = types;

            if (component == null)
            {
                _isNew = true;
                _component = new Component
                {
                    IsAvailable = true,
                    PerformanceLevel = 5,
                    Price = 0
                };
                TbTitle.Text = "Новое комплектующее";

                if (types.Count > 0)
                    CbType.SelectedIndex = 0;

                TbLevel.Text = "5";
                TbPrice.Text = "0.00";
            }
            else
            {
                _isNew = false;
                _component = component;
                TbTitle.Text = "Редактирование комплектующего";

                CbType.SelectedItem = types.FirstOrDefault(t => t.TypeID == _component.TypeID);
                TbName.Text = _component.Name;
                TbInventory.Text = _component.InventoryNumber;
                TbLevel.Text = _component.PerformanceLevel.ToString();
                TbPrice.Text = _component.Price.ToString("0.00", CultureInfo.InvariantCulture);
                CbIsAvailable.IsChecked = _component.IsAvailable;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            var selectedType = CbType.SelectedItem as ComponentType;
            if (selectedType == null)
            {
                MessageBox.Show("Выберите тип комплектующего.",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string name = TbName.Text.Trim();
            string inventory = TbInventory.Text.Trim();
            string levelText = TbLevel.Text.Trim();
            string priceText = TbPrice.Text.Trim().Replace(',', '.');

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Поле «Название» обязательно.",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                TbName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(inventory))
            {
                MessageBox.Show("Поле «Инвентарный номер» обязательно.",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                TbInventory.Focus();
                return;
            }

            // Проверка уникальности инвентарного номера
            bool invExists = App.Context.Components
                .Any(c => c.InventoryNumber == inventory && c.ComponentID != _component.ComponentID);

            if (invExists)
            {
                MessageBox.Show($"Комплектующее с инвентарным номером «{inventory}» уже существует.",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                TbInventory.Focus();
                return;
            }

            if (!int.TryParse(levelText, out int level) || level < 1 || level > 10)
            {
                MessageBox.Show("Уровень производительности должен быть числом от 1 до 10.",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                TbLevel.Focus();
                return;
            }

            if (!decimal.TryParse(priceText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price)
                || price < 0)
            {
                MessageBox.Show("Цена должна быть неотрицательным числом.",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                TbPrice.Focus();
                return;
            }

            try
            {
                _component.TypeID = selectedType.TypeID;
                _component.Name = name;
                _component.InventoryNumber = inventory;
                _component.PerformanceLevel = level;
                _component.Price = price;
                _component.IsAvailable = CbIsAvailable.IsChecked == true;

                if (_isNew) App.Context.Components.Add(_component);

                App.Context.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message,
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}