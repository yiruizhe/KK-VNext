namespace KK.Commons;

public static class IdHelper
{
    public static string NextId => GuidTo16String();

    /// <summary>
    /// 根据Guid生成16位字符串
    /// </summary>
    /// <returns></returns>
    private static string GuidTo16String()
    {
        long i = 1;
        foreach (byte b in Guid.NewGuid().ToByteArray())
            i *= ((int)b + 1);
        return string.Format("{0:x}", i - DateTime.Now.Ticks);
    }
}
