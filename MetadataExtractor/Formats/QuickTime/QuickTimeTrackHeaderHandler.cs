// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.QuickTime
{
    sealed class QuickTimeTrackHeaderHandler : QuickTimeAtomHandler<QuickTimeTrackHeaderDirectory>
    {
        private static readonly DateTime _epoch = new DateTime(1904, 1, 1);

        public QuickTimeTrackHeaderHandler(List<Directory> directories)
            : base(directories)
        {
        }

        protected override void Populate(QuickTimeTrackHeaderDirectory directory, SequentialReader reader, long atomSize)
        {
            directory.Set(QuickTimeTrackHeaderDirectory.TagVersion, reader.GetByte());
            directory.Set(QuickTimeTrackHeaderDirectory.TagFlags, reader.GetBytes(3));
            directory.Set(QuickTimeTrackHeaderDirectory.TagCreated, _epoch.AddTicks(TimeSpan.TicksPerSecond * reader.GetUInt32()));
            directory.Set(QuickTimeTrackHeaderDirectory.TagModified, _epoch.AddTicks(TimeSpan.TicksPerSecond * reader.GetUInt32()));
            directory.Set(QuickTimeTrackHeaderDirectory.TagTrackId, reader.GetUInt32());
            reader.Skip(4L);
            directory.Set(QuickTimeTrackHeaderDirectory.TagDuration, reader.GetUInt32());
            reader.Skip(8L);
            directory.Set(QuickTimeTrackHeaderDirectory.TagLayer, reader.GetUInt16());
            directory.Set(QuickTimeTrackHeaderDirectory.TagAlternateGroup, reader.GetUInt16());
            directory.Set(QuickTimeTrackHeaderDirectory.TagVolume, reader.Get16BitFixedPoint());
            reader.Skip(2L);
            directory.Set(QuickTimeTrackHeaderDirectory.TagMatrix, reader.GetMatrix());
            directory.Set(QuickTimeTrackHeaderDirectory.TagWidth, reader.Get32BitFixedPoint());
            directory.Set(QuickTimeTrackHeaderDirectory.TagHeight, reader.Get32BitFixedPoint());
            SetRotation(directory);
        }

        static void SetRotation(QuickTimeTrackHeaderDirectory directory)
        {
            var width = directory.GetInt32(QuickTimeTrackHeaderDirectory.TagWidth);
            var height = directory.GetInt32(QuickTimeTrackHeaderDirectory.TagHeight);
            if (width == 0 || height == 0 || directory.GetObject(QuickTimeTrackHeaderDirectory.TagRotation) != null) return;

            if (directory.GetObject(QuickTimeTrackHeaderDirectory.TagMatrix) is float[] matrix && matrix.Length > 5)
            {
                var x = matrix[1] + matrix[4];
                var y = matrix[0] + matrix[3];
                var theta = Math.Atan2(x, y);
                var degree = ((180 / Math.PI) * theta) - 45;
                if (degree < 0)
                    degree += 360;

                directory.Set(QuickTimeTrackHeaderDirectory.TagRotation, degree);
            }
        }
    }
}
