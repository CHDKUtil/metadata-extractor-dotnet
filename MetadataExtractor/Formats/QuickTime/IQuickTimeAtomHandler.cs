// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.IO;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.QuickTime
{
    /// <summary>
    /// Interface of a class capable of handling an individual QuickTime atom.
    /// </summary>
    /// <author>Dmitry Shechtman</author>
    public interface IQuickTimeAtomHandler
    {
        void ProcessAtom(Stream stream, SequentialReader reader, int atomSize);
    }
}
