using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GameClub.Logic;

namespace GameClub.Pages
{
    public partial class SessionsPage : Page
    {
        public SessionsPage()
        {
            InitializeComponent();
            LoadComboBoxes();
            LoadActiveSessions();
        }
        private void LoadComboBoxes()
        {
            CbCustomer.ItemsSource = App.Context.Customers
                .Where(c => c.IsActive)
                .OrderBy(c => c.FullName)
                .ToList();

            RefreshWorkstationsCombo();

            CbTariff.ItemsSource = App.Context.Tariffs
                .Where(t => t.IsActive)
                .OrderBy(t => t.TariffID)
                .ToList();

            CbGame.ItemsSource = App.Context.Games
                .Where(g => g.IsAvailable)
                .OrderBy(g => g.Name)
                .ToList();
        }

        private void RefreshWorkstationsCombo()
        {
            var freeWorkstations = App.Context.Workstations
                .Where(w => w.Status == "Свободен")
                .OrderBy(w => w.WorkstationID)
                .ToList();

            CbWorkstation.ItemsSource = freeWorkstations;
        }

        private void LoadActiveSessions()
        {
            var sessions = App.Context.Sessions
                .Where(s => s.Status == "Активна")
                .OrderBy(s => s.SessionID)
                .ToList();

            DgSessions.ItemsSource = sessions;

            TbStats.Text = $"Активных сессий: {sessions.Count}";
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            var customer = CbCustomer.SelectedItem as Customer;
            var workstation = CbWorkstation.SelectedItem as Workstation;
            var tariff = CbTariff.SelectedItem as Tariff;
            var game = CbGame.SelectedItem as Game;

            if (customer == null || workstation == null || tariff == null)
            {
                MessageBox.Show("Выберите клиента, рабочее место и тариф.",
                                "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool customerHasActive = App.Context.Sessions
                .Any(s => s.CustomerID == customer.CustomerID && s.Status == "Активна");

            bool workstationHasActive = App.Context.Sessions
                .Any(s => s.WorkstationID == workstation.WorkstationID && s.Status == "Активна");

            string error;
            if (!SessionValidator.CanStart(workstation.Status,
                                            customerHasActive,
                                            workstationHasActive,
                                            out error))
            {
                MessageBox.Show(error, "Невозможно начать сессию",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (game != null)
            {
                int totalPerf = App.Context.WorkstationComponents
                    .Where(wc => wc.WorkstationID == workstation.WorkstationID
                                 && wc.RemovedDate == null)
                    .Select(wc => wc.Component.PerformanceLevel)
                    .DefaultIfEmpty(0)
                    .Sum();

                if (!GameCompatibilityChecker.CanRun(totalPerf, game.MinPerformanceLevel))
                {
                    var answer = MessageBox.Show(
                        $"Игра «{game.Name}» требует уровень производительности " +
                        $"{game.MinPerformanceLevel}, а у ПК «{workstation.Name}» — {totalPerf}.\n\n" +
                        $"Всё равно начать сессию?",
                        "Несовместимая игра",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (answer != MessageBoxResult.Yes)
                        return;
                }
            }

            try
            {
                var session = new Session
                {
                    CustomerID = customer.CustomerID,
                    UserID = App.CurrentUser.UserID,
                    WorkstationID = workstation.WorkstationID,
                    TariffID = tariff.TariffID,
                    GameID = game?.GameID,
                    StartTime = DateTime.Now,
                    EndTime = null,
                    Cost = null,
                    Status = "Активна"
                };

                App.Context.Sessions.Add(session);

                workstation.Status = "Занят";

                App.Context.SaveChanges();

                LoadActiveSessions();
                RefreshWorkstationsCombo();

                CbCustomer.SelectedIndex = -1;
                CbWorkstation.SelectedIndex = -1;
                CbTariff.SelectedIndex = -1;
                CbGame.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка создания сессии: " + ex.Message,
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void DgSessions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Обработчик нажатия на строку таблицы
        }

        private void BtnFinishRow_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var session = button?.Tag as Session;

            if (session == null) return;

            var answer = MessageBox.Show(
                $"Завершить сессию?\n\n" +
                $"Клиент: {session.Customer.FullName}\n" +
                $"ПК: {session.Workstation.Name}\n" +
                $"Начало: {session.StartTime:dd.MM.yyyy HH:mm}\n" +
                $"Длительность: {session.MinutesPlayed} мин.\n" +
                $"Стоимость: {session.CurrentCost:N2} ₽",
                "Завершение сессии",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (answer != MessageBoxResult.Yes) return;

            try
            {
                var endTime = DateTime.Now;

                decimal cost = CostCalculator.Calculate(session.StartTime, endTime,
                                                         session.Tariff.PricePerHour);

                session.EndTime = endTime;
                session.Cost = cost;
                session.Status = "Завершена";

                var workstation = App.Context.Workstations
                    .FirstOrDefault(w => w.WorkstationID == session.WorkstationID);
                if (workstation != null && workstation.Status == "Занят")
                {
                    workstation.Status = "Свободен";
                }

                App.Context.SaveChanges();

                LoadActiveSessions();
                RefreshWorkstationsCombo();

                MessageBox.Show($"Сессия завершена.\nИтоговая стоимость: {cost:N2} ₽",
                                "Готово",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка завершения сессии: " + ex.Message,
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadComboBoxes();
            LoadActiveSessions();
        }

        private void CbWorkstation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateStartHint();
        }

        private void CbTariff_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateStartHint();
        }

        private void UpdateStartHint()
        {
            var workstation = CbWorkstation.SelectedItem as Workstation;
            var tariff = CbTariff.SelectedItem as Tariff;

            if (workstation != null && tariff != null)
            {
                TbStartHint.Text = $"ПК: {workstation.Name} · Тариф: {tariff.Name} · " +
                                   $"{tariff.PricePerHour:N0} ₽/час";
            }
            else
            {
                TbStartHint.Text = "";
            }
        }
    }
}