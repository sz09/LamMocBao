using System;
using System.Collections.Generic;

namespace LamMocBaoWeb.Models
{
    public class AdvisingModel
    {
        public string CustomerName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime? Birthday { get; set; }

        public string InterestedInProductIds { get; set; }
        public bool IncludeOtherProductsInterestedIn { get; set; }
    }

    public class AdvisingModelByProduct: AdvisingModel
    {
        public Guid InterestedInProductId { get; set; }
    }

}
