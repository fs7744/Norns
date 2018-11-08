using System;

namespace TestFuncToDll
{
    [TestInterceptor]
    public interface ISyncFunc
    {
        void SyncCallNoParameters();
    }

    public class SyncFunc : ISyncFunc
    {
        public void SyncCallNoParameters()
        {
            (3 + 4).ToString();
        }
    }
}
