//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Norns.DependencyInjection
//{
//    internal class ServiceProviderEngineScope : IServiceScope, IServiceProvider
//    {
//        public ServiceProviderEngine Engine { get; }

//        public ServiceProviderEngineScope(ServiceProviderEngine engine)
//        {
//            this.Engine = engine;
//        }

//        public IServiceProvider ServiceProvider => this;

//        public object GetService(Type serviceType)
//        {
//            return Engine.GetService(serviceType, this);
//        }

//        #region IDisposable Support
//        private bool disposedValue = false; 

//        protected virtual void Dispose(bool disposing)
//        {
//            if (!disposedValue)
//            {
//                if (disposing)
//                {
//                    // TODO: 释放托管状态(托管对象)。
//                }

//                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
//                // TODO: 将大型字段设置为 null。

//                disposedValue = true;
//            }
//        }

//        public void Dispose()
//        {
//            Dispose(true);
//        }
//        #endregion
//    }
//}
