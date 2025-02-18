using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FcbqStats.Responses;
public record ClubsListItem(int Id, string ClubCode, string Name);