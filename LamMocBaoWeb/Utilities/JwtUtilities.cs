using System.IdentityModel.Tokens.Jwt;

namespace LamMocBaoWeb.Utilities
{
    public static class JwtUtilities
    {
        public static JwtSecurityToken ReadJwtSecurityToken(string src)
        {
            var token = new JwtSecurityTokenHandler().ReadToken(src);
            if (token is JwtSecurityToken jwtSecurityToken)
            {
                return jwtSecurityToken;
            }

            return null;
        }
    }
}
