#region License
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
    public sealed class MlvFileDirectory : Directory
    {
        public const int TagFormatVersion = 1;
        public const int TagFileGuid = 2;
        public const int TagFileNumber = 3;
        public const int TagFileCount = 4;
        public const int TagFileFlags = 5;
        public const int TagVideoClass = 6;
        public const int TagAudioClass = 7;
        public const int TagVideoFrameCount = 8;
        public const int TagAudioFrameCount = 9;
        public const int TagFps = 10;

        private static readonly string[] _tagNames =
        {
            "Format Version",
            "File GUID",
            "File Number",
            "File Count",
            "File Flags",
            "Video Class",
            "Audio Class",
            "Video Frame Count",
            "Audio Frame Count",
            "FPS"
        };

        public MlvFileDirectory()
        {
            SetDescriptor(new MlvFileDescriptor(this));
        }

        public override string Name => "Magic Lantern Movie File";

        protected override bool TryGetTagName(int tagType, [NotNullWhen(true)] out string? tagName)
        {
            tagName = tagType > 0 && tagType <= _tagNames.Length
                ? _tagNames[tagType - 1]
                : null;
            return tagName != null;
        }
    }
}
