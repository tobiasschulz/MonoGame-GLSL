/*
 * Copyright (c) 2013-2014 Tobias Schulz
 *
 * Copying, redistribution and use of the source code in this file in source
 * and binary forms, with or without modification, are permitted provided
 * that the conditions of the MIT license are met.
 */

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Platform
{
    [ExcludeFromCodeCoverageAttribute]
    public static partial class SystemInfo
    {
        /// <summary>
        /// Das Einstellungsverzeichnis.
        /// </summary>

        public static string SettingsDirectory
        {
            get {
                if (settingsDirectory != null) {
                    return settingsDirectory;
                }
                else {
                    string directory;
                    if (SystemInfo.IsRunningOnLinux ()) {
                        directory = Environment.GetEnvironmentVariable ("HOME") + "/.knot3/";
                    }
                    else {
                        directory = Environment.GetFolderPath (System.Environment.SpecialFolder.Personal) + "\\Knot3\\";
                    }
                    Directory.CreateDirectory (directory);
                    return settingsDirectory = directory;
                }
            }
            set {
                settingsDirectory = value;
            }
        }

        private static string settingsDirectory = null;

        /// <summary>
        /// Das Spielstandverzeichnis.
        /// </summary>
        public static string SavegameDirectory
        {
            get {
                string directory = SettingsDirectory + "Savegames";
                Directory.CreateDirectory (directory);
                return directory;
            }
        }

        /// <summary>
        /// Das Bildschirmfotoverzeichnis.
        /// </summary>
        public static string ScreenshotDirectory
        {
            get {
                string directory;
                if (SystemInfo.IsRunningOnLinux ()) {
                    directory = Environment.GetEnvironmentVariable ("HOME");
                }
                else {
                    directory = Environment.GetFolderPath (System.Environment.SpecialFolder.MyPictures) + "\\Knot3\\";
                }
                Directory.CreateDirectory (directory);
                return directory;
            }
        }

        public static string DecodedMusicCache
        {
            get {
                if (decodedMusicCache != null) {
                    return decodedMusicCache;
                }
                else {
                    string directory;
                    if (SystemInfo.IsRunningOnLinux ()) {
                        directory = "/var/tmp/knot3/";
                    }
                    else {
                        directory = Environment.GetFolderPath (System.Environment.SpecialFolder.MyMusic) + "\\Knot3\\";
                    }
                    Directory.CreateDirectory (directory);
                    return decodedMusicCache = directory;
                }
            }
            set {
                decodedMusicCache = value;
            }
        }

        private static string decodedMusicCache = null;

        public static string BaseDirectory
        {
            get {
                if (baseDirectory != null) {
                    return baseDirectory;
                }
                else {
                    findBaseDirectory ();
                    return baseDirectory;
                }
            }
        }

        public static string RelativeBaseDirectory
        {
            get {
                if (relativeBaseDirectory != null) {
                    return relativeBaseDirectory;
                }
                else {
                    findBaseDirectory ();
                    return relativeBaseDirectory;
                }
            }
            set {
                Log.Debug ("Set Base directory: ", value);
                baseDirectory = value;
                Log.Debug ("Set Base directory (relative): ", value);
                relativeBaseDirectory = value;
            }
        }

        [ExcludeFromCodeCoverageAttribute]
        private static void findBaseDirectory ()
        {
            string baseDir = Directory.GetCurrentDirectory ();
            string relBaseDir = "." + PathSeparator;
            string[] binDirectories = new string[] {
                "Debug",
                "Release",
                "x86",
                "bin",
                "Game",
                "ModelEditor",
                "Tools",
                "VisualTests",
            };
            foreach (string dir in binDirectories) {
                if (Path.GetFileName (baseDir).ToLower () == dir.ToLower ()) {
                    baseDir = baseDir.Substring (0, baseDir.Length - dir.Length - 1);
                    relBaseDir += ".." + PathSeparator;
                }
            }
            Log.Debug ("Base directory: ", baseDir);
            baseDirectory = baseDir;
            Log.Debug ("Base directory (relative): ", relBaseDir);
            relativeBaseDirectory = relBaseDir;
        }

        private static string relativeBaseDirectory = null;
        private static string baseDirectory = null;
        public readonly static char PathSeparator = Path.DirectorySeparatorChar;

        public static string RelativeContentDirectory
        {
            get {
                return SystemInfo.RelativeBaseDirectory + "Content" + PathSeparator;
            }
        }
    }
}
