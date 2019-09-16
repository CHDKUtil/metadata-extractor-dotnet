﻿#region License
//
// Copyright 2002-2019 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System.Diagnostics.CodeAnalysis;

namespace MetadataExtractor.Formats.Mlv
{
    /// <author>Dmitry Shechtman</author>
    public sealed class MlvExpoDirectory : Directory
    {
        public const int TagIsoMode = 1;
        public const int TagIsoValue = 2;
        public const int TagIsoAnalog = 3;
        public const int TagDigitalGain = 4;
        public const int TagShutterValue = 5;

        private static readonly string[] _tagNames = 
        {
            "ISO Mode",
            "ISO Value",
            "ISO Analog",
            "Digital Gain",
            "Exposure Time"
        };

        public MlvExpoDirectory()
        {
            SetDescriptor(new TagDescriptor<MlvExpoDirectory>(this));
        }

        public override string Name => "Magic Lantern Exposure";

        protected override bool TryGetTagName(int tagType, [NotNullWhen(true)] out string? tagName)
        {
            tagName = tagType > 0 && tagType <= _tagNames.Length
                ? _tagNames[tagType - 1]
                : null;
            return tagName != null;
        }
    }
}
