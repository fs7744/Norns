using System;

namespace GenericContext
{
    public interface ISyncInterceptor
    {
        void OnInvokeSync(Context<Tuple<int, int>, int> context, Action<Context<Tuple<int, int>, int>> next);
    }
}