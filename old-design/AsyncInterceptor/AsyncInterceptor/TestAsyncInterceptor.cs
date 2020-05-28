using BenchmarkDotNet.Attributes;
using System;
using System.Threading.Tasks;

namespace AsyncInterceptor
{
    [AllStatisticsColumn]
    public class TestAsyncInterceptor
    {
        private readonly int x = 40;
        private readonly int y = 4;
        private readonly ITestService noInterceptor = new TestService();
        private readonly ITestService handwritingInterceptor = new TestServiceHandWriting(new TestService());
        private readonly TestServiceProxy asyncInterceptor = new TestServiceProxy(new TestService(), new TestInterceptor());
        private readonly TestServiceProxy syncBaseOnAsyncInterceptor = new TestServiceProxy(new TestService(), new TestInterceptor_SyncBaseOnAsync());

        [Benchmark]
        public void SyncMethod_HasParam_NoInterceptor()
        {
            noInterceptor.Sum(x, y);
        }

        [Benchmark]
        public void SyncMethod_HasParam_HandWritingInterceptor()
        {
            handwritingInterceptor.Sum(x, y);
        }

        [Benchmark]
        public void SyncMethod_HasParam_ProxyAndInterceptor()
        {
            asyncInterceptor.Sum(x, y);
        }

        [Benchmark]
        public async Task AsyncMethod_HasParam_NoInterceptor()
        {
            await noInterceptor.SumAsync(x, y);
        }

        [Benchmark]
        public async Task AsyncMethod_HasParam_HandWritingInterceptor()
        {
            await handwritingInterceptor.SumAsync(x, y);
        }

        [Benchmark]
        public async Task AsyncMethod_HasParam_NoAwait_ProxyAndInterceptor()
        {
            await asyncInterceptor.NoAwaitSumAsync(x, y);
        }

        [Benchmark]
        public async Task AsyncMethod_HasParam_ProxyAndInterceptor()
        {
            await asyncInterceptor.SumAsync(x, y);
        }

        [Benchmark]
        public void SyncMethod_HasParam_SyncBaseOnAsync_ProxyAndInterceptor()
        {
            syncBaseOnAsyncInterceptor.Sum(x, y);
        }
    }
}