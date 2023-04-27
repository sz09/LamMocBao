using Microsoft.AspNetCore.Identity;

namespace Services.Identify
{
    public class LMBIdentityResult: IdentityResult
    {
        public object Result { get; set; }

        public new static LMBIdentityResult Success(object result)
        {
            return new LMBIdentityResult
            {
                Result = result,
                Succeeded = true
            };
        }
    }
}
