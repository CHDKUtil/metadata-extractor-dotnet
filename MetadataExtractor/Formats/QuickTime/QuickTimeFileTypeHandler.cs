// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.QuickTime
{
    sealed class QuickTimeFileTypeHandler : QuickTimeAtomHandler<QuickTimeFileTypeDirectory>
    {
        public QuickTimeFileTypeHandler(List<Directory> directories)
            : base(directories)
        {
        }

        protected override void Populate(QuickTimeFileTypeDirectory directory, SequentialReader reader, int atomSize)
        {
            directory.Set(QuickTimeFileTypeDirectory.TagMajorBrand, reader.Get4ccString());
            directory.Set(QuickTimeFileTypeDirectory.TagMinorVersion, reader.GetUInt32());
            var compatibleBrands = new List<string>();
            for (var bytesLeft = atomSize - 8; bytesLeft >= 4; bytesLeft -= 4)
                compatibleBrands.Add(reader.Get4ccString());
            directory.Set(QuickTimeFileTypeDirectory.TagCompatibleBrands, compatibleBrands);
        }
    }
}
