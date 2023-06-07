namespace Core.Modeles
{
    public class SQLTeamMembersRepository : ITeamMembersRepository
    {
     #pragma warning disable CS8603 // Possible null reference return.
     #pragma warning disable CS8600 // Possible null reference return.

        private readonly AppDbContext context;

        public SQLTeamMembersRepository(AppDbContext context)
        {
            this.context = context;
        }


        public TeamMember Add(TeamMember newMember)
        {
           context.Members.Add(newMember);
            context.SaveChanges();
            return newMember;
        }

        public TeamMember Delete(int id)
        {
            TeamMember member = context.Members.Find(id);
            if (member != null)
            {
                context.Members.Remove(member);
                context.SaveChanges();
            }
            return member;
        }



        public IEnumerable<TeamMember> GetAllMembers()
        {
         return context.Members.ToList();
        }

        public TeamMember GetMember(int id)
        {
            return context.Members.Find(id);
        }

        public TeamMember Update(TeamMember updatedMember)
        {
            var member=context.Members.Attach(updatedMember);
            member.State=Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return updatedMember;
        }
    }
}
