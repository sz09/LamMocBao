using Shared.Models;
using System;
using System.Collections.Generic;

namespace LamMocBaoWeb.ViewModels
{
    public class CustomerDesiringViewModel
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<CustomerPreferedInfos> CustomerPrefereds { get; set; }

        public ICollection<CustomerDesiringViewModel> RelatedInfomations { get; set; }
    }

}
