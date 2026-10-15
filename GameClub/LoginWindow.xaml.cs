using System.Linq;
using System.Windows;
using GameClub.Logic;

namespace GameClub
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            TbError.Visibility = Visibility.Collapsed;

            string login = TbLogin.Text.Trim();
            string password = PbPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                ShowError("Заполните все поля.");
                return;
            }

            var user = App.Context.Users.FirstOrDefault(u => u.Login == login);

            if (user == null || !user.IsActive || !PasswordHasher.Verify(password, user.PasswordHash))
            {
                ShowError("Неверный логин или пароль.");
                return;
            }

            App.CurrentUser = user;

            var main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void ShowError(string message)
        {
            TbError.Text = message;
            TbError.Visibility = Visibility.Visible;
        }
    }
}