using System;
using System.Windows;

namespace GameClub
{
    public partial class GameEditWindow : Window
    {
        private readonly Game _game;
        private readonly bool _isNew;

        public GameEditWindow(Game game)
        {
            InitializeComponent();

            if (game == null)
            {
                _isNew = true;
                _game = new Game { IsAvailable = true, MinPerformanceLevel = 5 };
                TbTitle.Text = "Новая игра";
                TbMinLevel.Text = "5";
            }
            else
            {
                _isNew = false;
                _game = game;
                TbTitle.Text = "Редактирование игры";

                TbName.Text = _game.Name;
                TbGenre.Text = _game.Genre;
                TbMinLevel.Text = _game.MinPerformanceLevel.ToString();
                CbIsAvailable.IsChecked = _game.IsAvailable;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string name = TbName.Text.Trim();
            string genre = TbGenre.Text.Trim();
            string levelText = TbMinLevel.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Поле «Название» обязательно.",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                TbName.Focus();
                return;
            }

            if (!int.TryParse(levelText, out int level) || level < 1 || level > 10)
            {
                MessageBox.Show("Уровень производительности должен быть целым числом от 1 до 10.",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                TbMinLevel.Focus();
                return;
            }

            try
            {
                _game.Name = name;
                _game.Genre = string.IsNullOrEmpty(genre) ? null : genre;
                _game.MinPerformanceLevel = level;
                _game.IsAvailable = CbIsAvailable.IsChecked == true;

                if (_isNew) App.Context.Games.Add(_game);

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