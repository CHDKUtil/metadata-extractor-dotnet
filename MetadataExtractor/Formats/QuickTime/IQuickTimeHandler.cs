// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.QuickTime
{
    /// <summary>
    /// Interface of a class capable of handling events raised during the reading of a QuickTime file.
    /// </summary>
    /// <author>Dmitry Shechtman</author>
    public interface IQuickTimeHandler
    {
        void ProcessAtom(AtomCallbackArgs a);
    }
}
