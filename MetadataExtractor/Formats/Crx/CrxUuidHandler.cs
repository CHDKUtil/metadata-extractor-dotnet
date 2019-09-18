// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.QuickTime;
using MetadataExtractor.Formats.Xmp;
using MetadataExtractor.IO;
using MetadataExtractor.Util;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MetadataExtractor.Formats.Crx
{
    /// <author>Dmitry Shechtman</author>
    sealed class CrxUuidHandler : IQuickTimeAtomHandler
    {
        private static readonly byte[] XMP = new byte[] { 0xbe, 0x7a, 0xcf, 0xcb, 0x97, 0xa9, 0x42, 0xe8, 0x9c, 0x71, 0x99, 0x94, 0x91, 0xe3, 0xaf, 0xac };

        private readonly List<Directory> _directories;

        public CrxUuidHandler(List<Directory> directories)
        {
            _directories = directories;
        }

        public void ProcessAtom(Stream stream, SequentialReader reader, int atomSize)
        {
            if (atomSize >= XMP.Length)
            {
                var uuid = reader.GetBytes(XMP.Length);
                if (XMP.RegionEquals(0, XMP.Length, uuid))
                {
                    var xmpBytes = reader.GetNullTerminatedBytes(atomSize - XMP.Length);
                    var xmpDirectory = new XmpReader().Extract(xmpBytes);
                    xmpDirectory.Parent = _directories.OfType<ExifIfd0Directory>().SingleOrDefault();
                    _directories.Add(xmpDirectory);
                }
            }
        }
    }
}
