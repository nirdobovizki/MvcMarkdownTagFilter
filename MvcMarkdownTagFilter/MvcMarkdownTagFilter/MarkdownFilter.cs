using System.Web;
using System.Web.Mvc;

namespace NirDobovizki.MvcMarkdownTagFilter
{
    public class MarkdownFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ApplyTo(filterContext.HttpContext.Response);
        }

        public static void ApplyTo(HttpResponseBase response)
        {
            response.Filter = new Internal.Utf8TagFilteringStream(response.Filter, "<md>", "</md>", s => new MarkdownSharp.Markdown().Transform(s));
        }

    }
}
