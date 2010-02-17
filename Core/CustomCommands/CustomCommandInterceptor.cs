using System;
using System.IO;
using Bricks.RuntimeFramework;
using Castle.Core.Interceptor;
using White.Core.UIItems;

namespace White.Core.CustomCommands
{
    public class CustomCommandInterceptor : IInterceptor
    {
        private readonly UIItem uiItem;
        private static readonly Method doMethod;

        static CustomCommandInterceptor()
        {
            var uiItemClass = new Class(typeof (UIItem));
            doMethod = uiItemClass.GetMethod("Do");
        }

        public CustomCommandInterceptor(UIItem uiItem)
        {
            if (uiItem == null) throw new ArgumentNullException();
            this.uiItem = uiItem;
        }

        public virtual void Intercept(IInvocation invocation)
        {
            if (uiItem.AutomationElement == null) throw new NullReferenceException("AutomationElement in this UIItem is null");
            Type type = invocation.Method.DeclaringType;
            string assemblyFileName = new FileInfo(type.Assembly.Location).Name;
            object returnValue = doMethod.Invoke(uiItem, new object[] {assemblyFileName, type.FullName, invocation.Method.Name, invocation.Arguments});
            var exception = returnValue as Exception;
            if (exception != null) throw exception;
            invocation.ReturnValue = returnValue;
        }
    }
}