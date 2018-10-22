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
            Constraints = GetConstraints(type, context);
        }

        public Type Type { get; }

        public ReadonlyArray<PropertyValidator> Properties { get; }

        public ReadonlyArray<IConstraint> Constraints { get; }

        private static ReadonlyArray<PropertyValidator> GetPropertyValidators(Type type, BuilderContext context)
        {
            var properties = type.GetProperties();
            var array = new ArrayBuilder<PropertyValidator>(properties.Length);

            foreach (var property in properties)
            {
                if (PropertyValidator.Create(property, context) is PropertyValidator validator)
                {
                    array.Add(validator);
                }
            }

            return array.Build();
        }

        private static ReadonlyArray<IConstraint> GetConstraints(Type type, BuilderContext context)
        {
            var array = new ArrayBuilder<IConstraint>(context.Builder.Constraints.Count);

            foreach (var builder in context.Builder.Constraints)
            {
                if (builder.Create(type, context) is IConstraint constraint)
                {
                    array.Add(constraint);
                }
            }

            return array.Build();
        }
    }
}
