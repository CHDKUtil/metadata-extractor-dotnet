// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Exif.Makernotes;
using MetadataExtractor.Formats.QuickTime;
using MetadataExtractor.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace MetadataExtractor.Formats.Crx
{
    /// <author>Dmitry Shechtman</author>
    sealed class CrxFileHeaderHandler : QuickTimeHandler, IQuickTimeUuidHandler
    {
        public CrxFileHeaderHandler(List<Directory> directories)
            : base(directories, new Dictionary<string, Func<List<Directory>, IQuickTimeAtomHandler>>
            {
                { "CMT1", d => new CrxCmt1Handler(d) },
                { "CMT2", d => new CrxCmtHandler<ExifSubIfdDirectory, ExifIfd0Directory>(d) },
                { "CMT3", d => new CrxCmtHandler<CanonMakernoteDirectory, ExifSubIfdDirectory>(d) },
                { "CMT4", d => new CrxCmtHandler<GpsDirectory, ExifIfd0Directory>(d) },
            })
        {
        }

        public bool ProcessUuid(Stream stream, SequentialReader reader, long atomSize)
        {
            return ProcessAtom(stream, reader, atomSize);
        }
    }

    /// <author>Dmitry Shechtman</author>
    sealed class CrxMovieUuidHandler : QuickTimeUuidAtomHandler
    {
        public CrxMovieUuidHandler(List<Directory> directories)
            : base(directories, new Dictionary<byte[], Func<List<Directory>, IQuickTimeUuidHandler>>
            {
                { new byte[] { 0x85, 0xc0, 0xb6, 0x87, 0x82, 0x0f, 0x11, 0xe0, 0x81, 0x11, 0xf4, 0xce, 0x46, 0x2b, 0x6a, 0x48 }, d => new CrxFileHeaderHandler(d) }
            })
        {
        }
    }
}
