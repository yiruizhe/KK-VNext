using Listening.Domain.SubTitles;
using Listening.Domain.ValueObjects;
using Opportunity.LrcParser;

namespace Listening.Domain.Subtitles
{
    public class LrcParser : ISubtitleParser
    {
        public bool Accept(string typeName)
        {
            return typeName.Equals("lrc", StringComparison.OrdinalIgnoreCase);
        }

        public IEnumerable<Sentence> Parse(string subtitle)
        {
            IParseResult<Line> res = Lyrics.Parse(subtitle);
            if (res.Exceptions.Count > 0)
            {
                throw new ApplicationException("解析lrc失败");
            }
            res.Lyrics.PreApplyOffset();//应用上[offset:500]这样的偏移
            return FromLrc(res.Lyrics);
        }

        private IEnumerable<Sentence> FromLrc(Lyrics<Line> lyrics)
        {
            var lines = lyrics.Lines;
            var sentences = new List<Sentence>();

            for (int i = 0; i < lines.Count - 1; i++)
            {
                Line curLine = lines[i];
                Line nextLine = lines[i + 1];
                var sentence = new Sentence(curLine.Timestamp.TimeOfDay, nextLine.Timestamp.TimeOfDay, curLine.Content);
                sentences.Add(sentence);
            }
            // 处理最后一行
            var lastLine = lines.Last();
            TimeSpan lastLineStartTime = lastLine.Timestamp.TimeOfDay;
            TimeSpan lastLineEndTime = lastLineStartTime.Add(TimeSpan.FromMinutes(1));
            var lastSentence = new Sentence(lastLineStartTime, lastLineEndTime, lastLine.Content);
            sentences.Add(lastSentence);
            return sentences;
        }
    }
}
