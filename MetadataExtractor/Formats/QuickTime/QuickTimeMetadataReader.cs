// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using MetadataExtractor.IO;

#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace MetadataExtractor.Formats.QuickTime
{
    sealed class QuickTimeTrackHeaderHandler : QuickTimeAtomHandler<QuickTimeTrackHeaderDirectory>
    {
        private static readonly DateTime _epoch = new DateTime(1904, 1, 1);

        public QuickTimeTrackHeaderHandler(List<Directory> directories)
            : base(directories)
        {
        }

        protected override void Populate(QuickTimeTrackHeaderDirectory directory, SequentialReader reader, int atomSize)
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

    sealed class QuickTimeTrackHandler : QuickTimeHandler
    {
        public QuickTimeTrackHandler(List<Directory> directories)
            : base(directories, new Dictionary<string, Func<List<Directory>, IQuickTimeAtomHandler>>
            {
                { "tkhd", d => new QuickTimeTrackHeaderHandler(d) }
            })
        {
        }
    }

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

    sealed class QuickTimeMovieHandler : QuickTimeHandler
    {
        public QuickTimeMovieHandler(List<Directory> directories)
            : base(directories, new Dictionary<string, Func<List<Directory>, IQuickTimeAtomHandler>>
            {
                { "mvhd", d => new QuickTimeMovieHeaderHandler(d) },
                { "trak", d => new QuickTimeTrackHandler(d) }
            })
        {
        }
    }

    sealed class QuickTimeFileTypeHandler : QuickTimeAtomHandler<QuickTimeFileTypeDirectory>
    {
        public QuickTimeFileTypeHandler(List<Directory> directories)
            : base(directories)
        {
        }

        protected override void Populate(QuickTimeFileTypeDirectory directory, SequentialReader reader, int atomSize)
        {
            directory.Set(QuickTimeFileTypeDirectory.TagMajorBrand, reader.Get4ccString());
            directory.Set(QuickTimeFileTypeDirectory.TagMinorVersion, reader.GetUInt32());
            var compatibleBrands = new List<string>();
            for (var bytesLeft = atomSize - 8; bytesLeft >= 4; bytesLeft -= 4)
                compatibleBrands.Add(reader.Get4ccString());
            directory.Set(QuickTimeFileTypeDirectory.TagCompatibleBrands, compatibleBrands);
        }
    }

    sealed class QuickTimeRootHandler : QuickTimeHandler
    {
        public QuickTimeRootHandler(List<Directory> directories)
            : base(directories, new Dictionary<string, Func<List<Directory>, IQuickTimeAtomHandler>>
            {
                { "moov", d => new QuickTimeMovieHandler(d) },
                { "ftyp", d => new QuickTimeFileTypeHandler(d) }
            })
        {
        }
    }

    public static class QuickTimeMetadataReader
    {
        public static DirectoryList ReadMetadata(Stream stream)
        {
            var directories = new List<Directory>();
            QuickTimeReader.ProcessAtoms(stream, new QuickTimeRootHandler(directories));
            return directories;
        }
    }
}
