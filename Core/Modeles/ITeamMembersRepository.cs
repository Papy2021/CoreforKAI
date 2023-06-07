using System.Collections;

namespace Core.Modeles
{
    public interface ITeamMembersRepository
    {
        IEnumerable<TeamMember> GetAllMembers();
        TeamMember GetMember(int id);
        TeamMember Add(TeamMember NewEmployee);
        TeamMember Update(TeamMember UpdatedEmployee);
        TeamMember Delete(int id);

    }
}
