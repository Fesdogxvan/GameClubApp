using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GameClub.Pages
{
    public partial class GamesPage : Page
    {
        private List<Game> _allGames;

        public GamesPage()
        {
            InitializeComponent();
            LoadGames();
        }

        private void LoadGames()
        {
            _allGames = App.Context.Games
                .OrderBy(g => g.GameID)
                .ToList();

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            string query = TbSearch.Text.Trim().ToLower();

            IEnumerable<Game> filtered = _allGames;

            if (!string.IsNullOrEmpty(query))
            {
                filtered = _allGames.Where(g =>
                    (g.Name ?? "").ToLower().Contains(query) ||
                    (g.Genre ?? "").ToLower().Contains(query));
            }

            DgGames.ItemsSource = filtered.ToList();
        }

        private Game GetSelectedGame()
        {
            return DgGames.SelectedItem as Game;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var w = new GameEditWindow(null);
            if (w.ShowDialog() == true) LoadGames();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var game = GetSelectedGame();
            if (game == null)
            {
                MessageBox.Show("Выберите игру для редактирования.",
                                "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var w = new GameEditWindow(game);
            if (w.ShowDialog() == true) LoadGames();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var game = GetSelectedGame();
            if (game == null)
            {
                MessageBox.Show("Выберите игру для удаления.",
                                "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int sessionsCount = App.Context.Sessions
                .Count(s => s.GameID == game.GameID);

            if (sessionsCount > 0)
            {
                MessageBox.Show(
                    $"Невозможно удалить игру «{game.Name}».\n\n" +
                    $"С этой игрой связано {sessionsCount} сессий.\n" +
                    $"Удаление приведёт к потере истории.\n\n" +
                    $"Чтобы скрыть игру из списка, снимите галочку «Доступна» " +
                    $"в окне редактирования.",
                    "Удаление невозможно",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Удалить игру «{game.Name}»?",
                                         "Подтверждение",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                App.Context.Games.Remove(game);
                App.Context.SaveChanges();
                LoadGames();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка удаления: " + ex.Message,
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e) => LoadGames();
        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilter();
        private void DgGames_MouseDoubleClick(object sender, MouseButtonEventArgs e) => BtnEdit_Click(sender, e);
    }
}