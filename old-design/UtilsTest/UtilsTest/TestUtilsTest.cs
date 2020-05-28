using BenchmarkDotNet.Attributes;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System;
using BenchmarkDotNet.Engines;
using System.Linq;

namespace UtilsTest
{
    [SimpleJob(launchCount: 1, warmupCount: 3, targetCount: 3)]
    [AllStatisticsColumn, MemoryDiagnoser]
    public class TestUtilsTest
    {
        private readonly List<int> list;
        private readonly LinkedList<int> linkedList;

        public TestUtilsTest()
        {
            list = new List<int>();
            linkedList  = new LinkedList<int>();
            for (int i = 0; i < 100; i++)
            {
                list.Add(i);
                linkedList.Add(i);
            }
        }

        //[Benchmark]
        //public void ConcurrentDictionary()
        //{
        //    var dic = new Dictionary<MemberInfo, Func<MemberInfo, MemberInfo>>();
        //}

        //[Benchmark]
        //public void Dictionary()
        //{
        //    var dic = new Dictionary<MemberInfo, Func<MemberInfo, MemberInfo>>(1);
        //    //foreach (var item in typeof(Type).GetMethods())
        //    //{
        //    //    dic.Add(item, i => i);
        //    //}
        //    //dic.TryGetValue(typeof(Type).GetMethod("GetTypeArray"), out Func<MemberInfo, MemberInfo> a);
        //}

        //[Benchmark]
        //public void ListAdd()
        //{
        //    list.Add(33);
        //}

        //[Benchmark]
        //public void LinkedListAdd()
        //{
        //    linkedList.Add(33);
        //}

        //[Benchmark]
        //public void ListForeach()
        //{
        //    foreach (var item in list)
        //    {
        //        item.ToString();
        //    }
        //}

        //[Benchmark]
        //public void LinkedListForeach()
        //{
        //    foreach (var item in linkedList)
        //    {
        //        item.ToString();
        //    }
        //}

        [Benchmark]
        public void ListLast()
        {
            list.Last().ToString();
        }

        [Benchmark]
        public void LinkedListForeach()
        {
            linkedList.Last.ToString();
        }
    }

    internal static class LinkedListExtensions
    {
        internal static LinkedList<T> Add<T>(this LinkedList<T> linkedList, T value)
        {
            if (linkedList.Count == 0)
            {
                linkedList.AddFirst(value);
            }
            else
            {
                linkedList.AddAfter(linkedList.Last, value);
            }

            return linkedList;
        }
    }
}