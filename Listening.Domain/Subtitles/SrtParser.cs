using Listening.Domain.SubTitles;
using Listening.Domain.ValueObjects;
using SubtitlesParser.Classes;
using System.Text;

namespace Listening.Domain.Subtitles
{
    public class SrtParser : ISubtitleParser
    {
        public bool Accept(string typeName)
        {
            return typeName.Equals("srt", StringComparison.OrdinalIgnoreCase);
        }

        public IEnumerable<Sentence> Parse(string subtitle)
        {
            var parser = new SubtitlesParser.Classes.Parsers.SubParser();
            using MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(subtitle));
            List<SubtitleItem> subtitles = parser.ParseStream(memoryStream);
            return subtitles.Select(s => new Sentence(TimeSpan.FromMilliseconds(s.StartTime),
                TimeSpan.FromMilliseconds(s.EndTime),
                string.Join(" ", s.Lines)));
        }
    }
}
