using Shared.Models;
using System;
using System.Collections.Generic;

namespace Services.ModelResult
{
    public class ProductTypeTagModel
    {
        public Guid Id { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
