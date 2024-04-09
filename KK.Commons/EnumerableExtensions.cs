

namespace KK.Commons
{
    public static class EnumerableExtensions
    {
        public static bool SequenceIgnoreEquals<T>(this IEnumerable<T> item1, IEnumerable<T> item2)
        {
            if (item1 == item2)
                return true;
            if (item2 == null || item1 == null)
                return false;
            return item1.OrderBy(s => s).SequenceEqual(item2.OrderBy(s => s));
        }
    }
}
