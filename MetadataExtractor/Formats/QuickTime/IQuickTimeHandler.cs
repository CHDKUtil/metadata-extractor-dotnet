// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.IO;
using System.IO;

namespace MetadataExtractor.Formats.QuickTime
{
    /// <summary>
    /// Interface of a class capable of handling events raised during the reading of a QuickTime file.
    /// </summary>
    /// <author>Dmitry Shechtman</author>
    public interface IQuickTimeHandler
    {
        bool ProcessAtom(string fourCc, Stream stream, SequentialReader reader, long atomSize);
    }
}
