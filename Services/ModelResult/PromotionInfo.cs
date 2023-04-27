using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public class PromotionInfo
    {
        public bool IsExpired { get; set; }
        public Shared.Models.Promotion Promotion { get; set; }
    }
}
