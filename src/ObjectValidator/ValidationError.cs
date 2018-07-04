// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace ObjectValidator
{
    public class ValidationError
    {
        internal ValidationError(object instance, PropertyInfo property)
        {
            Instance = instance;
            Property = property;
        }

        public object Instance { get; }

        public PropertyInfo Property { get; }
    }
}
