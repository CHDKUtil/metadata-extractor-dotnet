// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.QuickTime;
using System.Collections.Generic;
using System.IO;

#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace MetadataExtractor.Formats.Crx
{
    public static class CrxMetadataReader
    {
        public static DirectoryList ReadMetadata(Stream stream)
        {
            var directories = new List<Directory>();
            QuickTimeReader.ProcessAtoms(stream, new CrxRootHandler(directories));
            return directories;
        }
    }
}
