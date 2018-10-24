// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Factly
{
    internal static class ValidatorBuilderTestExtensions
    {
        public static void AddEmptyClass<TState>(this ValidatorBuilder<TState> builder)
        {
            builder.AddKnownType<SimpleWithConstraint>();
        }

        public static void AddEmptyConstraint<TState>(this ValidatorBuilder<TState> builder, bool withType = false)
        {
            if (withType)
            {
                builder.AddKnownType<SimpleWithConstraint>();
                builder.AddConstraint(p =>
                {
                    if (p == typeof(SimpleWithConstraint).GetProperty(nameof(SimpleWithConstraint.Test)))
                    {
                        return new DelegateConstraint<TState>(() => true);
                    }

                    return null;
                });
            }
            else
            {
                builder.AddConstraint(_ => new DelegateConstraint<TState>(() => true));
            }
        }

        private class SimpleWithConstraint
        {
            public string Test { get; set; }
        }
    }
}
