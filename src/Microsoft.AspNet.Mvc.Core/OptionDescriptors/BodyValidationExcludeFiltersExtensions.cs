// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNet.Mvc.OptionDescriptors;

namespace Microsoft.AspNet.Mvc
{
    /// <summary>
    /// Extensions for <see cref="MvcOptions.BodyValidationExcludeFilters"/>.
    /// </summary>
    public static class BodyValidationExcludeFiltersExtensions
    {
        /// <summary>
        /// Adds a descriptor to the specified <paramref name="excludeBodyValidationDescriptorCollection" />
        /// that excludes the properties of the <see cref="Type"/> specified and it's derived types from validaton.
        /// </summary>
        /// <param name="excludeBodyValidationDescriptorCollection">A list of <see cref="ExcludeBodyValidationDescriptor"/>
        /// which are used to get a collection of exclude filters to be applied for filtering model properties during validation.
        /// </param>
        /// <param name="type"><see cref="Type"/> which should be excluded from validation.</param>
        public static void Add(this IList<ExcludeBodyValidationDescriptor> excludeBodyValidationDescriptorCollection,
                               Type type)
        {
            var genericType = typeof(DefaultTypeBasedExcludeFilter<>).MakeGenericType(type);
            excludeBodyValidationDescriptorCollection.Add(new ExcludeBodyValidationDescriptor(genericType));
        }

        /// <summary>
        /// Adds a descriptor to the specified <paramref name="excludeBodyValidationDescriptorCollection" />
        /// that excludes the properties of the type specified and it's derived types from validaton.
        /// </summary>
        /// <param name="excludeBodyValidationDescriptorCollection">A list of <see cref="ExcludeBodyValidationDescriptor"/>
        /// which are used to get a collection of exclude filters to be applied for filtering model properties during validation.
        /// </param>
        /// <param name="typeFullName">Full name of the type which should be excluded from validation.</param>
        public static void Add(this IList<ExcludeBodyValidationDescriptor> excludeBodyValidationDescriptorCollection,
                               string typeFullName)
        {
            var filter = new DefaultTypeNameBasedExcludeFilter(typeFullName);
            excludeBodyValidationDescriptorCollection.Add(new ExcludeBodyValidationDescriptor(filter));
        }
    }
}