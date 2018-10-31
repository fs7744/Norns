using BenchmarkDotNet.Attributes;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System;

namespace UtilsTest
{
    [AllStatisticsColumn, MemoryDiagnoser]
    public class TestUtilsTest
    {
        public TestUtilsTest()
        {
            
        }

        [Benchmark]
        public void ConcurrentDictionary()
        {
            var dic = new Dictionary<MemberInfo, Func<MemberInfo, MemberInfo>>();
        }

        [Benchmark]
        public void Dictionary()
        {
            var dic = new Dictionary<MemberInfo, Func<MemberInfo, MemberInfo>>(1);
            //foreach (var item in typeof(Type).GetMethods())
            //{
            //    dic.Add(item, i => i);
            //}
            //dic.TryGetValue(typeof(Type).GetMethod("GetTypeArray"), out Func<MemberInfo, MemberInfo> a);
        }
    }
}