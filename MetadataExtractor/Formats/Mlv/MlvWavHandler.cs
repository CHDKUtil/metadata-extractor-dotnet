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

using MetadataExtractor.Formats.Wav;
using MetadataExtractor.IO;
using System.Collections.Generic;

namespace MetadataExtractor.Formats.Mlv
{
    /// <author>Dmitry Shechtman</author>
    sealed class MlvWavHandler : MlvBlockHandler<WavFormatDirectory>
    {
        public MlvWavHandler(SortedList<long, Directory> directories)
            : base(directories)
        {
        }

        protected override int MinSize => 32;

        protected override long Populate(WavFormatDirectory directory, SequentialReader reader, int blockSize)
        {
            var stamp = reader.GetInt64();
            WavFormatHandler.Populate(directory, reader, blockSize - 16);
            return stamp;
        }
    }
}
