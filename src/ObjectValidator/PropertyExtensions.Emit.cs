using System;
using System.Reflection;
using System.Reflection.Emit;

namespace ObjectValidator
{
    internal static class PropertyExtensions
    {
        public static Func<object, object> GetPropertyDelegate(this PropertyInfo property)
        {
            var callingClass = property.DeclaringType;
            var method = new DynamicMethod("ValidatorFastPropertyGetter", property.PropertyType, new[] { typeof(object) }, callingClass, true);

            var il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, callingClass);
            il.Emit(OpCodes.Callvirt, property.GetGetMethod(true));
            il.Emit(OpCodes.Ret);

            return (Func<object, object>)method.CreateDelegate(typeof(Func<object, object>));
        }
    }
}
