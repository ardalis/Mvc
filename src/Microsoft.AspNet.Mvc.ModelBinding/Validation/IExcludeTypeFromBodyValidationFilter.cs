// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNet.Mvc.ModelBinding
{
    /// <summary>
    /// Provides an interface which can be used to decide to exclude a type from body model validation.
    /// </summary>
    public interface IExcludeTypeFromBodyValidationFilter
    {
        bool IsTypeExcluded([NotNull] Type propertyType);     
    }
}
