// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

namespace MetadataExtractor.Formats.QuickTime
{
    sealed class QuickTimeTrackHandler : QuickTimeHandler
    {
        public QuickTimeTrackHandler(List<Directory> directories)
            : base(directories, new Dictionary<string, Func<List<Directory>, IQuickTimeAtomHandler>>
            {
                { "tkhd", d => new QuickTimeTrackHeaderHandler(d) }
            })
        {
        }
    }

    sealed class QuickTimeMovieHandler : QuickTimeHandler
    {
        public QuickTimeMovieHandler(List<Directory> directories)
            : base(directories, new Dictionary<string, Func<List<Directory>, IQuickTimeAtomHandler>>
            {
                { "mvhd", d => new QuickTimeMovieHeaderHandler(d) },
                { "trak", d => new QuickTimeTrackHandler(d) }
            })
        {
        }
    }

    sealed class QuickTimeRootHandler : QuickTimeHandler
    {
        public QuickTimeRootHandler(List<Directory> directories)
            : base(directories, new Dictionary<string, Func<List<Directory>, IQuickTimeAtomHandler>>
            {
                { "moov", d => new QuickTimeMovieHandler(d) },
                { "uuid", d => new QuickTimeUuidHandler(d) },
                { "ftyp", d => new QuickTimeFileTypeHandler(d) }
            })
        {
        }
    }
}
