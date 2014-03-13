/*
 * Copyright (c) 2013-2014 Tobias Schulz, Maximilian Reuter, Pascal Knodel,
 *                         Gerd Augsburg, Christina Erler, Daniel Warzel
 *
 * This source code file is part of Knot3. Copying, redistribution and
 * use of the source code in this file in source and binary forms,
 * with or without modification, are permitted provided that the conditions
 * of the MIT license are met:
 *
 *   Permission is hereby granted, free of charge, to any person obtaining a copy
 *   of this software and associated documentation files (the "Software"), to deal
 *   in the Software without restriction, including without limitation the rights
 *   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *   copies of the Software, and to permit persons to whom the Software is
 *   furnished to do so, subject to the following conditions:
 *
 *   The above copyright notice and this permission notice shall be included in all
 *   copies or substantial portions of the Software.
 *
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 *
 * See the LICENSE file for full license details of the Knot3 project.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Platform
{
    /// <summary>
    /// Eine Hilfsklasse für Dateioperationen.
    /// </summary>
    public static class FileUtility
    {
        /// <summary>
        /// Konvertiert einen Namen eines Knotens oder einer Challenge in einen gültigen Dateinamen durch Weglassen ungültiger Zeichen.
        /// </summary>
        public static string ConvertToFileName (string name)
        {
            char[] arr = name.ToCharArray ();
            arr = Array.FindAll<char> (arr, (c => (char.IsLetterOrDigit (c)
                                                   || char.IsWhiteSpace (c)
                                                   || c == '-'))
                                      );
            return new string (arr);
        }

        /// <summary>
        /// Liefert einen Hash-Wert zu der durch filename spezifizierten Datei.
        /// </summary>
        public static string GetHash (string filename)
        {
            return string.Join ("\n", FileUtility.ReadFrom (filename)).ToMD5Hash ();
        }

        public static string ToMD5Hash (this string TextToHash)
        {
            if (string.IsNullOrEmpty (TextToHash)) {
                return string.Empty;
            }

            MD5 md5 = new MD5CryptoServiceProvider ();
            byte[] textToHash = Encoding.Default.GetBytes (TextToHash);
            byte[] result = md5.ComputeHash (textToHash);

            return System.BitConverter.ToString (result);
        }

        public static IEnumerable<string> ReadFrom (string file)
        {
            string line;
            using (var reader = File.OpenText (file)) {
                while ((line = reader.ReadLine ()) != null) {
                    yield return line;
                }
            }
        }

        public static void SearchFiles (IEnumerable<string> directories, IEnumerable<string> extensions, Action<string> add)
        {
            foreach (string directory in directories) {
                SearchFiles (directory, extensions, add);
            }
        }

        public static void SearchFiles (string directory, IEnumerable<string> extensions, Action<string> add)
        {
            Directory.CreateDirectory (directory);
            var files = Directory.GetFiles (directory, "*.*", SearchOption.AllDirectories)
                        .Where (s => extensions.Any (e => s.EndsWith (e)));
            foreach (string file in files) {
                add (file);
            }
        }
    }
}
