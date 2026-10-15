using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GameClub.Pages
{
    public partial class HistoryPage : Page
    {
        private List<Session> _allSessions;

        public HistoryPage()
        {
            InitializeComponent();
            LoadFilters();
            LoadSessions();
        }

        private void LoadFilters()
        {
            var customers = App.Context.Customers
                .OrderBy(c => c.FullName)
                .ToList();

            var customerList = new List<object> { "Все клиенты" };
            customerList.AddRange(customers);
            CbFilterCustomer.ItemsSource = customerList;
            CbFilterCustomer.SelectedIndex = 0;

            var workstations = App.Context.Workstations
                .OrderBy(w => w.WorkstationID)
                .ToList();

            var workstationList = new List<object> { "Все ПК" };
            workstationList.AddRange(workstations);
            CbFilterWorkstation.ItemsSource = workstationList;
            CbFilterWorkstation.SelectedIndex = 0;
        }

        private void LoadSessions()
        {
            _allSessions = App.Context.Sessions
                .OrderByDescending(s => s.StartTime)
                .ToList();

            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (_allSessions == null) return;

            IEnumerable<Session> filtered = _allSessions;

            var customerFilter = CbFilterCustomer.SelectedItem as Customer;
            if (customerFilter != null)
                filtered = filtered.Where(s => s.CustomerID == customerFilter.CustomerID);

            var workstationFilter = CbFilterWorkstation.SelectedItem as Workstation;
            if (workstationFilter != null)
                filtered = filtered.Where(s => s.WorkstationID == workstationFilter.WorkstationID);

            var statusItem = CbFilterStatus.SelectedItem as ComboBoxItem;
            string status = statusItem?.Content?.ToString();
            if (!string.IsNullOrEmpty(status) && status != "Все")
                filtered = filtered.Where(s => s.Status == status);

            if (DpFrom.SelectedDate.HasValue)
            {
                var from = DpFrom.SelectedDate.Value.Date;
                filtered = filtered.Where(s => s.StartTime.Date >= from);
            }

            if (DpTo.SelectedDate.HasValue)
            {
                var to = DpTo.SelectedDate.Value.Date;
                filtered = filtered.Where(s => s.StartTime.Date <= to);
            }

            var result = filtered.ToList();
            DgHistory.ItemsSource = result;

            UpdateStats(result);
        }

        private void UpdateStats(List<Session> sessions)
        {
            TbStatCount.Text = $"Сессий: {sessions.Count}";

            decimal revenue = sessions
                .Where(s => s.Status == "Завершена" && s.Cost.HasValue)
                .Sum(s => s.Cost.Value);

            TbStatRevenue.Text = $"Выручка: {revenue:N2} руб.";

            var completed = sessions
                .Where(s => s.Status == "Завершена" && s.EndTime.HasValue)
                .ToList();

            if (completed.Any())
            {
                double avgMinutes = completed
                    .Average(s => (s.EndTime.Value - s.StartTime).TotalMinutes);

                TbStatAvg.Text = $"Средняя длительность: {avgMinutes:F1} мин.";
            }
            else
            {
                TbStatAvg.Text = "Средняя длительность: —";
            }
        }
        private void FilterCombo_Changed(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void FilterDate_Changed(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            CbFilterCustomer.SelectedIndex = 0;
            CbFilterWorkstation.SelectedIndex = 0;
            CbFilterStatus.SelectedIndex = 0;
            DpFrom.SelectedDate = null;
            DpTo.SelectedDate = null;

            ApplyFilters();
        }
    }
}