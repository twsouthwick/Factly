// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;

namespace Factly
{
    [DebuggerDisplay("{Type.FullName,nq}")]
    internal readonly struct TypeValidator
    {
        public TypeValidator(Type type, ValidatorBuilder builder)
        {
            Type = type;
            Properties = GetPropertyValidators(type, builder);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Type Type { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public PropertyValidator[] Properties { get; }

        private static PropertyValidator[] GetPropertyValidators(Type type, ValidatorBuilder builder)
        {
            var array = default(ArrayBuilder<PropertyValidator>);

            foreach (var property in type.GetProperties())
            {
                var validator = PropertyValidator.Create(property, builder);

                if (validator.Property != null)
                {
                    array.Add(validator);
                }
            }

            return array.ToArray();
        }
    }
}
