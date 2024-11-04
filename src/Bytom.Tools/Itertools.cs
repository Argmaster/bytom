using System.Collections;

namespace Bytom.Tools
{
    public static class Itertools
    {
        public static IEnumerable yieldVoidTimes(long count)
        {
            for (long i = 0; i < count; i++)
            {
                yield return null;
            }
        }
        public static void exhaust(IEnumerable enumerable)
        {
            foreach (var _ in enumerable)
            { }
        }
        public static void saturate(IEnumerator enumerable)
        {
            while (enumerable.MoveNext())
            { }
        }
    }
}