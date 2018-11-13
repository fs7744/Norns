using System;

namespace TestFuncToDll
{
    public interface ISyncFunc
    {
        void SyncCallNoParameters();
    }

    public class SyncFunc : ISyncFunc
    {
        [TestInterceptor]
        public void SyncCallNoParameters()
        {
            (3 + 4).ToString();
        }
    }
}
