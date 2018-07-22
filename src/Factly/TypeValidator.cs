// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Factly.Collections;
using System;
using System.Diagnostics;

namespace Factly
{
    [DebuggerDisplay("{Type.FullName,nq}")]
    internal readonly struct TypeValidator
    {
        public TypeValidator(Type type, BuilderContext context)
        {
            Type = type;
            Properties = GetPropertyValidators(type, context);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Type Type { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public PropertyValidator[] Properties { get; }

        private static PropertyValidator[] GetPropertyValidators(Type type, BuilderContext context)
        {
            var array = default(ArrayBuilder<PropertyValidator>);

            foreach (var property in type.GetProperties())
            {
                var validator = PropertyValidator.Create(property, context);

                if (validator != null)
                {
                    array.Add(validator);
                }
            }

            return array.ToArray();
        }
    }
}
