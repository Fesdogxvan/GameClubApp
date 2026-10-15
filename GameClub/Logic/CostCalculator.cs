using System;

namespace GameClub.Logic
{
    public static class CostCalculator
    {
        public static decimal Calculate(DateTime startTime, DateTime endTime, decimal pricePerHour)
        {
            if (endTime < startTime)
                throw new ArgumentException("Время окончания не может быть раньше времени начала.");

            if (pricePerHour < 0)
                throw new ArgumentException("Стоимость часа не может быть отрицательной.");

            double totalMinutes = (endTime - startTime).TotalMinutes;
            decimal cost = (decimal)(totalMinutes / 60.0) * pricePerHour;

            return Math.Round(cost, 2, MidpointRounding.AwayFromZero);
        }
    }
}