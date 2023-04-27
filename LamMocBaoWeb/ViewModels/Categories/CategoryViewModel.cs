using LamMocBaoWeb.ViewModels;
using System;
using System.Collections.Generic;

namespace LamMocBaoWeb.ViewModels
{
    public class CategoryViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public int SequenceNumber { get; set; }
    }
}
