// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace Microsoft.AspNet.Mvc
{
    /// <summary>
    /// Provides an implementation of <see cref="IExcludeTypeFromBodyValidationFilter"/> which can filter 
    /// based on type full name.
    /// </summary>
    public class DefaultTypeNameBasedExcludeFilter : IExcludeTypeFromBodyValidationFilter
    {
        private readonly string _registeredTypeFullName;

        public DefaultTypeNameBasedExcludeFilter([NotNull] string typeFullName)
        {
            _registeredTypeFullName = typeFullName;
        }

        public bool IsTypeExcluded([NotNull] Type propertyType)
        {
            return CheckIfTypeNameMatches(propertyType);
        }

        private bool CheckIfTypeNameMatches(Type t)
        {
            if (t == null)
            {
                return false;
            }

            if (string.Equals(t.FullName, _registeredTypeFullName, StringComparison.Ordinal))
            {
                return true;
            }

            return CheckIfTypeNameMatches(t.BaseType());
        }
    }
}
