namespace GameClub.Logic
{
    public static class SessionValidator
    {
        public static bool CanStart(string workstationStatus,
                                    bool customerHasActiveSession,
                                    bool workstationHasActiveSession,
                                    out string errorMessage)
        {
            errorMessage = null;

            if (workstationStatus != "Свободен")
            {
                if (workstationStatus == "Занят")
                    errorMessage = "Выбранное рабочее место уже занято.";
                else if (workstationStatus == "Обслуживание")
                    errorMessage = "Рабочее место находится на обслуживании.";
                else
                    errorMessage = "Рабочее место недоступно.";

                return false;
            }

            if (customerHasActiveSession)
            {
                errorMessage = "У клиента уже есть активная сессия.";
                return false;
            }

            if (workstationHasActiveSession)
            {
                errorMessage = "У выбранного рабочего места уже есть активная сессия.";
                return false;
            }

            return true;
        }
    }
}