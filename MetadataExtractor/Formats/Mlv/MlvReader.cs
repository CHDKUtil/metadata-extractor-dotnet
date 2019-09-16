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
using System.IO;
using System.Text;

namespace MetadataExtractor.Formats.Mlv
{
    /// <summary>
    /// Processes MLV data, calling into client code via <see cref="MlvHandler"/>.
    /// </summary>
    /// <remarks>
    /// Source:
    /// https://www.magiclantern.fm/forum/index.php?topic=7122.0
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class MlvReader
    {
        /// <summary>Processes an MLV data sequence.</summary>
        /// <param name="stream">The <see cref="Stream"/> from which the data should be read.</param>
        /// <param name="handler">The <see cref="MlvHandler"/> that will coordinate processing and accept read values.</param>
        /// <exception cref="MlvProcessingException">An error occurred during the processing of MLV data that could not be ignored or recovered from.</exception>
        /// <exception cref="IOException">an error occurred while accessing the required data</exception>
        public void ProcessMlv(Stream stream, MlvHandler handler)
        {
            var reader = new SequentialStreamReader(stream, isMotorolaByteOrder: false);

            // The total size of the blocks that follow plus 4 bytes for the 'WEBP' or 'AVI ' FourCC
            var sizeLeft = stream.Length;

            // Processing blocks. Each block is 8 bytes header (4 bytes CC code + 4 bytes length of block) + data of the block
            while (reader.Position < sizeLeft)
            {
                // Check if end of the file is closer then 8 bytes
                if (reader.IsCloserToEnd(8))
                    return;

                var startPos = reader.Position;
                var fourCc = reader.GetString(4, Encoding.ASCII);

                if (startPos == 0)
                {
                    if (fourCc != "MLVI")
                        throw new MlvProcessingException($"Invalid MLV header: {fourCc}");
                    fourCc = string.Empty;
                }

                var blockSize = reader.GetInt32();

                // NOTE we fail a negative block size here (greater than 0x7FFFFFFF) as we cannot allocate arrays larger than this
                if (blockSize < 0 || sizeLeft < blockSize)
                    throw new MlvProcessingException("Invalid MLVI block size");

                sizeLeft -= 8;

                // Check if end of the file is closer then blockSize bytes
                if (reader.IsCloserToEnd(blockSize)) return;

                var offset = handler.ProcessBlock(fourCc, reader, blockSize)
                    ? (int)(blockSize - reader.Position + startPos)
                    : blockSize - 8;

                if (offset > 0)
                    reader.Skip(offset);

                sizeLeft -= blockSize;
            }
        }
    }
}
