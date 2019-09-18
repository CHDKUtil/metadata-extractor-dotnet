// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.IO;
using System.Text;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.QuickTime
{
    /// <summary>
    /// Static class for processing atoms the QuickTime container format.
    /// </summary>
    /// <remarks>
    /// QuickTime file format specification: https://developer.apple.com/library/mac/documentation/QuickTime/QTFF/qtff.pdf
    /// </remarks>
    public static class QuickTimeReader
    {
        /// <summary>
        /// Reads atom data from <paramref name="stream"/>, invoking <paramref name="handler"/> for each atom encountered.
        /// </summary>
        /// <param name="stream">The stream to read atoms from.</param>
        /// <param name="handler">The handler to handle each atom.</param>
        /// <param name="stopByBytes">The maximum number of bytes to process before discontinuing.</param>
        public static void ProcessAtoms(Stream stream, IQuickTimeHandler handler, long stopByBytes = -1)
        {
            var reader = new SequentialStreamReader(stream);

            var seriesStartPos = stream.Position;

            while (stopByBytes == -1 || stream.Position < seriesStartPos + stopByBytes)
            {
                var atomStartPos = stream.Position;

                try
                {
                    // Check if the end of the stream is closer then 8 bytes to current position (Length of the atom's data + atom type)
                    if (reader.IsCloserToEnd(8))
                        return;

                    // Length of the atom's data, in bytes, including size bytes
                    long atomSize = reader.GetUInt32();

                    // Typically four ASCII characters, but may be non-printable.
                    // By convention, lowercase 4CCs are reserved by Apple.
                    var fourCc = reader.GetString(4, Encoding.UTF8);

                    if (atomSize == 1)
                    {
                        // Check if the end of the stream is closer then 8 bytes
                        if (reader.IsCloserToEnd(8))
                            return;

                        // Size doesn't fit in 32 bits so read the 64 bit size here
                        atomSize = checked((long)reader.GetUInt64());
                    }
                    else if (atomSize < 8)
                    {
                        // Atom should be at least 8 bytes long
                        return;
                    }

                    if (!handler.ProcessAtom(fourCc, stream, reader, atomSize - 8))
                        return;

                    if (atomSize == 0)
                        return;

                    var toSkip = atomStartPos + atomSize - stream.Position;

                    if (toSkip < 0)
                        throw new Exception("Handler moved stream beyond end of atom");

                    // To avoid exception handling we can check if needed number of bytes are available
                    if (!reader.IsCloserToEnd(toSkip))
                        reader.TrySkip(toSkip);
                }
                catch (IOException)
                {
                    // Exception trapping is used when stream doesn't support stream length method only
                    return;
                }
            }
        }
    }
}
