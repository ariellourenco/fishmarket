using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FishMarket.Api.Helpers;

public static class FileHelper
{
    private static readonly string[] Formats = [".jpg"];

    // For more file signatures, see the File Signatures Database (https://www.filesignatures.net/)
    // and the official specifications for the file types you wish to add.
    private static readonly Dictionary<string, List<byte[]>> Signatures = new()
    {
        { ".jpg", new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 },
            }
        }
    };

    /// <summary>
    /// Determines whether the specified file extension is supported.
    /// </summary>
    /// <param name="filename">The file name and its extension.</param>
    /// <param name="data">The content of the file.</param>
    /// <returns><see langword="true" /> if the extension is supported; <see langword="false" /> otherwise.</returns>
    public static bool IsValidFileExtension(string filename, Stream data)
    {
        if (string.IsNullOrWhiteSpace(filename) || (data == null) || (data.Length == 0))
            return false;

        var extension = Path.GetExtension(filename).ToLowerInvariant();

        if (string.IsNullOrEmpty(extension) || ! Formats.Contains(extension))
            return false;

        data.Position = 0;

        using (var reader = new BinaryReader(data, Encoding.UTF8, leaveOpen: true))
        {
            // Tests the input content's file signature.
            var signature = Signatures[extension];
            var headerBytes = reader.ReadBytes(signature.Max(m => m.Length));

            return signature.Any(signature => headerBytes.Take(signature.Length).SequenceEqual(signature));
        }
    }
}