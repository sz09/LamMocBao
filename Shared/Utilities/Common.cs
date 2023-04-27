using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Utilities
{
    public class IdLabel
    {
        public Guid? Id { get; set; }
        public string Label { get; set; }
        public object ExtraInfos { get; set; }
    }

    public static class CommonExtensions 
    {
        public static bool HasValue(this Guid? guid)
        {
            return guid.HasValue && guid.Value != Guid.Empty;
        }
    }
}
