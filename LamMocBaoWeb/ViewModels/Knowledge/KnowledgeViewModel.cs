using System;
using System.Collections.Generic;

namespace LamMocBaoWeb.ViewModels
{
    public class KnowledgeViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Content { get; set; }
        public List<string> ImagePreviews { get; set; }
        public Guid UploadedImageId { get; set; }
        public int SequenceNumber { get; set; }
    }

    public class KnowledgeLiteModel
    {
        public string Name { get; set; }
        public string LinkName { get; set; }
    }
}
