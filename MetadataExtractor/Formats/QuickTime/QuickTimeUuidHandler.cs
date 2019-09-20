// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Xmp;
using MetadataExtractor.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MetadataExtractor.Formats.QuickTime
{
    /// <author>Dmitry Shechtman</author>
    sealed class QuickTimeXmpHanlder : IQuickTimeUuidHandler
    {
        private readonly List<Directory> _directories;

        public QuickTimeXmpHanlder(List<Directory> directories)
        {
            _directories = directories;
        }

        public bool ProcessUuid(Stream stream, SequentialReader reader, long atomSize)
        {
            var xmpBytes = reader.GetNullTerminatedBytes((int)atomSize);
            var xmpDirectory = new XmpReader().Extract(xmpBytes);
            xmpDirectory.Parent = _directories.OfType<ExifIfd0Directory>().SingleOrDefault();
            _directories.Add(xmpDirectory);
            return true;
        }
    }

    /// <author>Dmitry Shechtman</author>
    sealed class QuickTimeUuidHandler : QuickTimeUuidAtomHandler
    {
        public QuickTimeUuidHandler(List<Directory> directories)
            : base(directories, new Dictionary<byte[], Func<List<Directory>, IQuickTimeUuidHandler>>
            {
                { new byte[] { 0xbe, 0x7a, 0xcf, 0xcb, 0x97, 0xa9, 0x42, 0xe8, 0x9c, 0x71, 0x99, 0x94, 0x91, 0xe3, 0xaf, 0xac }, d => new QuickTimeXmpHanlder(d) }
            })
        {
        }
    }
}
