using System;
using System.IO;
using System.Text;

namespace NirDobovizki.MvcMarkdownTagFilter.Internal
{
    public class Utf8TagFilteringStream : TagFilteringStream
    {
        public Utf8TagFilteringStream(Stream targetStream, string startTag, string endTag, Converter<string, string> contentProcessor) :
            base(targetStream, Encoding.UTF8.GetBytes(startTag), Encoding.UTF8.GetBytes(endTag),
                (b, l) => Encoding.UTF8.GetBytes(contentProcessor(Encoding.UTF8.GetString(b, 0, l))))
        { }
    }
}
