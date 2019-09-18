// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.QuickTime
{
    sealed class QuickTimeMovieHeaderHandler : QuickTimeAtomHandler<QuickTimeMovieHeaderDirectory>
    {
        private static readonly DateTime _epoch = new DateTime(1904, 1, 1);

        public QuickTimeMovieHeaderHandler(List<Directory> directories)
            : base(directories)
        {
        }

        protected override void Populate(QuickTimeMovieHeaderDirectory directory, SequentialReader reader, int atomSize)
        {
            directory.Set(QuickTimeMovieHeaderDirectory.TagVersion, reader.GetByte());
            directory.Set(QuickTimeMovieHeaderDirectory.TagFlags, reader.GetBytes(3));
            directory.Set(QuickTimeMovieHeaderDirectory.TagCreated, _epoch.AddTicks(TimeSpan.TicksPerSecond * reader.GetUInt32()));
            directory.Set(QuickTimeMovieHeaderDirectory.TagModified, _epoch.AddTicks(TimeSpan.TicksPerSecond * reader.GetUInt32()));
            var timeScale = reader.GetUInt32();
            directory.Set(QuickTimeMovieHeaderDirectory.TagTimeScale, timeScale);
            directory.Set(QuickTimeMovieHeaderDirectory.TagDuration, TimeSpan.FromSeconds(reader.GetUInt32() / (double)timeScale));
            directory.Set(QuickTimeMovieHeaderDirectory.TagPreferredRate, reader.Get32BitFixedPoint());
            directory.Set(QuickTimeMovieHeaderDirectory.TagPreferredVolume, reader.Get16BitFixedPoint());
            reader.Skip(10);
            directory.Set(QuickTimeMovieHeaderDirectory.TagMatrix, reader.GetBytes(36));
            directory.Set(QuickTimeMovieHeaderDirectory.TagPreviewTime, reader.GetUInt32());
            directory.Set(QuickTimeMovieHeaderDirectory.TagPreviewDuration, reader.GetUInt32());
            directory.Set(QuickTimeMovieHeaderDirectory.TagPosterTime, reader.GetUInt32());
            directory.Set(QuickTimeMovieHeaderDirectory.TagSelectionTime, reader.GetUInt32());
            directory.Set(QuickTimeMovieHeaderDirectory.TagSelectionDuration, reader.GetUInt32());
            directory.Set(QuickTimeMovieHeaderDirectory.TagCurrentTime, reader.GetUInt32());
            directory.Set(QuickTimeMovieHeaderDirectory.TagNextTrackId, reader.GetUInt32());
        }
    }
}
