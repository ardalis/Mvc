// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace Microsoft.AspNet.Mvc
{
    /// <summary>
    /// Extensions for <see cref="MvcOptions.ExcludeFromValidationDelegates"/>.
    /// </summary>
    public static class ExcludeFromValidationDelegateExtensions
    {
        /// <summary>
        /// Adds a delegate to the specified <paramref name="list" />
        /// that exludes the properties of the <see cref="Type"/> specified and it's derived types from validaton.
        /// </summary>
        /// <param name="list"><see cref="IList{T}"/> of <see cref="ExcludeFromValidationDelegate"/>.</param>
        /// <param name="type"><see cref="Type"/> which should be excluded from validation.</param>
        public static void Add(this IList<ExcludeFromValidationDelegate> list, Type type)
        {
            list.Add(t => type.IsAssignableFrom(t));
        }

        /// <summary>
        /// Adds a delegate to the specified <paramref name="list" />
        /// that exludes the properties of the type specified and it's derived types from validaton.
        /// </summary>
        /// <param name="list"><see cref="IList{T}"/> of <see cref="ExcludeFromValidationDelegate"/>.</param>
        /// <param name="typeName">Name of the type which should be excluded from validation.</param>
        public static void Add(this IList<ExcludeFromValidationDelegate> list, string typeName)
        {
            list.Add(t => CheckIfTypeNameMatches(t, typeName));
        }

        private static bool CheckIfTypeNameMatches(Type t, string typeName)
        {
            if (t == null)
            {
                return false;
            }

            if (string.Equals(t.Name, typeName, StringComparison.Ordinal))
            {
                return true;
            }

            return CheckIfTypeNameMatches(t.BaseType, typeName);
        }
    }
}