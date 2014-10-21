// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNet.Mvc;
using MyClassTuple = System.Tuple<System.Linq.Expressions.Expression<System.Func<ModelBindingWebSite.User, object>>,
    System.Linq.Expressions.Expression<System.Func<ModelBindingWebSite.User, object>>
    >;

namespace ModelBindingWebSite.Controllers
{
    public class BindAttribute2Controller : Controller
    {

        public User EchoUser([Bind(PredicateProviderType = typeof(UserPredicateProvider))] User user)
        {
            return user;
        }
    }

    public class UserPredicateProvider : IModelPropertyPredicateProvider<User>
    {
        public IEnumerable<Expression<Func<User, object>>> IncludePropertyPredicates
        {
            get
            {
                // Include UserName and RegisterationMonth
                yield return model => model.UserName;
                yield return model => model.RegisterationMonth;
            }
        }

        public IEnumerable<Func<string, bool>> PropertyPredicates
        {
            get
            {
                // exclude key and id.
                yield return propertyName => !string.Equals(propertyName, nameof(User.Id));  
                yield return propertyName => !string.Equals(propertyName, nameof(User.Key));
            }
        }
    }


    public class BindAttribute3Controller : Controller
    {
        public IEnumerable<Expression<Func<User, object>>> IncludePropertyExpressionPredicates
        {
            get
            {
                // Include UserName and RegisterationMonth
                yield return model => model.UserName;
                yield return model => model.RegisterationMonth;
            }
        }

        public Func<string, bool> PropertyPredicate
        {
            get
            {
                // exclude key and id.
                return propertyName => !string.Equals(propertyName, nameof(User.Id)) &&
                                       !string.Equals(propertyName, nameof(User.Key));
            }
        }

        public User EchoUser([Bind2(nameof(IncludePropertyExpressionPredicates), 
                                   nameof(PropertyPredicate))] User user)
        {
            return user;
        }

        public User Echo2User([Bind2(Include = new[] { nameof(User.Id), nameof(User.UserName) }, Prefix = "x")] User user)
        {
            return user;
        }
    }

    public class Bind2Attribute : Attribute
    {
        public Bind2Attribute()
        {
        }

        public Bind2Attribute(string predicateExpressionsPropertyName, string predicatePropertyName)
        {
            PredicateExpressionsPropertyName = predicateExpressionsPropertyName;
            PredicatePropertyName = predicatePropertyName;
        }

        public string PredicateExpressionsPropertyName { get; set; }

        public string PredicatePropertyName { get; set; }

        public string[] Include { get; set; }

        /// <summary>
        /// Allows a user to specify a particular prefix to match during model binding.
        /// </summary>
        public string Prefix { get; set; }
    }
}