// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Factly
{
    internal static class ValidatorBuilderTestExtensions
    {
        private static readonly IConstraint _constraint = new DelegateConstraint(() => true);

        public static void AddEmptyClass(this ValidatorBuilder builder)
        {
            builder.AddKnownType<SimpleWithConstraint>();
        }

        public static void AddEmptyConstraint(this ValidatorBuilder builder, bool withType = false)
        {
            if (withType)
            {
                builder.AddKnownType<SimpleWithConstraint>();
                builder.AddConstraint(p =>
                {
                    if (p == typeof(SimpleWithConstraint).GetProperty(nameof(SimpleWithConstraint.Test)))
                    {
                        return _constraint;
                    }

                    return null;
                });
            }
            else
            {
                builder.AddConstraint(_ => _constraint);
            }
        }

        private class SimpleWithConstraint
        {
            public string Test { get; set; }
        }
    }
}
