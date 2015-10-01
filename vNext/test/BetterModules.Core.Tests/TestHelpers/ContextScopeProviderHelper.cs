﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Autofac;
//using BetterModules.Core.Dependencies;

//namespace BetterModules.Core.Tests.TestHelpers
//{
//    public class ContextScopeProviderHelper : IDisposable
//    {
//        private IDictionary<Type, Type> originalObjects = new Dictionary<Type, Type>();

//        public void RegisterFakeService(Type type, Type interfaceType)
//        {
//            using (var container = ContextScopeProvider.CreateChildContainer())
//            {
//                var service = container.Resolve(interfaceType);
//                if (!originalObjects.ContainsKey(interfaceType))
//                {
//                    originalObjects.Add(interfaceType, service.GetType());
//                }
//            }

//            var builder = new ContainerBuilder();
//            builder.RegisterType(type).As(interfaceType).InstancePerLifetimeScope();

//            ContextScopeProvider.RegisterTypes(builder);
//        }
        
//        public void RegisterFakeServiceInstance(Object type, Type interfaceType)
//        {
//            using (var container = ContextScopeProvider.CreateChildContainer())
//            {
//                var service = container.Resolve(interfaceType);
//                if (!originalObjects.ContainsKey(interfaceType))
//                {
//                    originalObjects.Add(interfaceType, service.GetType());
//                }
//            }

//            var builder = new ContainerBuilder();
//            builder.RegisterInstance(type).As(interfaceType).SingleInstance();

//            ContextScopeProvider.RegisterTypes(builder);
//        }

//        public void Dispose()
//        {
//            if (!originalObjects.Any())
//            {
//                return;
//            }

//            var builder = new ContainerBuilder();
//            foreach (var keyValue in originalObjects)
//            {
//                builder.RegisterType(keyValue.Value).As(keyValue.Key).InstancePerLifetimeScope();
//            }

//            ContextScopeProvider.RegisterTypes(builder);
//        }
//    }
//}
