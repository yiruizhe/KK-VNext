namespace Listening.Admin.WebApi.Episodes
{
    public record EncodingEpisodeInfo(string Id, string Name, string Subtitle,
        string SubtitleType, string AlbumId, double DurationInSecond, string Status);
}
