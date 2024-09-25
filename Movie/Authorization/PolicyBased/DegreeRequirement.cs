using Microsoft.AspNetCore.Authorization;

namespace Movie.Authorization.PolicyBased
{
    public class DegreeRequirement:IAuthorizationRequirement
    {
        public DegreeRequirement(int degree)
        {
            Degree = degree;
        }

        public int Degree { get; }
    }
}
