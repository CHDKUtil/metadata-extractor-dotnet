// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Exif.Makernotes;
using MetadataExtractor.Formats.QuickTime;
using MetadataExtractor.IO;
using MetadataExtractor.Util;
using System;
using System.Collections.Generic;
using System.IO;

namespace MetadataExtractor.Formats.Crx
{
    /// <author>Dmitry Shechtman</author>
    sealed class CrxMovieUuidHandler : QuickTimeHandler
    {
        private static readonly byte[] CRX = new byte[] { 0x85, 0xc0, 0xb6, 0x87, 0x82, 0x0f, 0x11, 0xe0, 0x81, 0x11, 0xf4, 0xce, 0x46, 0x2b, 0x6a, 0x48 };

        public CrxMovieUuidHandler(List<Directory> directories)
            : base(directories, new Dictionary<string, Func<List<Directory>, IQuickTimeAtomHandler>>
                {
                    { "CMT1", d => new CrxCmt1Handler(d) },
                    { "CMT2", d => new CrxCmtHandler<ExifSubIfdDirectory, ExifIfd0Directory>(d) },
                    { "CMT3", d => new CrxCmtHandler<CanonMakernoteDirectory, ExifSubIfdDirectory>(d) },
                    { "CMT4", d => new CrxCmtHandler<GpsDirectory, ExifIfd0Directory>(d) },
                })
        {
        }

        public override bool ProcessAtom(Stream stream, SequentialReader reader, long atomSize)
        {
            if (atomSize >= CRX.Length)
            {
                var uuid = reader.GetBytes(CRX.Length);
                if (CRX.RegionEquals(0, CRX.Length, uuid))
                    base.ProcessAtom(stream, reader, atomSize - CRX.Length);
            }
            return true;
        }
    }
}
