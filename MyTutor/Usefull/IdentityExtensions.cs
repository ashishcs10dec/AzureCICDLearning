using System.Security.Claims;
using System.Security.Principal;

namespace MyTutor.Usefull
{
    public static class IdentityExtensions
    {
        public static int GetUserId1(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(ClaimTypes.PrimarySid);

            if (claim == null)
                return 0;

            return int.Parse(claim.Value);
        }

        public static string GetName(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(ClaimTypes.Name);

            return claim?.Value ?? string.Empty;
        }
    }
}
