using System;

namespace DynamicContext
{
    public interface ISyncInterceptor
    {
        void OnInvokeSync(Context context, Action<Context> next);
    }
}