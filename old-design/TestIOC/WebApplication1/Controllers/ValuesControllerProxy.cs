using AsyncInterceptor;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestIOC;

namespace WebApplication1.Controllers
{
    [Replace(typeof(ValuesController))]
    public class ValuesControllerProxy : ValuesController
    {
        private readonly AopAttribute interceptor;

        public ValuesControllerProxy(AopAttribute interceptor)
        {
            this.interceptor = interceptor;
        }

        private void GetReal(Context context)
        {
            context.Result = base.Get();
        }

        public override ActionResult<IEnumerable<string>> Get()
        {
            var context = new Context()
            {
                Parameters = new object[0]
            };
            interceptor.OnInvokeSync(context, GetReal);
            return (ActionResult<IEnumerable<string>>)context.Result;
        }
    }
}
