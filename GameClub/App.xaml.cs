using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
namespace GameClub
{
    public partial class App : Application
    {
        public static GameClubEntities Context { get; set; } = new GameClubEntities();
        public static User CurrentUser { get; set; }
    }
}
