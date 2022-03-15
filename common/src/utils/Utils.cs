using System.Runtime.CompilerServices;

namespace common.utils
{
    public static class Utils
    {
        public static int SizeOf<T>(T var)
        {
            return Unsafe.SizeOf<T>();
        }
    }
}