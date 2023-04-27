using AutoMapper;
using Shared.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LamMocBaoWeb.Utilities
{
    public class ResultListView<T>
    {
        public List<T> Data { get; private set; }
        public int Total { get; private set; }
        public int MaxPage { get; private set; }

        public static ResultListView<T> From<T1>(SearchResult<T1> searchResult, int pageSize, IMapper mapper)
        {
            var posiblePages = new List<int>();

            return new ResultListView<T>
            {
                Data = mapper.Map<List<T>>(searchResult.Data),
                Total = searchResult.Total,
                MaxPage = (searchResult.Total / pageSize) + (searchResult.Total % pageSize > 0 ? 1 : 0) - 1
            };
        }
    }

    public class PageUtitlities
    {
        private static int NUMBER_SHOWING_PAGE = 3;
        private static string PAGE_INDEX_TEMPLATE = "<li class='page-item'><input type='hidden' class='page-link {0}' href='#' name='Page' value='{1}'/>{2}</li>";
        private static string PAGE_INDEX_TEMPLATE_1 = "<li class='page-item {2}'><a type='button' class='page-link btn btn-sm border' onclick='Pagination.GoToPage({0})'><span aria-hidden='true'>{1}</span><span class='sr-only'>{1}</span></a></li>";
        private static string FIRST = "<li class='page-item'><a class='page-link' href='#' onclick='Pagination.GoToPage({0})'><span aria-hidden='true'>&laquo;</span><span class='sr-only'>{1}</span></a></li>";
        private static string LAST = "<li class='page-item'><a class='page-link' href='#' onclick='Pagination.GoToPage({0})'> <span aria-hidden='true'>&raquo;</span><span class='sr-only'></span></a></li>";
        public static string Pagination1(int maxPage, int currentPage, string paddingCss = "m-auto")
        {
            var possiblePages = new List<int>();
            if (currentPage > 1)
            {
                possiblePages.Add(currentPage - 1);
            }

            int index = currentPage;
            do
            {
                possiblePages.Add(index++);
            }
            while (possiblePages.Count < NUMBER_SHOWING_PAGE && index <= maxPage);

            if (possiblePages.Count < NUMBER_SHOWING_PAGE)
            {
                for (int i = possiblePages.First() - 1; i >= 1; i--)
                {
                    possiblePages.Insert(0, i);
                    if (possiblePages.Count == NUMBER_SHOWING_PAGE)
                    {
                        break;
                    }
                }
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"<div class='pagination-panel {paddingCss}'>");
            stringBuilder.AppendLine("<ul class='pagination'>");
            stringBuilder.AppendLine(FIRST);
            foreach (var item in possiblePages)
            {
                stringBuilder.AppendLine(string.Format(PAGE_INDEX_TEMPLATE, currentPage == item ? "btn btn-dark" : string.Empty, item));
            }

            stringBuilder.AppendLine(string.Format(LAST, maxPage));
            stringBuilder.AppendLine("</ul>");
            stringBuilder.AppendLine("<input type='text' class='d-none' id='hidden-paging' name='Page' />");
            stringBuilder.AppendLine("</div>");
            return stringBuilder.ToString();
        }
        public static string Pagination(int maxPage, int currentPage, string paddingCss = "m-auto")
        {
            var possiblePages = new List<int>();
            var posibleFirstPages = new List<int>();
            for (int i = 0; i < NUMBER_SHOWING_PAGE; i++)
            {
                posibleFirstPages.Add(i);
            }
            var posibleLastPages = new List<int>();
            for (int i = 0; i < NUMBER_SHOWING_PAGE; i++)
            {
                posibleLastPages.Add(maxPage - i);
            }
            posibleLastPages.Sort();
            if (maxPage <= 6) // Show all list
            {
                for (int i = 0; i < maxPage; i++)
                {
                    possiblePages.Add(i);
                }
            }
            else
            {
                if (currentPage < 2 || currentPage > maxPage - 2)
                {
                    possiblePages.AddRange(posibleFirstPages);
                    possiblePages.AddRange(posibleLastPages);
                }
                else
                {
                    possiblePages.Add(0);

                    possiblePages.Add(currentPage - 1);
                    possiblePages.Add(currentPage);
                    possiblePages.Add(currentPage + 1);

                    possiblePages.Add(maxPage);
                }
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"<div class='pagination-panel {paddingCss}'>");
            stringBuilder.AppendLine("<ul class='pagination'>");
            var saved = -1;
            foreach (var item in possiblePages)
            {
                if (item > saved + 1)
                {
                    // append dot
                    stringBuilder.AppendLine("<li class='page-item middle'><a>...</a></li>");
                }

                stringBuilder.AppendLine(string.Format(PAGE_INDEX_TEMPLATE_1, item, item + 1, item == currentPage ? "active" : ""));
                saved = item;
            }
            stringBuilder.AppendLine("</ul>");
            stringBuilder.AppendLine("<input type='text' class='d-none' id='hidden-paging' name='page' />");
            stringBuilder.AppendLine("</div>");
            return stringBuilder.ToString();
        }
    }
}
