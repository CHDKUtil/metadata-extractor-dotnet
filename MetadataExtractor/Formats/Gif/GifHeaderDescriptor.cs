// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Gif
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class GifHeaderDescriptor : TagDescriptor<GifHeaderDirectory>
    {
        public GifHeaderDescriptor(GifHeaderDirectory directory)
            : base(directory)
        {
        }
    }
}
