// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.QuickTime
{
    /// <summary>
    /// Implementation of <see cref="IQuickTimeHandler"/> using a dictionary of
    /// <see cref="IQuickTimeAtomHandler"/> factory delegates.
    /// </summary>
    /// <author>Dmitry Shechtman</author>
    abstract class QuickTimeHandler : IQuickTimeHandler, IQuickTimeAtomHandler
    {
        private readonly List<Directory> _directories;
        private readonly Dictionary<string, Func<List<Directory>, IQuickTimeAtomHandler>> _handlers;

        protected QuickTimeHandler(List<Directory> directories, Dictionary<string, Func<List<Directory>, IQuickTimeAtomHandler>> handlers)
        {
            _directories = directories;
            _handlers = handlers;
        }

        public virtual void ProcessAtom(Stream stream, SequentialReader reader, int atomSize)
        {
            QuickTimeReader.ProcessAtoms(stream, this, atomSize);
        }

        void IQuickTimeHandler.ProcessAtom(AtomCallbackArgs a)
        {
            if (!_handlers.TryGetValue(a.TypeString, out var createHandler))
                return;
            var handler = createHandler(_directories);
            handler.ProcessAtom(a.Stream, a.Reader, (int)a.BytesLeft);
        }
    }
}
