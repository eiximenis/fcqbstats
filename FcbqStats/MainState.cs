using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FcbqStats.Data;

namespace FcbqStats;
public static class MainState
{
    public static Club? SelectedClub { get; set; }
    public static Team? SelectedTeam { get; set; }
    public static Match? SelectedMatch { get; set; }
}
