using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GameClub.Pages
{
    public partial class TariffsPage : Page
    {
        private List<Tariff> _allTariffs;

        public TariffsPage()
        {
            InitializeComponent();
            LoadTariffs();
        }

        private void LoadTariffs()
        {
            _allTariffs = App.Context.Tariffs
                .OrderBy(t => t.TariffID)
                .ToList();

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            string query = TbSearch.Text.Trim().ToLower();

            IEnumerable<Tariff> filtered = _allTariffs;

            if (!string.IsNullOrEmpty(query))
            {
                filtered = _allTariffs.Where(t =>
                    (t.Name ?? "").ToLower().Contains(query) ||
                    (t.Description ?? "").ToLower().Contains(query));
            }

            DgTariffs.ItemsSource = filtered.ToList();
        }

        private Tariff GetSelectedTariff()
        {
            return DgTariffs.SelectedItem as Tariff;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new TariffEditWindow(null);
            if (editWindow.ShowDialog() == true)
            {
                LoadTariffs();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var tariff = GetSelectedTariff();
            if (tariff == null)
            {
                MessageBox.Show("Выберите тариф для редактирования.",
                                "Внимание",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            var editWindow = new TariffEditWindow(tariff);
            if (editWindow.ShowDialog() == true)
            {
                LoadTariffs();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var tariff = GetSelectedTariff();
            if (tariff == null)
            {
                MessageBox.Show("Выберите тариф для удаления.",
                                "Внимание",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            int sessionsCount = App.Context.Sessions
                .Count(s => s.TariffID == tariff.TariffID);

            if (sessionsCount > 0)
            {
                MessageBox.Show(
                    $"Невозможно удалить тариф «{tariff.Name}».\n\n" +
                    $"По этому тарифу оформлено {sessionsCount} сессий.\n" +
                    $"Удаление тарифа приведёт к потере истории.\n\n" +
                    $"Чтобы скрыть тариф из списка, снимите галочку «Активен» " +
                    $"в окне редактирования.",
                    "Удаление невозможно",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Удалить тариф «{tariff.Name}»?",
                                         "Подтверждение удаления",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                App.Context.Tariffs.Remove(tariff);
                App.Context.SaveChanges();
                LoadTariffs();
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
            LoadTariffs();
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void DgTariffs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BtnEdit_Click(sender, e);
        }
    }
}