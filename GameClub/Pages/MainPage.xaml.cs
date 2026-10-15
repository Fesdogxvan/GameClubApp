using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GameClub.Pages
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            LoadWorkstations();
        }

        private void LoadWorkstations()
        {
            IcWorkstations.Items.Clear();

            var workstations = App.Context.Workstations
                .OrderBy(w => w.WorkstationID)
                .ToList();

            int free = 0, busy = 0, service = 0;

            foreach (var ws in workstations)
            {
                if (ws.Status == "Свободен") free++;
                if (ws.Status == "Занят") busy++;
                if (ws.Status == "Обслуживание") service++;

                IcWorkstations.Items.Add(CreateWorkstationCard(ws));
            }

            TbStats.Text = $"Свободно: {free}   |   Занято: {busy}   |   Обслуживание: {service}";
        }

        private Border CreateWorkstationCard(Workstation ws)
        {
            // Цвет в зависимости от статуса
            Brush statusBrush;
            switch (ws.Status)
            {
                case "Свободен":
                    statusBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50"));
                    break;
                case "Занят":
                    statusBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E53935"));
                    break;
                case "Обслуживание":
                    statusBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FB8C00"));
                    break;
                default:
                    statusBrush = Brushes.Gray;
                    break;
            }

            var border = new Border
            {
                Width = 220,
                Height = 130,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2A2A3E")),
                CornerRadius = new CornerRadius(8),
                Margin = new Thickness(5),
                Padding = new Thickness(12),
                BorderBrush = statusBrush,
                BorderThickness = new Thickness(2)
            };

            var stack = new StackPanel();

            stack.Children.Add(new TextBlock
            {
                Text = ws.Name,
                Foreground = Brushes.White,
                FontSize = 16,
                FontWeight = FontWeights.Bold
            });

            stack.Children.Add(new TextBlock
            {
                Text = $"Инв. № {ws.InventoryNumber}",
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A0A0B0")),
                FontSize = 11,
                Margin = new Thickness(0, 3, 0, 8)
            });

            stack.Children.Add(new TextBlock
            {
                Text = ws.Location ?? "—",
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A0A0B0")),
                FontSize = 11,
                Margin = new Thickness(0, 0, 0, 8)
            });

            stack.Children.Add(new TextBlock
            {
                Text = ws.Status.ToUpper(),
                Foreground = statusBrush,
                FontSize = 13,
                FontWeight = FontWeights.Bold
            });

            border.Child = stack;
            return border;
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadWorkstations();
        }
    }
}