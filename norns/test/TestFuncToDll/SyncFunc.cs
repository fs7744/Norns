using System;

namespace TestFuncToDll
{
    public class SyncFunc : ISyncFunc
    {
        [TestInterceptor]
        public void SyncCallNoParameters()
        {
            (3 + 4).ToString();
        }
    }

    [Norns.AOP.Attributes.NoIntercept]
    public class SyncFunc2 : ISyncFunc2
    {
        public void SyncCallNoParameters()
        {
            (3 + 4).ToString();
        }
    }
}

// SyncFunc
//using Norns.AOP.Interceptors;
//using Norns.DependencyInjection;
//using System;
//using System.Reflection;
//using TestFuncToDll;

//public class SyncFunc : ISyncFunc
//{
//    private IInterceptDelegateBuilder interceptDelegateBuilder_267b6c6d32e6410b8fdce762f385e819;

//    private static readonly MethodInfo f_SyncCallNoParameters_b829f3d8ec6749c39247b4264318ac71;

//    private InterceptDelegate f_Call_SyncCallNoParameters_b829f3d8ec6749c39247b4264318ac71;

//    [FromDI]
//    public IInterceptDelegateBuilder InterceptDelegateBuilder_8f4ed8e93b1644e2a0b10f2d374dd124
//    {
//        get => interceptDelegateBuilder_267b6c6d32e6410b8fdce762f385e819;
//        set
//        {
//            interceptDelegateBuilder_267b6c6d32e6410b8fdce762f385e819 = value;
//            f_Call_SyncCallNoParameters_b829f3d8ec6749c39247b4264318ac71 = value.BuildInterceptDelegate(f_SyncCallNoParameters_b829f3d8ec6749c39247b4264318ac71, Call_SyncCallNoParameters_b829f3d8ec6749c39247b4264318ac71);
//        }
//    }

//    [TestInterceptor]
//    public void SyncCallNoParameters()
//    {
//        InterceptContext context = new InterceptContext();
//        context.ServiceMethod = f_SyncCallNoParameters_b829f3d8ec6749c39247b4264318ac71;
//        context.Additions = new Additions();
//        object[] array = context.Parameters = new object[0];
//        f_Call_SyncCallNoParameters_b829f3d8ec6749c39247b4264318ac71(context);
//    }

//    static SyncFunc()
//    {
//        Type typeFromHandle = typeof(SyncFunc);
//        f_SyncCallNoParameters_b829f3d8ec6749c39247b4264318ac71 = typeFromHandle.GetMethod("SyncCallNoParameters", new Type[0]);
//    }

//    public void SyncCallNoParameters_b829f3d8ec6749c39247b4264318ac71()
//    {
//        7.ToString();
//    }

//    private void Call_SyncCallNoParameters_b829f3d8ec6749c39247b4264318ac71(InterceptContext context)
//    {
//        SyncCallNoParameters_b829f3d8ec6749c39247b4264318ac71();
//    }
//}