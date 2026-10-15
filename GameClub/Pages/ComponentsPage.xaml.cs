using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GameClub.Pages
{
    public partial class ComponentsPage : Page
    {
        private List<Component> _allComponents;

        public ComponentsPage()
        {
            InitializeComponent();
            LoadComponents();
        }

        private void LoadComponents()
        {
            _allComponents = App.Context.Components
                .OrderBy(c => c.ComponentID)
                .ToList();

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            string query = TbSearch.Text.Trim().ToLower();

            IEnumerable<Component> filtered = _allComponents;

            if (!string.IsNullOrEmpty(query))
            {
                filtered = _allComponents.Where(c =>
                    (c.Name ?? "").ToLower().Contains(query) ||
                    (c.InventoryNumber ?? "").ToLower().Contains(query));
            }

            DgComponents.ItemsSource = filtered.ToList();
        }

        private Component GetSelectedComponent()
        {
            return DgComponents.SelectedItem as Component;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var w = new ComponentEditWindow(null);
            if (w.ShowDialog() == true) LoadComponents();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var component = GetSelectedComponent();
            if (component == null)
            {
                MessageBox.Show("Выберите комплектующее для редактирования.",
                                "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var w = new ComponentEditWindow(component);
            if (w.ShowDialog() == true) LoadComponents();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var component = GetSelectedComponent();
            if (component == null)
            {
                MessageBox.Show("Выберите комплектующее для удаления.",
                                "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int usageCount = App.Context.WorkstationComponents
                .Count(wc => wc.ComponentID == component.ComponentID);

            if (usageCount > 0)
            {
                MessageBox.Show(
                    $"Невозможно удалить комплектующее «{component.Name}».\n\n" +
                    $"Оно используется в {usageCount} конфигурациях рабочих мест.\n\n" +
                    $"Чтобы скрыть комплектующее из списка, снимите галочку " +
                    $"«Доступно» в окне редактирования.",
                    "Удаление невозможно",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Удалить комплектующее «{component.Name}»?",
                                         "Подтверждение",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                App.Context.Components.Remove(component);
                App.Context.SaveChanges();
                LoadComponents();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка удаления: " + ex.Message,
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e) => LoadComponents();
        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilter();
        private void DgComponents_MouseDoubleClick(object sender, MouseButtonEventArgs e) => BtnEdit_Click(sender, e);
    }
}