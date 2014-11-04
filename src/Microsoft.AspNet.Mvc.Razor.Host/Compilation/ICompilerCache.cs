// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.FileSystems;

namespace Microsoft.AspNet.Mvc.Razor
{
    /// <summary>
    /// Caches the result of runtime compilation of Razor files for the duration of the app lifetime.
    /// </summary>
    public interface ICompilerCache
    {
        /// <summary>
        /// Adds the specified <paramref name="type"/> to the cache.
        /// </summary>
        /// <param name="info">The <see cref="RazorFileInfo"/> to add the entry for.</param>
        /// <param name="type">The <see cref="Type"/> to cache.</param>
        void Add(RazorFileInfo info, Type type);

        /// <summary>
        /// Get an existing compilation result, or create and add a new one if it is
        /// not available in the cache.
        /// </summary>
        /// <param name="fileInfo">A <see cref="RelativeFileInfo"/> representing the file.</param>
        /// <param name="fileSystem">An <see cref="IFileSystem"/> that represents application's file system.</param>
        /// <param name="compile">An delegate that will generate a compilation result.</param>
        /// <returns>A cached <see cref="CompilationResult"/>.</returns>
        /// <remarks>
        /// The compilation of a Razor file is affected by _ViewStart files that are in the file's directory hierarchy.
        /// The <paramref name="fileSystem"/> is used to scan for the addition or deletion of _ViewStarts and validity
        /// of existing _ViewStart cache entries to ensure the file at <paramref name="fileInfo"/> does not need to be
        /// recompiled.
        /// </remarks>
        CompilationResult GetOrAdd([NotNull] RelativeFileInfo fileInfo,
                                   [NotNull] IFileSystem fileSystem,
                                   [NotNull] Func<CompilationResult> compile);

        /// <summary>
        /// Gets existing metadata associated with the <see cref="CompilationResult"/> for the entry specified by
        /// <paramref name="fileInfo"/>, or creates and adds a new one if the entry is not available or has
        /// expired.
        /// </summary>
        /// <param name="fileInfo">A <see cref="RelativeFileInfo"/> representing the file.</param>
        /// <param name="fileSystem">An <see cref="IFileSystem"/> that represents application's file system.</param>
        /// <param name="key">The key for the metadata.</param>
        /// <param name="valueFactory">The factory used to create new metadata values.</param>
        /// <returns>The metadata value.</returns>
        object GetOrAddMetadata(RelativeFileInfo fileInfo,
                                IFileSystem fileSystem,
                                object key,
                                Func<object> valueFactory);
    }
}