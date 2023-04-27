using System;

namespace LamMocBaoWeb.ViewModels
{
    public class NewsPaperPostViewModel
    {
        public Guid Id { get; set; }

        public string Hint { get; set; }
        public string Link { get; set; }
        public string ImagePreview { get; set; }
        public int SequenceNumber { get; set; }
    }
}
