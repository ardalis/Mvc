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
        /// Adds a delegate to the specified <paramref name="excludeFromValidationPredicateCollection" />
        /// that excludes the properties of the <see cref="Type"/> specified and it's derived types from validaton.
        /// </summary>
        /// <param name="excludeFromValidationPredicateCollection">A list of <see cref="ExcludeFromValidationDelegate"/>
        /// which are applied to filter model properties while validation.</param>
        /// <param name="type"><see cref="Type"/> which should be excluded from validation.</param>
        public static void Add(this IList<ExcludeFromValidationDelegate> excludeFromValidationPredicateCollection,
                               Type type)
        {
            excludeFromValidationPredicateCollection.Add(t => type.IsAssignableFrom(t));
        }

        /// <summary>
        /// Adds a delegate to the specified <paramref name="excludeFromValidationPredicateCollection" />
        /// that excludes the properties of the type specified and it's derived types from validaton.
        /// </summary>
        /// <param name="excludeFromValidationPredicateCollection">A list of <see cref="ExcludeFromValidationDelegate"/>
        /// which are applied to filter model properties while validation.</param>
        /// <param name="typeFullName">Full name of the type which should be excluded from validation.</param>
        public static void Add(this IList<ExcludeFromValidationDelegate> excludeFromValidationPredicateCollection,
                               string typeFullName)
        {
            excludeFromValidationPredicateCollection.Add(t => CheckIfTypeNameMatches(t, typeFullName));
        }

        private static bool CheckIfTypeNameMatches(Type t, string typeFullName)
        {
            if (t == null)
            {
                return false;
            }

            if (string.Equals(t.FullName, typeFullName, StringComparison.Ordinal))
            {
                return true;
            }

            return CheckIfTypeNameMatches(t.BaseType, typeFullName);
        }
    }
}