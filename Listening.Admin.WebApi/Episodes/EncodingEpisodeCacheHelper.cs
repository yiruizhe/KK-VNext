using KK.Commons;
using StackExchange.Redis;

namespace Listening.Admin.WebApi.Episodes;

public class EncodingEpisodeCacheHelper
{
    private readonly IConnectionMultiplexer redisConn;

    public EncodingEpisodeCacheHelper(IConnectionMultiplexer redisConn)
    {
        this.redisConn = redisConn;
    }

    // 获取保存这个albumId中所有的转码中的episodeid
    private string GetKeyForEpisodesOfAlbum(string albumId) => $"Listening.EncodingEpisodesIdsOfAlbum.{albumId}";
    // 获取Episode的key
    private string GetStatusKeyForEpisode(string episodeId) => $"Listening.EncodingEpisode.{episodeId}";

    // 增加待转码任务的详细信息
    public async Task AddEncodingEpisodeAsync(string episodeId, EncodingEpisodeInfo info)
    {
        string key = GetStatusKeyForEpisode(episodeId);
        var db = redisConn.GetDatabase();
        await db.StringSetAsync(key, info.ToJsonString());
        string keyForEncodingEpisodesOfAlbum = GetKeyForEpisodesOfAlbum(info.AlbumId);
        await db.SetAddAsync(keyForEncodingEpisodesOfAlbum, episodeId);
    }

    // 获取这个album下的所有的转码任务
    public async Task<IEnumerable<string>> GetEncodingEpisodesIdsAsync(string album)
    {
        string key = GetKeyForEpisodesOfAlbum(album);
        var db = redisConn.GetDatabase();
        var res = await db.SetMembersAsync(key);
        return res.Select(r => r.ToString());
    }

    // 删除一个episode任务
    public async Task RemoveEncodingEpisodeAsync(string episodeId, string albumId)
    {
        var db = redisConn.GetDatabase();
        await db.KeyDeleteAsync(GetStatusKeyForEpisode(episodeId));
        await db.SetRemoveAsync(GetKeyForEpisodesOfAlbum(albumId), episodeId);
    }

    // 修改一个episode的状态
    public async Task UpdateEncodingEpisodeStatusAsync(string episodeId, string status)
    {
        string key = GetStatusKeyForEpisode(episodeId);
        var db = redisConn.GetDatabase();
        string valueJson = await db.StringGetAsync(key);
        var info = valueJson.ParseJson<EncodingEpisodeInfo>();
        info = info with { Status = status };
        await db.StringSetAsync(key, info.ToJsonString());
    }

    // 获取一个episode 的转码状态
    public async Task<EncodingEpisodeInfo> GetEncodingEpisodeAsync(string episodeId)
    {
        string key = GetStatusKeyForEpisode(episodeId);
        var db = redisConn.GetDatabase();
        string valueJson = await db.StringGetAsync(key);
        return valueJson.ParseJson<EncodingEpisodeInfo>()!;
    }

}
