// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.QuickTime;
using System;
using System.Collections.Generic;

namespace MetadataExtractor.Formats.Crx
{
    /// <author>Dmitry Shechtman</author>
    sealed class CrxRootHandler : QuickTimeHandler
    {
        public CrxRootHandler(List<Directory> directories)
            : base(directories, new Dictionary<string, Func<List<Directory>, IQuickTimeAtomHandler>>
            {
                { "moov", d => new CrxMovieHandler(d) },
                { "uuid", d => new CrxUuidHandler(d) },
                { "ftyp", d => new QuickTimeFileTypeHandler(d) }
            })
        {
        }
    }
}
