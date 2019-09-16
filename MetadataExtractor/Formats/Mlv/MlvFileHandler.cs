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

using MetadataExtractor.IO;
using System.Collections.Generic;
using System.Text;

namespace MetadataExtractor.Formats.Mlv
{
    /// <author>Dmitry Shechtman</author>
    sealed class MlvFileHandler : MlvBlockHandler<MlvFileDirectory>
    {
        public MlvFileHandler(SortedList<long, Directory> directories)
            : base(directories)
        {
        }

        protected override long Populate(MlvFileDirectory directory, SequentialReader reader, int blockSize)
        {
            directory.Set(MlvFileDirectory.TagFormatVersion, reader.GetString(8, Encoding.ASCII).TrimEnd('\0'));
            directory.Set(MlvFileDirectory.TagFileGuid, reader.GetUInt64());
            directory.Set(MlvFileDirectory.TagFileNumber, reader.GetUInt16());
            directory.Set(MlvFileDirectory.TagFileCount, reader.GetUInt16());
            directory.Set(MlvFileDirectory.TagFileFlags, reader.GetUInt32());
            directory.Set(MlvFileDirectory.TagVideoClass, reader.GetUInt16());
            directory.Set(MlvFileDirectory.TagAudioClass, reader.GetUInt16());
            directory.Set(MlvFileDirectory.TagVideoFrameCount, reader.GetUInt32());
            directory.Set(MlvFileDirectory.TagAudioFrameCount, reader.GetUInt32());
            directory.Set(MlvFileDirectory.TagFps, GetFps(reader));
            return long.MinValue;
        }

        private static Rational GetFps(SequentialReader reader)
        {
            var num = reader.GetUInt32();
            var denom = reader.GetUInt32();
            return new Rational(num, denom);
        }

        protected override int MinSize => 52;
    }
}
