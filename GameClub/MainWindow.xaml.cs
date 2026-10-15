using System.Windows;
using GameClub.Pages;
namespace GameClub
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (App.CurrentUser != null)
            {
                TbUserName.Text = App.CurrentUser.FullName;
                TbUserRole.Text = App.CurrentUser.RoleUser == "Admin"
                    ? "Администратор"
                    : "Сотрудник";

                if (App.CurrentUser.RoleUser != "Admin")
                {
                    BtnTariffs.Visibility = Visibility.Collapsed;
                    BtnGames.Visibility = Visibility.Collapsed;
                    BtnComponents.Visibility = Visibility.Collapsed;
                }
            }
            MainFrame.Navigate(new MainPage());
        }

        private void BtnMain_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MainPage());
        }

        private void BtnSessions_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SessionsPage());
        }

        private void BtnHistory_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new HistoryPage());
        }

        private void BtnCustomers_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CustomersPage());
        }

        private void BtnTariffs_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TariffsPage());
        }

        private void BtnGames_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new GamesPage());
        }

        private void BtnComponents_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate (new ComponentsPage());
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Выйти из учётной записи?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                App.CurrentUser = null;

                var login = new LoginWindow();
                login.Show();
                this.Close();
            }
        }
    }
}