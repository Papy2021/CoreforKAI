using Core.Modeles;
using System.Diagnostics.CodeAnalysis;

namespace Core.ViewModeles
{
    public class TeamDetailsViewModel
    {
        [AllowNull]
        public TeamMember Member { get; set; }
        [AllowNull]
        public string PageTitle { get; set; }
    }
}
