using Norns.AOP.Attributes;
using Norns.AOP.Interceptors;
using System;
using System.Threading.Tasks;

namespace TestFuncToDll
{
    public class TestInterceptorAttribute : InterceptorBaseAttribute
    {
        public override void Intercept(InterceptContext context, InterceptDelegate next)
        {
            context.ToString();
            //Console.WriteLine("Begin");
            next(context);
            //Console.WriteLine("End");
        }

        public override async Task InterceptAsync(InterceptContext context, AsyncInterceptDelegate nextAsync)
        {
            Console.WriteLine("Async Begin");
            await nextAsync(context);
            Console.WriteLine("Async End");
        }
    }


    //public class TestInterceptor : InterceptorBase
    //{
    //    public override void Intercept(InterceptContext context, InterceptDelegate next)
    //    {
    //        Console.WriteLine("Begin");
    //        next(context);
    //        Console.WriteLine("End");
    //    }

    //    public override async Task InterceptAsync(InterceptContext context, AsyncInterceptDelegate nextAsync)
    //    {
    //        Console.WriteLine("Async Begin");
    //        await nextAsync(context);
    //        Console.WriteLine("Async End");
    //    }
    //}
}