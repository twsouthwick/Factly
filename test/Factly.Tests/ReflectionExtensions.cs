// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if FEATURE_TYPEINFO
using System;
using System.Reflection;

namespace Factly
{
    internal static class ReflectionExtensions
    {
        public static PropertyInfo GetProperty(this Type type, string name)
        {
            return type.GetTypeInfo().GetProperty(name);
        }
    }
}
#endif
