// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNet.Mvc.ModelBinding;
using Xunit;

namespace Microsoft.AspNet.Mvc
{
    public class ExcludeFromValidationDelegateExtensionsTests
    {
        [Theory]
        [InlineData(typeof(BaseType))]
        [InlineData(typeof(DerivedType))]
        public void Insert_WithType_RegistersTypesAndDerivedType_ToBeExcluded(Type type)
        {
            // Arrange
            var collection = new List<ExcludeFromValidationDelegate>();

            // Act
            collection.Add(typeof(BaseType));

            // Assert
            Assert.True(collection[0](type));
        }

        [Theory]
        [InlineData(typeof(BaseType))]
        [InlineData(typeof(DerivedType))]
        public void Insert_WithTypeName_RegistersTypesAndDerivedType_ToBeExcluded(Type type)
        {
            // Arrange
            var collection = new List<ExcludeFromValidationDelegate>();

            // Act
            collection.Add("BaseType");

            // Assert
            Assert.True(collection[0](type));
        }

        private class BaseType
        {
            public int Prop1 { get; set; }
            public string Prop2 { get; set; }
        }

        private class DerivedType : BaseType
        {
            public int DerivedProp1 { get; set; }
        }
    }
}
