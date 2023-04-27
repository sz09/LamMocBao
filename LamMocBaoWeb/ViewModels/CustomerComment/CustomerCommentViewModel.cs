using System;

namespace LamMocBaoWeb.ViewModels
{
    public class CustomerCommentViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LinkName { get; set; }
        public string Content { get; set; }
        public string ImagePreview { get; set; }
        public int SequenceNumber { get; set; }
    }
}
