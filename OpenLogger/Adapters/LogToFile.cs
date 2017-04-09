using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace OpenLogger.Adapters
{
    public class LogToFile : ILog
    {
        // Derived format [path]\[fileNameBase]_[fileCount].[extension]
        private readonly string path;
        private string fileNameBase;
        private string extension;
        private static int fileCount = 0;

        public LogToFile(string path, string fileNameBase, string extension, bool flushPreviousFiles)
        {
            this.path = path;
            this.fileNameBase = fileNameBase;
            this.extension = extension;

            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var files = Directory.GetFiles(path, fileNameBase + "*." + extension);
                foreach (var file in files.Where(file => flushPreviousFiles && File.Exists(file)))
                {
                    File.Delete(file);
                }

                UpdateFileCount(files);
            }
            catch (Exception)
            {
                // Suppress
            }
        }

        private string FileName
        {
            get
            {
                return Path.Combine(path,
                                    fileNameBase + (fileCount > 0 ? "_" + fileCount : string.Empty) + "." +
                                    extension);
            }
        }

        private void UpdateFileCount(IEnumerable<string> files)
        {
            foreach (var text in files.Select(file => (file.Remove(0, fileNameBase.Count() + 1)).Replace("." + extension, string.Empty)))
            {
                int value;
                if (Int32.TryParse(text, out value))
                {
                    if (value > fileCount)
                        fileCount = value;
                }
            }
        }

        public void Log(object sender, LogEventArgs e)
        {
            var message = e.SeverityString + " - [" + e.Timestamp.ToString(CultureInfo.InvariantCulture) + "] (" +
                          e.Origin + ") " + e.Message;

            try
            {
                if (File.Exists(FileName))
                {
                    using (var streamWriter = File.AppendText(FileName))
                    {
                        streamWriter.WriteLine(message);
                    }

                    var info = new FileInfo(FileName);
                    if (info.Length > (50000000)) // ~50 MB
                    {
                        ++fileCount;
                    }
                }
                else
                {
                    using (var streamWriter = File.CreateText(FileName))
                    {
                        streamWriter.WriteLine(message);
                    }
                }
            }
            catch (Exception)
            {
                // Suppress
            }
        }
    }
}
