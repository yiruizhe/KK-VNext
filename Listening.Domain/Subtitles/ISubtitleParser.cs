using Listening.Domain.ValueObjects;

namespace Listening.Domain.SubTitles
{
    public interface ISubtitleParser
    {
        /// <summary>
        /// 本解析器是否可以解析该字幕类型
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        bool Accept(string typeName);

        /// <summary>
        /// 解析字幕
        /// </summary>
        /// <param name="subtitle"></param>
        /// <returns></returns>
        IEnumerable<Sentence> Parse(string subtitle);
    }
}
