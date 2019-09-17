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
using System;
using System.Collections.Generic;

namespace MetadataExtractor.Formats.Mlv
{
    /// <summary>
    /// Handles events raised during the reading of an MLV file via <see cref="MlvReader"/>.
    /// </summary>
    /// <remarks>
    /// Source:
    /// https://www.magiclantern.fm/forum/index.php?topic=7122.0
    /// </remarks>
    /// <author>Dmitry Shechtman</author>
    public sealed class MlvHandler
    {
        private readonly SortedList<long, Directory> directories;
        private readonly Dictionary<string, Func<SortedList<long, Directory>, IMlvBlockHandler>> handlers;

        public MlvHandler(SortedList<long, Directory> directories)
        {
            this.directories = directories;
            handlers = new Dictionary<string, Func<SortedList<long, Directory>, IMlvBlockHandler>>
            {
                { string.Empty, d => new MlvFileHandler(d) },
                { "IDNT", d => new MlvCameraHandler(d) },
                { "EXPO", d => new MlvExpoHandler(d) },
                { "LENS", d => new MlvLensHandler(d) },
                { "VERS", d => new MlvVersionHandler(d) },
                { "WAVI", d => new MlvWavHandler(d) },
                { "AUDF", d => MlvStopHandler.Instance },
                { "VIDF", d => MlvStopHandler.Instance },
            };
        }

        public bool ProcessBlock(string fourCc, SequentialReader reader, int blockSize)
        {
            if (!handlers.TryGetValue(fourCc, out Func<SortedList<long, Directory>, IMlvBlockHandler> createHandler))
                return false;
            var handler = createHandler(directories);
            return handler.ProcessBlock(reader, blockSize);
        }
    }
}
