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

using System.Collections.Generic;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Mlv
{
    sealed class MlvExpoHandler : MlvBlockHandler<MlvExpoDirectory>
    {
        /// <author>Dmitry Shechtman</author>
        public MlvExpoHandler(SortedList<long, Directory> directories)
            : base(directories)
        {
        }

        protected override int MinSize => 40;

        protected override long Populate(MlvExpoDirectory directory, SequentialReader reader, int blockSize)
        {
            var stamp = reader.GetInt64();
            directory.Set(MlvExpoDirectory.TagIsoMode, reader.GetUInt32());
            directory.Set(MlvExpoDirectory.TagIsoValue, reader.GetUInt32());
            directory.Set(MlvExpoDirectory.TagIsoAnalog, reader.GetUInt32());
            directory.Set(MlvExpoDirectory.TagDigitalGain, reader.GetUInt32());
            directory.Set(MlvExpoDirectory.TagShutterValue, reader.GetUInt64());
            return stamp;
        }
    }
}
