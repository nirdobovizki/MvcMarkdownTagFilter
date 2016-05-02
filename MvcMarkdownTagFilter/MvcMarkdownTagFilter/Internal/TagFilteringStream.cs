using System;
using System.IO;

namespace NirDobovizki.MvcMarkdownTagFilter.Internal
{
    public class TagFilteringStream : Stream
    {
        public delegate byte[] Processoer(byte[] buffer, int count);

        private Stream _targetStream;
        private byte[] _startTag;
        private byte[] _endTag;
        private Processoer _contentProcessor;
        private int _state;
        private int _tagPos;
        private MemoryStream _toProcesses;

        public TagFilteringStream(Stream targetStream, byte[] startTag, byte[] endTag, Processoer contentProcessor)
        {
            _targetStream = targetStream;
            _startTag = startTag;
            _endTag = endTag;
            _contentProcessor = contentProcessor;
            _toProcesses = new MemoryStream();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            int start = 0;
            for (var i = 0; i < count; ++i)
            {
                switch (_state)
                {
                    case 0:
                        if (buffer[i + offset] == _startTag[0])
                        {
                            _state = 1;
                            _tagPos = 1;
                            if (i != start)
                            {
                                _targetStream.Write(buffer, offset + start, i - start);
                            }
                            start = i;
                        }
                        break;
                    case 1:
                        if (buffer[i + offset] == _startTag[_tagPos])
                        {
                            _tagPos++;
                            if (_tagPos == _startTag.Length)
                            {
                                _state = 2;
                                start = i + 1;
                            }
                        }
                        else
                        {
                            _targetStream.Write(_startTag, 0, _tagPos);
                            _state = 0;
                            start = i;
                        }
                        break;
                    case 2:
                        if (buffer[i + offset] == _endTag[0])
                        {
                            _state = 3;
                            _tagPos = 1;
                            if (i != start)
                            {
                                _toProcesses.Write(buffer, offset + start, i - start);
                            }
                            start = i + 1;
                        }
                        break;
                    case 3:
                        if (buffer[i + offset] == _endTag[_tagPos])
                        {
                            _tagPos++;
                            if (_tagPos == _endTag.Length)
                            {
                                DoProcessing();
                                _state = 0;
                                start = i + 1;
                            }
                        }
                        else
                        {
                            _toProcesses.Write(_endTag, 0, _tagPos);
                            _state = 2;
                            start = i;
                        }
                        break;

                }
            }
            if (start < count)
            {
                switch (_state)
                {
                    case 0:
                        _targetStream.Write(buffer, offset + start, count - start);
                        break;
                    case 2:
                        _toProcesses.Write(buffer, offset + start, count - start);
                        break;
                }
            }
        }

        private void DoProcessing()
        {
            var result = _contentProcessor(_toProcesses.GetBuffer(), (int)_toProcesses.Length);
            _targetStream.Write(result, 0, result.Length);
        }

        public override void Flush()
        {
            if (_state != 0) throw new Exception("Missing closing tag");
            _targetStream.Flush();
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

    }
}
