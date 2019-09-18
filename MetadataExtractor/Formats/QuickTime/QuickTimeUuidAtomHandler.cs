// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using MetadataExtractor.IO;
using MetadataExtractor.Util;

namespace MetadataExtractor.Formats.QuickTime
{
    /// <author>Dmitry Shechtman</author>
    public abstract class QuickTimeUuidAtomHandler : IQuickTimeAtomHandler
    {
        private const int UuidSize = 16;

        private readonly List<Directory> _directories;
        private readonly Dictionary<byte[], Func<List<Directory>, IQuickTimeUuidHandler>> _handlers;

        protected QuickTimeUuidAtomHandler(List<Directory> directories, Dictionary<byte[], Func<List<Directory>, IQuickTimeUuidHandler>> handlers)
        {
            _directories = directories;
            _handlers = handlers;
        }

        public bool ProcessAtom(Stream stream, SequentialReader reader, long atomSize)
        {
            if (atomSize >= UuidSize)
            {
                var uuid = reader.GetBytes(UuidSize);
                foreach (var kvp in _handlers)
                {
                    if (kvp.Key.RegionEquals(0, UuidSize, uuid))
                    {
                        var handler = kvp.Value(_directories);
                        return handler.ProcessUuid(stream, reader, atomSize - UuidSize);
                    }
                }
            }
            return true;
        }
    }
}
