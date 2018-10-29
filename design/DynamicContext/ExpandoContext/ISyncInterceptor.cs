using System;

namespace ExpandoContext
{
    public interface ISyncInterceptor
    {
        void OnInvokeSync(Context context, Action<Context> next);
    }
}