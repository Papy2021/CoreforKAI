using Core.Modeles;
using System.Diagnostics.CodeAnalysis;

namespace Core.ViewModeles
{
    public class TeamIndexViewModel
    {
        [AllowNull]
       public  IEnumerable<TeamMember> ListTeamMembers { get; set; }

        [AllowNull]
        public string PageTitle { get; set; }
    }
}
