// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.QuickTime
{
    /// <summary>
    /// Base class for <see cref="IQuickTimeAtomHandler"/> implementations.
    /// </summary>
    /// <typeparam name="T">Directory type.</typeparam>
    /// <author>Dmitry Shechtman</author>
    public abstract class QuickTimeAtomHandler<T> : IQuickTimeAtomHandler
        where T : Directory, new()
    {
        private readonly List<Directory> _directories;

        protected QuickTimeAtomHandler(List<Directory> directories)
        {
            _directories = directories;
        }

        public void ProcessAtom(Stream stream, SequentialReader reader, int atomSize)
        {
            var directory = new T();
            Populate(directory, reader, atomSize);
            _directories.Add(directory);
        }

        protected abstract void Populate(T directory, SequentialReader reader, int atomSize);
    }
}
