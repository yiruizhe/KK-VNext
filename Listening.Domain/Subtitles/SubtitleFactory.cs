using Listening.Domain.SubTitles;

namespace Listening.Domain.Subtitles
{
    static class SubtitleFactory
    {
        private static List<ISubtitleParser> parsers = new();

        /// <summary>
        /// 将本程序集下的所有ISubtitle类型加入到集合中
        /// </summary>
        static SubtitleFactory()
        {
            var parserTypes = typeof(SubtitleFactory).Assembly.GetTypes()
                .Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(ISubtitleParser)));
            foreach (var item in parserTypes)
            {
                ISubtitleParser parser = (ISubtitleParser)Activator.CreateInstance(item);
                parsers.Add(parser);
            }
        }

        /// <summary>
        /// 从和集合中获取一个可以解析该字幕类型的解析器，没有返回null
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static ISubtitleParser? GetParser(string typeName)
        {
            foreach (var parser in parsers)
            {
                if (parser.Accept(typeName))
                {
                    return parser;
                }
            }
            return null;
        }
    }
}
