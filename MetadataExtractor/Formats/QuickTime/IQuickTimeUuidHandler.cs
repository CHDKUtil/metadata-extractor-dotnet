// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.IO;
using System.IO;

namespace MetadataExtractor.Formats.QuickTime
{
    /// <author>Dmitry Shechtman</author>
    public interface IQuickTimeUuidHandler
    {
        bool ProcessUuid(Stream stream, SequentialReader reader, long atomSize);
    }
}
