// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.QuickTime;
using MetadataExtractor.Formats.Tiff;
using MetadataExtractor.IO;
using System.Collections.Generic;

namespace MetadataExtractor.Formats.Crx
{
    /// <author>Dmitry Shechtman</author>
    abstract class CrxCmtHandler : IQuickTimeAtomHandler
    {
        protected readonly List<Directory> _directories;

        protected CrxCmtHandler(List<Directory> directories)
        {
            _directories = directories;
        }

        public bool ProcessAtom(System.IO.Stream stream, SequentialReader reader, long atomSize)
        {
            var handler = CreateTiffHandler();
            var indexedReader = new IndexedSeekingReader(stream, (int)reader.Position);
            TiffReader.ProcessTiff(indexedReader, handler);
            return true;
        }

        protected abstract ITiffHandler CreateTiffHandler();
    }

    /// <author>Dmitry Shechtman</author>
    sealed class CrxCmt1Handler : CrxCmtHandler
    {
        public CrxCmt1Handler(List<Directory> directories)
            : base(directories)
        {
        }

        protected override ITiffHandler CreateTiffHandler() => new ExifTiffHandler(_directories);
    }

    /// <author>Dmitry Shechtman</author>
    sealed class CrxCmtHandler<T, TParent> : CrxCmtHandler
        where T : Directory, new()
        where TParent : Directory
    {
        public CrxCmtHandler(List<Directory> directories)
            : base(directories)
        {
        }

        protected override ITiffHandler CreateTiffHandler() => new CrxTiffHandler<T, TParent>(_directories);
    }
}
