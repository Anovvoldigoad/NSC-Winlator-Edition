using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.ComponentModel;

namespace NSC.Winlator.Libraries.LibCPK
{
    public class FileEntry
    {
        public object? DirName { get; set; }
        public object? FileName { get; set; }
        public object? FileSize { get; set; }
        public Type? FileSizeType { get; set; }
        public object? ExtractSize { get; set; }
        public Type? ExtractSizeType { get; set; }
        public byte[]? data { get; set; }
    }

    // Stub CPK class - read-only, no write support
    // Use YACpkTool.exe for actual CPK compilation
    public class CPK
    {
        public List<FileEntry> files = new();

        public bool Read(byte[] data)
        {
            return false;
        }

        public byte[] Build()
        {
            return new byte[0];
        }
    }
}
