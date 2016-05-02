# MvcMarkdownTagFilter
Add markdown to any ASP.NET MVC view

Just add the [MarkdownFilter] attribute to any controller
or action and everything between &lt;md&gt; and &lt/md&gt
will be processed using the StackOverflow markdown processor

If you are not using MVC or want to control when markdown processing 
is applyed yourself you can use MarkdownFilter.ApplyTo on any HttpResponse object
at the begining of request processing to add the &lt;md&gt; tag processing 
