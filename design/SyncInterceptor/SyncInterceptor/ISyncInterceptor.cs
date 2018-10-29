using System;

namespace SyncInterceptor
{
    public interface ISyncInterceptor
    {
        void OnInvokeSync(Context context, Action<Context> next);
    }
}