// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Factly.Collections;
using System;
using System.Diagnostics;

namespace Factly
{
    [DebuggerDisplay("{Type.FullName,nq}")]
    internal readonly struct TypeValidator<TState>
    {
        public TypeValidator(Type type, BuilderContext<TState> context)
        {
            Type = type;
            Properties = GetPropertyValidators(type, context);
            Constraints = GetConstraints(type, context);
        }

        public Type Type { get; }

        public ReadonlyArray<PropertyValidator<TState>> Properties { get; }

        public ReadonlyArray<IConstraint<TState>> Constraints { get; }

        private static ReadonlyArray<PropertyValidator<TState>> GetPropertyValidators(Type type, BuilderContext<TState> context)
        {
            var properties = type.GetInstanceProperties();
            var array = new ArrayBuilder<PropertyValidator<TState>>(properties.Length);

            foreach (var property in properties)
            {
                if (PropertyValidator<TState>.Create(property, context) is PropertyValidator<TState> validator)
                {
                    array.Add(validator);
                }
            }

            return array.Build();
        }

        private static ReadonlyArray<IConstraint<TState>> GetConstraints(Type type, BuilderContext<TState> context)
        {
            var array = new ArrayBuilder<IConstraint<TState>>(context.Builder.Constraints.Count);

            foreach (var builder in context.Builder.Constraints)
            {
                if (builder.Create(type, context) is IConstraint<TState> constraint)
                {
                    array.Add(constraint);
                }
            }

            return array.Build();
        }
    }
}
