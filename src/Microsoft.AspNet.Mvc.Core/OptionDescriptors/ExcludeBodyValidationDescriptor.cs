// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace Microsoft.AspNet.Mvc.OptionDescriptors
{
    /// <summary>
    /// Encapsulates information that describes an <see cref="IExcludeTypeFromBodyValidationFilter"/>.
    /// </summary>
    public class ExcludeBodyValidationDescriptor : OptionDescriptor<IExcludeTypeFromBodyValidationFilter>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExcludeBodyValidationDescriptor"/>.
        /// </summary>
        /// <param name="type">
        /// A <see cref="IExcludeTypeFromBodyValidationFilter"/> type that the descriptor represents.
        /// </param>
        public ExcludeBodyValidationDescriptor([NotNull] Type type)
            : base(type)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="ExcludeBodyValidationDescriptor"/>.
        /// </summary>
        /// <param name="predicateProvider">An instance of <see cref="IExcludeTypeFromBodyValidationFilter"/>
        /// that the descriptor represents.</param>
        public ExcludeBodyValidationDescriptor([NotNull] IExcludeTypeFromBodyValidationFilter predicateProvider)
            : base(predicateProvider)
        {
        }
    }
}