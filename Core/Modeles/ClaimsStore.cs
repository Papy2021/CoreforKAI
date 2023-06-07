using System.Security.Claims;

namespace Core.Modeles
{
    public static class ClaimsStore
    {
        public static List<Claim> AllClaims = new()
        {
          new Claim("Edit Role", "Edit Role"),
          new Claim("Create Role", "Create Role"),
          new Claim("Delete Role", "Delete Role"),
        };
    }
}
