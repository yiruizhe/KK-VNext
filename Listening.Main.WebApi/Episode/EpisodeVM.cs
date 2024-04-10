namespace Listening.Main.WebApi.Episode
{
    public record EpisodeVM(string Id,string Name,string AlbumId,Uri AudioUrl,
        double DurationInSecond,IEnumerable<SentenceVM>? Sentences)
    {

        public static EpisodeVM? Create(Domain.Entities.Episode? e,bool isLoadSentences)
        {
            if(e == null)
            {
                return null;
            }
            List<SentenceVM> sentences = new();
            if (isLoadSentences)
            {
                var subtitle = e.ParseSubtitle();
                foreach (var item in subtitle)
                {
                    sentences.Add(new SentenceVM(item.StartTime.TotalSeconds,item.EndTime.TotalSeconds, item.Value));
                }
            }
            return new EpisodeVM(e.Id, e.Name, e.AlbumId, e.AudioUrl, e.DurationInSecond, sentences);
        }

        public static EpisodeVM[] Create(Domain.Entities.Episode[] es,bool isLoadSentences = false)
        {
            return es.Select(e => EpisodeVM.Create(e, isLoadSentences)!).ToArray();
        }


    }
}
