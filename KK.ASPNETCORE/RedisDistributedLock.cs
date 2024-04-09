using StackExchange.Redis;

namespace KK.ASPNETCORE
{
    public class RedisDistributedLock
    {
        private readonly IDatabase db;

        public RedisDistributedLock(IDatabase database)
        {
            this.db = database;
        }

        /// <summary>
        /// 尝试获取锁
        /// </summary>
        /// <param name="lockKey">key</param>
        /// <param name="expireTime">锁的过期时间</param>
        /// <param name="retryCount">重试次数</param>
        /// <param name="retryDelay">重试等待延迟 (毫秒)</param>
        /// <returns></returns>
        public async Task<bool> AcquireLockAsync(string lockKey, TimeSpan expireTime, int retryCount = 3, int retryDelay = 100)
        {
            for (int i = 0; i < retryCount; i++)
            {
                if (await db.StringSetAsync(lockKey, 1, expiry: expireTime, when: When.NotExists))
                {
                    return true;
                }
                await Task.Delay(retryDelay);
            }
            return false;
        }

        public void ReleaseLock(string lockKey)
        {
            db.KeyDelete(lockKey);
        }
    }
}
