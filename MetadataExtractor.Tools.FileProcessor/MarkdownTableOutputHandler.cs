// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Exif;

namespace MetadataExtractor.Tools.FileProcessor
{
    /// <summary>
    /// Creates a table describing sample images using Wiki markdown.
    /// <para/>
    /// Output hosted at: https://github.com/drewnoakes/metadata-extractor-images/wiki/ContentSummary
    /// </summary>
    internal class MarkdownTableOutputHandler : FileHandlerBase
    {
        // TODO this should be modelled centrally
        private readonly Dictionary<string, string> _extensionEquivalence = new Dictionary<string, string> { { "jpeg", "jpg" }, { "tiff", "tif" } };
        private readonly Dictionary<string, List<Row>> _rowsByExtension = new Dictionary<string, List<Row>>();

        private class Row
        {
            public string FilePath { get; }
            public string RelativePath { get; }
            public int DirectoryCount { get; }
            public string? Manufacturer { get; }
            public string? Model { get; }
            public string? ExifVersion { get; }
            public string? Thumbnail { get; }
            public string? Makernote { get; }

            internal Row(string filePath, ICollection<Directory> directories, string relativePath)
            {
                FilePath = filePath;
                RelativePath = relativePath;
                DirectoryCount = directories.Count;

                var ifd0Dir = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
                var subIfdDir = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                var thumbDir = directories.OfType<ExifThumbnailDirectory>().FirstOrDefault();

                if (ifd0Dir != null)
                {
                    Manufacturer = ifd0Dir.GetDescription(ExifDirectoryBase.TagMake);
                    Model = ifd0Dir.GetDescription(ExifDirectoryBase.TagModel);
                }

                var hasMakernoteData = false;
                if (subIfdDir != null)
                {
                    ExifVersion = subIfdDir.GetDescription(ExifDirectoryBase.TagExifVersion);
                    hasMakernoteData = subIfdDir.ContainsTag(ExifDirectoryBase.TagMakernote);
                }

                if (thumbDir != null)
                {
                    Thumbnail = thumbDir.TryGetInt32(ExifDirectoryBase.TagImageWidth, out int width) &&
                                thumbDir.TryGetInt32(ExifDirectoryBase.TagImageHeight, out int height)
                        ? $"Yes ({width} x {height})"
                        : "Yes";
                }

                foreach (var directory in directories)
                {
                    if (directory.GetType().Name.Contains("Makernote"))
                    {
                        Makernote = directory.Name.Replace("Makernote", "").Trim();
                        break;
                    }
                }

                if (Makernote is null)
                    Makernote = hasMakernoteData ? "(Unknown)" : "N/A";
            }
        }

        public override void OnExtractionSuccess(string filePath, IList<Directory> directories, string relativePath, TextWriter log, long streamPosition)
        {
            base.OnExtractionSuccess(filePath, directories, relativePath, log, streamPosition);

            var extension = Path.GetExtension(filePath);

            if (extension.Length == 0)
                return;

            // Sanitise the extension
            extension = extension.ToLower();
            if (_extensionEquivalence.ContainsKey(extension))
                extension = _extensionEquivalence[extension];

            if (!_rowsByExtension.TryGetValue(extension, out List<Row>? rows))
            {
                rows = new List<Row>();
                _rowsByExtension[extension] = rows;
            }

            rows.Add(new Row(filePath, directories, relativePath));
        }

        public override void OnScanCompleted(TextWriter log)
        {
            base.OnScanCompleted(log);

            using var stream = File.OpenWrite("ContentSummary.md");
            using var writer = new StreamWriter(stream);
            WriteOutput(writer);
        }

        private void WriteOutput(TextWriter writer)
        {
            writer.WriteLine("# Image Database Summary");
            writer.WriteLine();

            foreach (var extension in _rowsByExtension.Keys)
            {
                writer.WriteLine($"## {extension.ToUpper()} Files");
                writer.WriteLine();

                writer.Write("File|Manufacturer|Model|Dir Count|Exif?|Makernote|Thumbnail|All Data\n");
                writer.Write("----|------------|-----|---------|-----|---------|---------|--------\n");

                var rows = _rowsByExtension[extension];

                // Order by manufacturer, then model
                rows.Sort((o1, o2) =>
                {
                    var c1 = string.CompareOrdinal(o1.Manufacturer, o2.Manufacturer);
                    return c1 != 0
                        ? c1
                        : string.CompareOrdinal(o1.Model, o2.Model);
                });

                foreach (var row in rows)
                {
                    var fileName = Path.GetFileName(row.FilePath);
                    var urlEncodedFileName = Uri.EscapeDataString(fileName).Replace("%20", "+");

                    writer.WriteLine(
                        "[{0}](https://raw.githubusercontent.com/drewnoakes/metadata-extractor-images/master/{1}/{2})|{3}|{4}|{5}|{6}|{7}|{8}|[metadata](https://raw.githubusercontent.com/drewnoakes/metadata-extractor-images/master/{9}/metadata/{10}.txt)",
                        fileName,
                        row.RelativePath,
                        urlEncodedFileName,
                        row.Manufacturer,
                        row.Model,
                        row.DirectoryCount,
                        row.ExifVersion,
                        row.Makernote,
                        row.Thumbnail,
                        row.RelativePath,
                        urlEncodedFileName.ToLower());
                }

                writer.WriteLine();
            }
        }
    }
}
