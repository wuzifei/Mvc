﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if DNX451
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#endif
using System.Threading.Tasks;
#if DNX451
using Moq;
#endif
using Xunit;

namespace Microsoft.AspNet.Mvc.ModelBinding.Test
{
    public class CollectionModelBinderTest
    {
#if DNX451
        [Fact]
        public async Task BindComplexCollectionFromIndexes_FiniteIndexes()
        {
            // Arrange
            var valueProvider = new SimpleHttpValueProvider
            {
                { "someName[foo]", "42" },
                { "someName[baz]", "200" }
            };
            var bindingContext = GetModelBindingContext(valueProvider);
            var binder = new CollectionModelBinder<int>();

            // Act
            var boundCollection = await binder.BindComplexCollectionFromIndexes(bindingContext, new[] { "foo", "bar", "baz" });

            // Assert
            Assert.Equal(new[] { 42, 0, 200 }, boundCollection.ToArray());
        }

        [Fact]
        public async Task BindComplexCollectionFromIndexes_InfiniteIndexes()
        {
            // Arrange
            var valueProvider = new SimpleHttpValueProvider
            {
                { "someName[0]", "42" },
                { "someName[1]", "100" },
                { "someName[3]", "400" }
            };
            var bindingContext = GetModelBindingContext(valueProvider);
            var binder = new CollectionModelBinder<int>();

            // Act
            var boundCollection = await binder.BindComplexCollectionFromIndexes(bindingContext, indexNames: null);

            // Assert
            Assert.Equal(new[] { 42, 100 }, boundCollection.ToArray());
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task BindModel_ComplexCollection_Succeeds(bool isReadOnly)
        {
            // Arrange
            var valueProvider = new SimpleHttpValueProvider
            {
                { "someName.index", new[] { "foo", "bar", "baz" } },
                { "someName[foo]", "42" },
                { "someName[bar]", "100" },
                { "someName[baz]", "200" }
            };
            var bindingContext = GetModelBindingContext(valueProvider, isReadOnly);
            var modelState = bindingContext.ModelState;
            var binder = new CollectionModelBinder<int>();

            // Act
            var result = await binder.BindModelAsync(bindingContext);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsModelSet);

            var list = Assert.IsAssignableFrom<IList<int>>(result.Model);
            Assert.Equal(new[] { 42, 100, 200 }, list.ToArray());

            Assert.True(modelState.IsValid);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task BindModel_ComplexCollection_BindingContextModelNonNull_Succeeds(bool isReadOnly)
        {
            // Arrange
            var valueProvider = new SimpleHttpValueProvider
            {
                { "someName.index", new[] { "foo", "bar", "baz" } },
                { "someName[foo]", "42" },
                { "someName[bar]", "100" },
                { "someName[baz]", "200" }
            };
            var bindingContext = GetModelBindingContext(valueProvider, isReadOnly);
            var modelState = bindingContext.ModelState;
            var list = new List<int>();
            bindingContext.Model = list;
            var binder = new CollectionModelBinder<int>();

            // Act
            var result = await binder.BindModelAsync(bindingContext);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsModelSet);

            Assert.Same(list, result.Model);
            Assert.Equal(new[] { 42, 100, 200 }, list.ToArray());

            Assert.True(modelState.IsValid);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task BindModel_SimpleCollection_Succeeds(bool isReadOnly)
        {
            // Arrange
            var valueProvider = new SimpleHttpValueProvider
            {
                { "someName", new[] { "42", "100", "200" } }
            };
            var bindingContext = GetModelBindingContext(valueProvider, isReadOnly);
            var modelState = bindingContext.ModelState;
            var binder = new CollectionModelBinder<int>();

            // Act
            var result = await binder.BindModelAsync(bindingContext);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsModelSet);

            var list = Assert.IsAssignableFrom<IList<int>>(result.Model);
            Assert.Equal(new[] { 42, 100, 200 }, list.ToArray());

            Assert.True(modelState.IsValid);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task BindModel_SimpleCollection_BindingContextModelNonNull_Succeeds(bool isReadOnly)
        {
            // Arrange
            var valueProvider = new SimpleHttpValueProvider
            {
                { "someName", new[] { "42", "100", "200" } }
            };
            var bindingContext = GetModelBindingContext(valueProvider, isReadOnly);
            var modelState = bindingContext.ModelState;
            var list = new List<int>();
            bindingContext.Model = list;
            var binder = new CollectionModelBinder<int>();

            // Act
            var result = await binder.BindModelAsync(bindingContext);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsModelSet);

            Assert.Same(list, result.Model);
            Assert.Equal(new[] { 42, 100, 200 }, list.ToArray());

            Assert.True(modelState.IsValid);
        }
#endif

        [Fact]
        public async Task BindSimpleCollection_RawValueIsEmptyCollection_ReturnsEmptyList()
        {
            // Arrange
            var binder = new CollectionModelBinder<int>();

            var context = new ModelBindingContext()
            {
                OperationBindingContext = new OperationBindingContext()
                {
                    MetadataProvider = TestModelMetadataProvider.CreateDefaultProvider(),
                },
            };

            // Act
            var boundCollection = await binder.BindSimpleCollection(context, rawValue: new object[0], culture: null);

            // Assert
            Assert.NotNull(boundCollection);
            Assert.Empty(boundCollection);
        }

        [Fact]
        public async Task BindSimpleCollection_RawValueIsNull_ReturnsNull()
        {
            // Arrange
            var binder = new CollectionModelBinder<int>();

            // Act
            var boundCollection = await binder.BindSimpleCollection(bindingContext: null, rawValue: null, culture: null);

            // Assert
            Assert.Null(boundCollection);
        }

#if DNX451
        [Fact]
        public async Task BindSimpleCollection_SubBindingSucceeds()
        {
            // Arrange
            var culture = new CultureInfo("fr-FR");
            var bindingContext = GetModelBindingContext(new SimpleHttpValueProvider());

            Mock.Get<IModelBinder>(bindingContext.OperationBindingContext.ModelBinder)
                .Setup(o => o.BindModelAsync(It.IsAny<ModelBindingContext>()))
                .Returns((ModelBindingContext mbc) =>
                {
                    Assert.Equal("someName", mbc.ModelName);
                    return Task.FromResult(new ModelBindingResult(42, mbc.ModelName, true));
                });
            var modelBinder = new CollectionModelBinder<int>();

            // Act
            var boundCollection = await modelBinder.BindSimpleCollection(bindingContext, new int[1], culture);

            // Assert
            Assert.Equal(new[] { 42 }, boundCollection.ToArray());
        }

        private static ModelBindingContext GetModelBindingContext(
            IValueProvider valueProvider,
            bool isReadOnly = false)
        {
            var metadataProvider = new TestModelMetadataProvider();
            metadataProvider.ForType<IList<int>>().BindingDetails(bd => bd.IsReadOnly = isReadOnly);

            var bindingContext = new ModelBindingContext
            {
                ModelMetadata = metadataProvider.GetMetadataForType(typeof(int)),
                ModelName = "someName",
                ValueProvider = valueProvider,
                OperationBindingContext = new OperationBindingContext
                {
                    ModelBinder = CreateIntBinder(),
                    MetadataProvider = metadataProvider
                }
            };

            return bindingContext;
        }

        private static IModelBinder CreateIntBinder()
        {
            Mock<IModelBinder> mockIntBinder = new Mock<IModelBinder>();
            mockIntBinder
                .Setup(o => o.BindModelAsync(It.IsAny<ModelBindingContext>()))
                .Returns(async (ModelBindingContext mbc) =>
                {
                    var value = await mbc.ValueProvider.GetValueAsync(mbc.ModelName);
                    if (value != null)
                    {
                        var model = value.ConvertTo(mbc.ModelType);
                        return new ModelBindingResult(model, mbc.ModelName, true);
                    }

                    return null;
                });
            return mockIntBinder.Object;
        }
#endif
    }
}
