// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Linq;

namespace Factly
{
    [DebuggerDisplay("{Type.FullName,nq}")]
    internal readonly struct TypeValidator
    {
        public TypeValidator(Type type, ValidatorBuilder builder)
        {
            Type = type;
            Properties = type.GetProperties()
                .Select(p => PropertyValidator.Create(p, builder))
                .Where(p => p.Property != null)
                .ToArray();
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Type Type { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public PropertyValidator[] Properties { get; }
    }
}
