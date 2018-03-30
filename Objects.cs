using System;
using System.Web.Script.Serialization;

namespace G.Extensions
{
    public static class ObjectsExtensions
    {
        public static string SerializeAnonymousType(Type obj)
        {
            var o = Activator.CreateInstance(obj);
            var result = new JavaScriptSerializer().Serialize(o);
            return result;
        }
    }
}
