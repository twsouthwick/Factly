﻿// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if NO_CANCELLATION_TOKEN
using System;

namespace ObjectValidator
{
    internal readonly struct CancellationToken
    {
        private readonly ValidationContext _context;

        public CancellationToken(ValidationContext context)
        {
            _context = context;
        }

        public void ThrowIfCancellationRequested()
        {
            if (_context == null)
            {
                return;
            }

            if (_context.IsCancelled)
            {
                throw new OperationCanceledException();
            }
        }
    }
}
#endif

