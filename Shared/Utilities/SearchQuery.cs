using System;
using System.Collections.Generic;

namespace Shared.Utilities
{
    public class SearchQuery<TEntity>
    {
        private string _searchText { get; set; }
        public string Search { get { return _searchText;  } set { _searchText = value?.Trim() ?? null; } }
        private string _orderBy;
        public string OrderBy
        {
            get
            {
                return _orderBy;
            }
            set
            {
                _orderBy = value;
                var parts = _orderBy.Split(" ", StringSplitOptions.TrimEntries);
                if (parts.Length == 2)
                {
                    Order = parts[0];
                    switch (parts[1])
                    {
                        case "desc":
                            OrderDirection = OrderDirection.Descending;
                            break;
                        default:
                            OrderDirection = OrderDirection.Ascending;
                            break;
                    }
                }
            }
        }
        public string Order { get; set; }
        public OrderDirection OrderDirection { get; set; }
        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = 10;
        public Func<TEntity, bool> SearchFunc { get; set; }
        public int Skip => Page * PageSize;
        public static SearchQuery<TEntity> Default = new SearchQuery<TEntity>();
        public bool IncludeTotal { get; set; } = true;
        public Filter<TEntity> Filter { get; set; }
    }

    public enum OrderDirection
    {
        Ascending,
        Descending
    }


    public abstract class Filter<TEntity>
    {
        public Func<TEntity, bool> Predicate { get; set; }
    }

    public class EagerLoadings
    {
        public List<string> NavigationCollectionNames { get; set; } = new List<string>();
        public List<string> NavigationValueNames { get; set; } = new List<string>();
    }
}
