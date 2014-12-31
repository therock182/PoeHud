using System;
using System.Threading;

namespace PoeHUD.Framework.Helpers
{
    public static class ActionHelper
    {
        public static void SafeInvoke<T>(this Action<T> action, T parameter)
        {
            SafeInvoke(() => action(parameter));
        }

        public static void SafeInvoke(this Action action)
        {
            action = Volatile.Read(ref action);
            if (action != null)
            {
                action();
            }
        }

        public static void ThrowIf<TException>(bool condition)
            where TException : Exception, new()
        {
            if (condition)
            {
                throw new TException();
            }
        }

        public static bool TryInvoke(this Action action)
        {
            try
            {
                action();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}