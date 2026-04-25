using System.Text;
using DNTPersianUtils.Core;

namespace DNTCommon.Web.Core;

public static class HtmlPaginator
{
    public static string CreateSimplePaginator(int totalItemCount,
        int itemsPerPage,
        int? currentPage,
        bool showNumbersInPersian,
        Func<int, string> getPageHref,
        string dataSourceIsEmptyMessage = "اطلاعاتی برای نمایش یافت نشد.",
        int marginTop = 3,
        int maxPagerItems = 10,
        string paginationClass = "pagination shadow-sm",
        string paginationItemClass = "page-item",
        string paginationPageLinkClass = "page-link",
        string paginationScrollToId = "header")
    {
        ArgumentNullException.ThrowIfNull(getPageHref);

        // اگر داده‌ای وجود نداشت
        if (totalItemCount == 0)
        {
            return $"""
                    <div class="alert alert-warning" role="alert">
                        {dataSourceIsEmptyMessage}
                    </div>
                    """;
        }

        // محاسبه تعداد صفحات کل
        var totalPages = (int)Math.Ceiling(totalItemCount / (double)itemsPerPage);
        var hasPagination = totalPages > 1;

        if (!hasPagination)
        {
            return string.Empty;
        }

        // تنظیم currentPage
        var validCurrentPage = currentPage ?? 1;

        if (validCurrentPage < 1)
        {
            validCurrentPage = 1;
        }

        if (validCurrentPage > totalPages)
        {
            validCurrentPage = totalPages;
        }

        // محاسبه pagerStart و pagerEnd
        int pagerStart, pagerEnd;

        if (totalPages <= maxPagerItems)
        {
            pagerStart = 1;
            pagerEnd = totalPages;
        }
        else
        {
            var maxPagesBeforeCurrentPage = (int)Math.Floor(maxPagerItems / (decimal)2);
            var maxPagesAfterCurrentPage = (int)Math.Ceiling(maxPagerItems / (decimal)2) - 1;

            if (validCurrentPage <= maxPagesBeforeCurrentPage)
            {
                pagerStart = 1;
                pagerEnd = maxPagerItems;
            }
            else if (validCurrentPage + maxPagesAfterCurrentPage >= totalPages)
            {
                pagerStart = totalPages - maxPagerItems + 1;
                pagerEnd = totalPages;
            }
            else
            {
                pagerStart = validCurrentPage - maxPagesBeforeCurrentPage;
                pagerEnd = validCurrentPage + maxPagesAfterCurrentPage;
            }
        }

        // ساخت HTML پیجینیشن
        var html = new StringBuilder();

        html.AppendLine(CultureInfo.InvariantCulture, $"""
                                                       <div class="row mt-{marginTop}">
                                                           <nav class="d-flex justify-content-center" aria-label="Pagination">
                                                               <ul class="{paginationClass}">
                                                       """);

        // دکمه صفحه اول و ...
        if (pagerStart != 1)
        {
            html.AppendLine(CultureInfo.InvariantCulture, $"""
                                                                       <li class="{paginationItemClass}">
                                                                           <a class="{paginationPageLinkClass}" href="{getPageHref(arg: 1)}#{paginationScrollToId}">&laquo;</a>
                                                                       </li>
                                                                       <li class="{paginationItemClass}">
                                                                           <a class="{paginationPageLinkClass}" href="{getPageHref(pagerStart - 1)}#{paginationScrollToId}">...</a>
                                                                       </li>
                                                           """);
        }

        // حلقه شماره صفحات
        for (var pageNo = pagerStart; pageNo <= pagerEnd; pageNo++)
        {
            var numbers = showNumbersInPersian
                ? pageNo.ToPersianNumbers()
                : pageNo.ToString(CultureInfo.InvariantCulture);

            if (pageNo == validCurrentPage)
            {
                html.AppendLine(CultureInfo.InvariantCulture, $"""
                                                                           <li class="{paginationItemClass} active">
                                                                               <span class="{paginationPageLinkClass}">{numbers}</span>
                                                                           </li>
                                                               """);
            }
            else
            {
                html.AppendLine(CultureInfo.InvariantCulture, $"""
                                                                           <li class="{paginationItemClass}">
                                                                               <a class="{paginationPageLinkClass}" href="{getPageHref(pageNo)}#{paginationScrollToId}">{numbers}</a>
                                                                           </li>
                                                               """);
            }
        }

        // دکمه ... و صفحه آخر
        if (pagerEnd < totalPages)
        {
            html.AppendLine(CultureInfo.InvariantCulture, $"""
                                                                       <li class="{paginationItemClass}">
                                                                           <a class="{paginationPageLinkClass}" href="{getPageHref(pagerEnd + 1)}#{paginationScrollToId}">...</a>
                                                                       </li>
                                                                       <li class="{paginationItemClass}">
                                                                           <a class="{paginationPageLinkClass}" href="{getPageHref(totalPages)}#{paginationScrollToId}">&raquo;</a>
                                                                       </li>
                                                           """);
        }

        html.AppendLine(value: """
                                       </ul>
                                   </nav>
                               </div>
                               """);

        return html.ToString();
    }
}
