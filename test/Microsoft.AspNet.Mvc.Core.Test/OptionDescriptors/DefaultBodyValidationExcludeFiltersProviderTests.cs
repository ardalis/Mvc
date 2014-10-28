// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;
using Moq;
using Xunit;

namespace Microsoft.AspNet.Mvc.OptionDescriptors
{
    public class DefaultBodyValidationExcludeFiltersProviderTests
    {
        [Theory]
        [InlineData(typeof(BaseType))]
        [InlineData(typeof(DerivedType))]
        public void Add_WithType_RegistersTypesAndDerivedType_ToBeExcluded(Type type)
        {
            // Arrange
            var options = new MvcOptions();
            options.BodyValidationExcludeFilters.Add(type);
            var optionsAccessor = new Mock<IOptions<MvcOptions>>();
            optionsAccessor.SetupGet(o => o.Options)
                           .Returns(options);
            var activator = new TypeActivator();
            var serviceProvider = new Mock<IServiceProvider>();
            var provider = new DefaultBodyValidationExcludeFiltersProvider(optionsAccessor.Object,
                                                                           activator,
                                                                           serviceProvider.Object);

            // Act
            var binders = provider.ExcludeFilters;

            // Assert
            Assert.Equal(1, binders.Count);
            Assert.True(binders[0].IsTypeExcluded(type));
        }

        [Theory]
        [InlineData(typeof(BaseType))]
        [InlineData(typeof(UnRelatedType))]
        public void Add_RegisterDerivedType_BaseAndUnrealatedTypesAreNotExcluded(Type type)
        {
            // Arrange
            var options = new MvcOptions();
            options.BodyValidationExcludeFilters.Add(typeof(DerivedType));
            var optionsAccessor = new Mock<IOptions<MvcOptions>>();
            optionsAccessor.SetupGet(o => o.Options)
                           .Returns(options);
            var activator = new TypeActivator();
            var serviceProvider = new Mock<IServiceProvider>();
            var provider = new DefaultBodyValidationExcludeFiltersProvider(optionsAccessor.Object,
                                                                           activator,
                                                                           serviceProvider.Object);

            // Act
            var binders = provider.ExcludeFilters;

            // Assert
            Assert.Equal(1, binders.Count);
            Assert.False(binders[0].IsTypeExcluded(type));
        }

        [Theory]
        [InlineData(typeof(BaseType))]
        [InlineData(typeof(DerivedType))]
        public void Add_WithTypeName_RegistersTypesAndDerivedType_ToBeExcluded(Type type)
        {
            // Arrange
            var options = new MvcOptions();
            options.BodyValidationExcludeFilters.Add(type.FullName);
            var optionsAccessor = new Mock<IOptions<MvcOptions>>();
            optionsAccessor.SetupGet(o => o.Options)
                           .Returns(options);
            var activator = new TypeActivator();
            var serviceProvider = new Mock<IServiceProvider>();
            var provider = new DefaultBodyValidationExcludeFiltersProvider(optionsAccessor.Object,
                                                                           activator,
                                                                           serviceProvider.Object);

            // Act
            var binders = provider.ExcludeFilters;

            // Assert
            Assert.Equal(1, binders.Count);
            Assert.True(binders[0].IsTypeExcluded(type));
        }

        [Theory]
        [InlineData(typeof(BaseType))]
        [InlineData(typeof(UnRelatedType))]
        public void Add_WithTypeName_RegisterDerivedType_BaseAndUnrealatedTypesAreNotExcluded(Type type)
        {
            // Arrange
            var options = new MvcOptions();
            options.BodyValidationExcludeFilters.Add(typeof(DerivedType).FullName);
            var optionsAccessor = new Mock<IOptions<MvcOptions>>();
            optionsAccessor.SetupGet(o => o.Options)
                           .Returns(options);
            var activator = new TypeActivator();
            var serviceProvider = new Mock<IServiceProvider>();
            var provider = new DefaultBodyValidationExcludeFiltersProvider(optionsAccessor.Object,
                                                                           activator,
                                                                           serviceProvider.Object);

            // Act
            var binders = provider.ExcludeFilters;

            // Assert
            Assert.Equal(1, binders.Count);
            Assert.False(binders[0].IsTypeExcluded(type));
        }

        private class BaseType
        {
        }

        private class DerivedType : BaseType
        {
        }

        private class UnRelatedType
        {
        }
    }
}
