// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.AspNet.Mvc
{
    /// <summary>
    /// Represents an entity which has binding information for a model.
    /// </summary>
    public interface IModelPropertyPredicateProvider<TModel>
    {
        IEnumerable<Expression<Func<TModel, object>>> IncludePropertyPredicates { get; }

        IEnumerable<Func<string, bool>> PropertyPredicates { get; }
    }
}
