using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.ComponentModel;

namespace NSC.Winlator.Libraries.LibCPK
{
    // Stub CPK class - read-only, no write support
    // Use YACpkTool.exe for actual CPK compilation
    public class CPK
    {
        public List<FileEntry> files = new();

        public bool Read(byte[] data)
        {
            // TODO: implement CPK reading
            return false;
        }

        public byte[] Build()
        {
            return new byte[0];
        }

        public class FileEntry
        {
            public string? FileName { get; set; }
            public byte[]? data { get; set; }
        }
    }
}
