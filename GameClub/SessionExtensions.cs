using System;
using System.Xml.Linq;

namespace GameClub
{
    public partial class Session
    {
        public int MinutesPlayed
        {
            get
            {
                var end = EndTime ?? DateTime.Now;
                return (int)(end - StartTime).TotalMinutes;
            }
        }

        public decimal CurrentCost
        {
            get
            {
                if (Cost.HasValue) return Cost.Value;

                var tariff = Tariff;
                if (tariff == null) return 0;

                var end = EndTime ?? DateTime.Now;
                return Logic.CostCalculator.Calculate(StartTime, end, tariff.PricePerHour);
            }
        }
    }

    public partial class Workstation
    {
        public string DisplayName
        {
            get { return $"{Name} ({InventoryNumber})"; }
        }
    }

    public partial class Tariff
    {
        public string DisplayName
        {
            get { return $"{Name} ({PricePerHour:N0} ₽/час)"; }
        }
    }
}