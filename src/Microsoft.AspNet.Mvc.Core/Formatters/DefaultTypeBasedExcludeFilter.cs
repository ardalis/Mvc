// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace Microsoft.AspNet.Mvc
{
    /// <summary>
    /// Provides an implementation of <see cref="IExcludeTypeFromBodyValidationFilter"/> which can filter 
    /// based on <see cref="T"/> parameter.
    /// </summary>
    /// <typeparam name="T">Represents a type that needs to be filtered.</typeparam>
    public class DefaultTypeBasedExcludeFilter<T> : IExcludeTypeFromBodyValidationFilter
    {
        public bool IsTypeExcluded([NotNull] Type propertyType)
        {
            return typeof(T).IsAssignableFrom(propertyType);
        }    
    }
}
