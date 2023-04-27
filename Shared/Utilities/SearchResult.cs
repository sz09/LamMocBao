using Shared.Models;
using System.Collections.Generic;

namespace Shared.Utilities
{
    public class SearchResult<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int Total { get; set; }
    }
}
