// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNet.FileSystems;
using Moq;
using Xunit;

namespace Microsoft.AspNet.Mvc.Razor.Test
{
    public class CompilerCacheTest
    {
        [Fact]
        public void GetOrAdd_ReturnsCompilationResultFromFactory()
        {
            // Arrange
            var cache = new CompilerCache();
            var fileInfo = new Mock<IFileInfo>();

            fileInfo
                .SetupGet(i => i.LastModified)
                .Returns(DateTime.FromFileTimeUtc(10000));

            var type = GetType();
            var expected = UncachedCompilationResult.Successful(type, "hello world");

            var runtimeFileInfo = new RelativeFileInfo(fileInfo.Object, "ab");
            var fileSystem = Mock.Of<IFileSystem>();

            // Act
            var actual = cache.GetOrAdd(runtimeFileInfo, fileSystem, () => expected);

            // Assert
            Assert.Same(expected, actual);
            Assert.Equal("hello world", actual.CompiledContent);
            Assert.Same(type, actual.CompiledType);
        }

        private abstract class View
        {
            public abstract string Content { get; }
        }

        private class PreCompile : View
        {
            public override string Content { get { return "Hello World it's @DateTime.Now"; } }
        }

        private class RuntimeCompileIdentical : View
        {
            public override string Content { get { return new PreCompile().Content; } }
        }

        private class RuntimeCompileDifferent : View
        {
            public override string Content { get { return new PreCompile().Content.Substring(1) + " "; } }
        }

        private class RuntimeCompileDifferentLength : View
        {
            public override string Content
            {
                get
                {
                    return new PreCompile().Content + " longer because it was modified at runtime";
                }
            }
        }

        [Theory]
        [InlineData(typeof(RuntimeCompileIdentical), 10000)]
        [InlineData(typeof(RuntimeCompileIdentical), 11000)]
        [InlineData(typeof(RuntimeCompileDifferent), 10000)] // expected failure: same time and length
        public void GetOrAdd_UsesFilesFromCache_IfTimestampDiffers_ButContentAndLengthAreTheSame(
            Type resultViewType,
            long fileTimeUTC)
        {
            // Arrange
            var instance = (View)Activator.CreateInstance(resultViewType);
            var length = Encoding.UTF8.GetByteCount(instance.Content);
            var cache = new CompilerCache();

            var fileInfo = new Mock<IFileInfo>();
            fileInfo
                .SetupGet(i => i.Length)
                .Returns(length);
            fileInfo
                .SetupGet(i => i.LastModified)
                .Returns(DateTime.FromFileTimeUtc(fileTimeUTC));
            fileInfo.Setup(i => i.CreateReadStream())
                .Returns(GetMemoryStream(instance.Content));

            var runtimeFileInfo = new RelativeFileInfo(fileInfo.Object, "ab");

            var precompiledContent = new PreCompile().Content;
            var razorFileInfo = new RazorFileInfo
            {
                FullTypeName = typeof(PreCompile).FullName,
                Hash = RazorFileHash.GetHash(GetMemoryStream(precompiledContent)),
                LastModified = DateTime.FromFileTimeUtc(10000),
                Length = Encoding.UTF8.GetByteCount(precompiledContent),
                RelativePath = "ab",
            };

            // Act
            cache.Add(razorFileInfo, typeof(PreCompile));
            var actual = cache.GetOrAdd(runtimeFileInfo,
                                        Mock.Of<IFileSystem>(),
                                        compile: () => CompilationResult.Successful(resultViewType));

            // Assert
            Assert.Equal(typeof(PreCompile), actual.CompiledType);
        }

        [Fact]
        public void GetOrAdd_UsesValueFromCache_IfViewStartHasNotChanged()
        {
            // Arrange
            var instance = (View)Activator.CreateInstance(typeof(PreCompile));
            var length = Encoding.UTF8.GetByteCount(instance.Content);
            var cache = new CompilerCache();
            var lastModified = DateTime.UtcNow;

            var fileInfo = new TestFileInfo
            {
                Length = length,
                LastModified = lastModified,
                Content = instance.Content
            };

            var runtimeFileInfo = new RelativeFileInfo(fileInfo, "ab");

            var precompiledContent = new PreCompile().Content;
            var razorFileInfo = new RazorFileInfo
            {
                FullTypeName = typeof(PreCompile).FullName,
                Hash = RazorFileHash.GetHash(GetMemoryStream(precompiledContent)),
                LastModified = DateTime.FromFileTimeUtc(10000),
                Length = Encoding.UTF8.GetByteCount(precompiledContent),
                RelativePath = "ab",
            };

            var viewStartRazorFileInfo = new RazorFileInfo
            {
                Hash = RazorFileHash.GetHash(GetMemoryStream("viewstart-content")),
                LastModified = DateTime.UtcNow,
                Length = 30,
                RelativePath = "/_viewstart.cshtml"
            };

            IFileInfo viewStartFileInfo = new TestFileInfo
            {
                Length = viewStartRazorFileInfo.Length,
                Content = "viewstart-content",
                LastModified = viewStartRazorFileInfo.LastModified
            };

            var fileSystem = new Mock<IFileSystem>();
            fileSystem.Setup(f => f.TryGetFileInfo("/_viewstart.cshtml", out viewStartFileInfo))
                      .Returns(true);

            // Act
            cache.Add(razorFileInfo, typeof(PreCompile));
            cache.Add(viewStartRazorFileInfo, typeof(RuntimeCompileIdentical));
            var actual = cache.GetOrAdd(runtimeFileInfo,
                                        fileSystem.Object,
                                        compile: () => { throw new Exception("shouldn't be invoked"); });

            // Assert
            Assert.Equal(typeof(PreCompile), actual.CompiledType);
        }

        [Fact]
        public void GetOrAdd_IgnoresCachedValueIfFileIsIdentical_ButViewStartWasAdedSinceTheCacheWasCreated()
        {
            // Arrange
            var expectedType = typeof(RuntimeCompileDifferent);
            var lastModified = DateTime.UtcNow;
            var content = "some content";
            var cache = new CompilerCache();

            var fileInfo = new TestFileInfo
            {
                Length = 1020,
                Content = content,
                LastModified = lastModified
            };

            var runtimeFileInfo = new RelativeFileInfo(fileInfo, "ab");

            var razorFileInfo = new RazorFileInfo
            {
                FullTypeName = typeof(PreCompile).FullName,
                Hash = RazorFileHash.GetHash(fileInfo),
                LastModified = lastModified,
                Length = Encoding.UTF8.GetByteCount(content),
                RelativePath = "ab",
            };

            IFileInfo viewStart = new TestFileInfo
            {
                Length = 1030
            };

            var fileSystem = new Mock<IFileSystem>();
            fileSystem.Setup(f => f.TryGetFileInfo("/views/_viewstart.cshtml", out viewStart))
                      .Returns(true);

            // Act 1
            cache.Add(razorFileInfo, typeof(PreCompile));
            var actual1 = cache.GetOrAdd(runtimeFileInfo,
                                        fileSystem.Object,
                                        compile: () => CompilationResult.Successful(expectedType));

            // Assert 1
            Assert.Equal(expectedType, actual1.CompiledType);

            // Act 2
            var actual2 = cache.GetOrAdd(runtimeFileInfo,
                                         fileSystem.Object,
                                         compile: () => { throw new Exception("should not be called"); });

            // Assert 2
            Assert.Equal(expectedType, actual2.CompiledType);
        }

        [Fact]
        public void GetOrAdd_IgnoresCachedValueIfFileIsIdentical_ButViewStartWasDeletedSinceCacheWasCreated()
        {
            // Arrange
            var expectedType = typeof(RuntimeCompileDifferent);
            var lastModified = DateTime.UtcNow;
            var content = "some content";
            var cache = new CompilerCache();

            var fileInfo = new TestFileInfo
            {
                Length = 1020,
                Content = content,
                LastModified = lastModified
            };

            var runtimeFileInfo = new RelativeFileInfo(fileInfo, "ab");

            var razorFileInfo = new RazorFileInfo
            {
                FullTypeName = typeof(PreCompile).FullName,
                Hash = RazorFileHash.GetHash(fileInfo),
                LastModified = lastModified,
                Length = Encoding.UTF8.GetByteCount(content),
                RelativePath = "ab",
            };

            var viewStart = new RazorFileInfo { RelativePath = "viewstart" };
            var fileSystem = Mock.Of<IFileSystem>();

            // Act 1
            cache.Add(razorFileInfo, typeof(PreCompile));
            cache.Add(viewStart, typeof(object));
            var actual1 = cache.GetOrAdd(runtimeFileInfo,
                                        fileSystem,
                                        compile: () => CompilationResult.Successful(expectedType));

            // Assert 1
            Assert.Equal(expectedType, actual1.CompiledType);

            // Act 2
            var actual2 = cache.GetOrAdd(runtimeFileInfo,
                                         fileSystem,
                                         compile: () => { throw new Exception("should not be called"); });

            // Assert 2
            Assert.Equal(expectedType, actual2.CompiledType);
        }

        public static IEnumerable<object[]> GetOrAdd_IgnoresCachedValue_IfViewStartWasChangedSinceCacheWasCreatedData
        {
            get
            {
                var viewStartContent = "viewstart-content";
                var contentStream = GetMemoryStream(viewStartContent);
                var lastModified = DateTime.UtcNow;
                int length = Encoding.UTF8.GetByteCount(viewStartContent);
                var path = "/views/_viewstart.cshtml";

                var razorFileInfo = new RazorFileInfo
                {
                    Hash = RazorFileHash.GetHash(contentStream),
                    LastModified = lastModified,
                    Length = length,
                    RelativePath = path
                };

                // Length does not match
                var testFileInfo1 = new TestFileInfo
                {
                    Length = 7732
                };

                yield return new object[] { razorFileInfo, testFileInfo1 };

                // Content and last modified do not match
                var testFileInfo2 = new TestFileInfo
                {
                    Length = length,
                    Content = "viewstart-modified",
                    LastModified = lastModified.AddSeconds(100),
                };

                yield return new object[] { razorFileInfo, testFileInfo2 };
            }
        }

        [Theory]
        [MemberData(nameof(GetOrAdd_IgnoresCachedValue_IfViewStartWasChangedSinceCacheWasCreatedData))]
        public void GetOrAdd_IgnoresCachedValue_IfViewStartWasChangedSinceCacheWasCreated(
            RazorFileInfo viewStartRazorFileInfo, IFileInfo viewStartFileInfo)
        {
            // Arrange
            var expectedType = typeof(RuntimeCompileDifferent);
            var lastModified = DateTime.UtcNow;
            var viewStartLastModified = DateTime.UtcNow;
            var content = "some content";
            var cache = new CompilerCache();

            var fileInfo = new TestFileInfo
            {
                Length = 1020,
                Content = content,
                LastModified = lastModified
            };

            var runtimeFileInfo = new RelativeFileInfo(fileInfo, "/views/home/index.cshtml");

            var razorFileInfo = new RazorFileInfo
            {
                FullTypeName = typeof(PreCompile).FullName,
                Hash = RazorFileHash.GetHash(fileInfo),
                LastModified = lastModified,
                Length = Encoding.UTF8.GetByteCount(content),
                RelativePath = "ab",
            };

            var fileSystem = new Mock<IFileSystem>();
            fileSystem.Setup(f => f.TryGetFileInfo("/views/_viewstart.cshtml", out viewStartFileInfo))
                      .Returns(true);

            // Act
            cache.Add(razorFileInfo, typeof(PreCompile));
            cache.Add(viewStartRazorFileInfo, typeof(RuntimeCompileIdentical));
            var actual = cache.GetOrAdd(runtimeFileInfo,
                                        fileSystem.Object,
                                        compile: () => CompilationResult.Successful(expectedType));

            // Assert
            Assert.Equal(expectedType, actual.CompiledType);
        }

        [Theory]
        [InlineData(typeof(RuntimeCompileDifferent), 11000)]
        [InlineData(typeof(RuntimeCompileDifferentLength), 10000)]
        [InlineData(typeof(RuntimeCompileDifferentLength), 10000)]
        public void GetOrAdd_IgnoresFilesFromCache_IfTheFileLengthOnDiskDiffersFromTheOneInCache(
            Type resultViewType,
            long fileTimeUTC)
        {
            // Arrange
            var instance = (View)Activator.CreateInstance(resultViewType);
            var length = Encoding.UTF8.GetByteCount(instance.Content);
            var cache = new CompilerCache();

            var fileInfo = new TestFileInfo
            {
                Length = length,
                LastModified = DateTime.FromFileTimeUtc(fileTimeUTC),
                Content = instance.Content
            };
            var runtimeFileInfo = new RelativeFileInfo(fileInfo, "ab");

            var precompiledContent = new PreCompile().Content;
            var razorFileInfo = new RazorFileInfo
            {
                FullTypeName = typeof(PreCompile).FullName,
                Hash = RazorFileHash.GetHash(GetMemoryStream(precompiledContent)),
                LastModified = DateTime.FromFileTimeUtc(10000),
                Length = Encoding.UTF8.GetByteCount(precompiledContent),
                RelativePath = "ab",
            };

            // Act
            cache.Add(razorFileInfo, typeof(PreCompile));
            var actual = cache.GetOrAdd(runtimeFileInfo,
                                        Mock.Of<IFileSystem>(),
                                        compile: () => CompilationResult.Successful(resultViewType));

            // Assert
            Assert.Equal(resultViewType, actual.CompiledType);
        }

        [Fact]
        public void GetOrAdd_DoesNotCacheCompiledContent_OnCallsAfterInitial()
        {
            // Arrange
            var lastModified = DateTime.UtcNow;
            var cache = new CompilerCache();
            var fileInfo = new TestFileInfo
            {
                PhysicalPath = "test",
                LastModified = lastModified
            };
            var type = GetType();
            var fileSystem = Mock.Of<IFileSystem>();
            var uncachedResult = UncachedCompilationResult.Successful(type, "hello world");

            var runtimeFileInfo = new RelativeFileInfo(fileInfo, "test");

            // Act
            cache.GetOrAdd(runtimeFileInfo, fileSystem, () => uncachedResult);
            var actual1 = cache.GetOrAdd(runtimeFileInfo, fileSystem, () => uncachedResult);
            var actual2 = cache.GetOrAdd(runtimeFileInfo, fileSystem, () => uncachedResult);

            // Assert
            Assert.NotSame(uncachedResult, actual1);
            Assert.NotSame(uncachedResult, actual2);
            var result = Assert.IsType<CompilationResult>(actual1);
            Assert.Null(actual1.CompiledContent);
            Assert.Same(type, actual1.CompiledType);

            result = Assert.IsType<CompilationResult>(actual2);
            Assert.Null(actual2.CompiledContent);
            Assert.Same(type, actual2.CompiledType);
        }

        [Fact]
        public void GetOrAddMetadata_ReturnsValueFromValueFactoryIfCacheEntryDoesNotExist()
        {
            // Arrange
            var cache = new CompilerCache();
            var key = new object();
            var fileInfo = new TestFileInfo
            {
                PhysicalPath = "test",
                LastModified = DateTime.UtcNow
            };
            var expected = new object();
            var relativeFileInfo = new RelativeFileInfo(fileInfo, "/views/home/index.cshtml");

            // Act
            var result = cache.GetOrAddMetadata(relativeFileInfo,
                                                Mock.Of<IFileSystem>(),
                                                key,
                                                () => expected);

            // Assert
            Assert.Same(expected, result);
        }

        public static IEnumerable<object[]> GetOrAddMetadata_ReturnsValueFromValueFactoryIfCacheEntryIsInvalidData
        {
            get
            {
                var viewStartContent = "content";
                var contentStream = GetMemoryStream(viewStartContent);
                var lastModified = DateTime.UtcNow;
                int length = Encoding.UTF8.GetByteCount(viewStartContent);

                var razorFileInfo = new RazorFileInfo
                {
                    Hash = RazorFileHash.GetHash(contentStream),
                    LastModified = lastModified,
                    Length = length,
                    RelativePath = "/views/shared/view.cshtml"
                };

                // Length does not match
                var testFileInfo1 = new TestFileInfo
                {
                    Length = 7732
                };

                yield return new object[] { razorFileInfo, testFileInfo1 };

                // Content and last modified do not match
                var testFileInfo2 = new TestFileInfo
                {
                    Length = length,
                    Content = "content-modified",
                    LastModified = lastModified.AddSeconds(100),
                };

                yield return new object[] { razorFileInfo, testFileInfo2 };
            }
        }

        [Theory]
        [MemberData(nameof(GetOrAddMetadata_ReturnsValueFromValueFactoryIfCacheEntryIsInvalidData))]
        public void GetOrAddMetadata_ReturnsValueFromValueFactoryIfCacheEntryIsInvalid(
            RazorFileInfo razorFileInfo, IFileInfo fileInfo)
        {
            // Arrange
            var cache = new CompilerCache();
            var key = new object();
           
            var expected = new object();
            cache.Add(razorFileInfo, typeof(PreCompile));
            var relativeFileInfo = new RelativeFileInfo(fileInfo, razorFileInfo.RelativePath);

            // Act
            var result = cache.GetOrAddMetadata(relativeFileInfo,
                                                Mock.Of<IFileSystem>(),
                                                key,
                                                () => expected);

            // Assert
            Assert.Same(expected, result);
        }

        [Fact]
        public void GetOrAddMetadata_ReturnsCachedValueIfCacheEntryIsValid()
        {
            // Arrange
            var key = new object();
            var expected = new object();

            var fileInfo = new TestFileInfo
            {
                Length = 124,
                LastModified = DateTime.UtcNow,
                Content = "some-content"
            };

            var runtimeFileInfo = new RelativeFileInfo(fileInfo, "/home/views/index.cshtml");

            var precompiledContent = new PreCompile().Content;
            var razorFileInfo = new RazorFileInfo
            {
                Hash = RazorFileHash.GetHash(GetMemoryStream(fileInfo.Content)),
                LastModified = fileInfo.LastModified,
                Length = fileInfo.Length,
                RelativePath = "/home/views/index.cshtml",
            };

            var viewStartRazorFileInfo = new RazorFileInfo
            {
                Hash = RazorFileHash.GetHash(GetMemoryStream("viewstart-content")),
                LastModified = DateTime.UtcNow,
                Length = 30,
                RelativePath = "/_viewstart.cshtml"
            };

            IFileInfo viewStartFileInfo = new TestFileInfo
            {
                Length = viewStartRazorFileInfo.Length,
                Content = "viewstart-content",
                LastModified = viewStartRazorFileInfo.LastModified
            };

            var fileSystem = new Mock<IFileSystem>();
            fileSystem.Setup(f => f.TryGetFileInfo("/_viewstart.cshtml", out viewStartFileInfo))
                      .Returns(true);
            var cache = new CompilerCache();
            cache.Add(razorFileInfo, typeof(PreCompile));
            cache.Add(viewStartRazorFileInfo, typeof(RuntimeCompileIdentical));

            // Act - 1
            var actual1 = cache.GetOrAddMetadata(runtimeFileInfo,
                                                 fileSystem.Object,
                                                 key,
                                                 valueFactory: () => expected);

            // Assert - 1
            Assert.Same(expected, actual1);

            // Act - 2 
            var actual2 = cache.GetOrAddMetadata(runtimeFileInfo,
                                                  fileSystem.Object,
                                                  key,
                                                  valueFactory: () => { throw new Exception("shouldn't be called"); });

            // Assert - 2
            Assert.Same(expected, actual2);
        }

        private sealed class TestFileInfo : IFileInfo
        {
            public bool IsDirectory { get; } = false;

            public DateTime LastModified { get; set; }

            public long Length { get; set; }

            public string Name { get; set; }

            public string PhysicalPath { get; set; }

            public string Content { get; set; }

            public Stream CreateReadStream()
            {
                return GetMemoryStream(Content);
            }
        }

        private static Stream GetMemoryStream(string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);

            return new MemoryStream(bytes);
        }
    }
}