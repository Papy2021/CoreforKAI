namespace Core.Modeles
{
    public class TeamMemberInCodeRepository : ITeamMembersRepository
    {
        #pragma warning disable CS8603 // Possible null reference return.
        #pragma warning disable CS8600 // Possible null reference return.

        public readonly List<TeamMember> _Members;


        public TeamMemberInCodeRepository()
        {
            _Members = new List<TeamMember>() {

            new TeamMember() {Id=1, Name="Papy", Position=Position.Technician},
            new TeamMember() {Id=2, Name="Dane", Position=Position.Camera_Assistant},
            new TeamMember() {Id=4, Name="Mum", Position=Position.Producer},
            };

        }

        public TeamMember Add(TeamMember newMember)
        {
            newMember.Id = _Members.Count() + 1;
            _Members.Add(newMember);
            return newMember;
        }

        public TeamMember Delete(int id)
        {

            TeamMember memberToDelete = _Members.FirstOrDefault(x => x.Id == id);
            if (memberToDelete != null)
            {
               _Members.Remove(memberToDelete);
            }
            return memberToDelete;
        }

        public IEnumerable<TeamMember> GetAllMembers()
        {
            return _Members;
        }

        public TeamMember GetMember(int id)
        {

            return _Members.FirstOrDefault<TeamMember>((e) => e.Id == id);

        }

        public TeamMember Update(TeamMember updatedMember)
        {
            TeamMember member = _Members.FirstOrDefault(x => x.Id == updatedMember.Id);
            if (member != null)
            {
                //employee.Id = UpdatedEmployee.Id;
                member.Name = updatedMember.Name;
               member.Position = updatedMember.Position;
            }
            return member;
        }
    }
}
