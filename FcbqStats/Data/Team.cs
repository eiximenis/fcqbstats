using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FcbqStats.Data;

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int ClubId { get; set; }

    public Club Club { get; set; } = null!;
}
