// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace Microsoft.AspNet.Mvc
{
    /// <summary>
    /// Provides an activated collection of <see cref="IExcludeTypeFromBodyValidationFilter"/> instances.
    /// </summary>
    public interface IBodyValidationExcludeFiltersProvider
    {
        /// <summary>
        /// Gets a collection of activated <see cref="IExcludeTypeFromBodyValidationFilter"/> instances.
        /// </summary>
        IReadOnlyList<IExcludeTypeFromBodyValidationFilter> ExcludeFilters { get; }
    }
}
