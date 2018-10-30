using System;

namespace AsyncInterceptor
{
    public interface ISyncInterceptor
    {
        void OnInvokeSync(Context context, Action<Context> next);
    }
}