/*
 * Copyright (c) 2013-2014 Tobias Schulz
 *
 * Copying, redistribution and use of the source code in this file in source
 * and binary forms, with or without modification, are permitted provided
 * that the conditions of the MIT license are met.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Platform
{
    public static partial class SystemInfo
    {
        public static bool IsRunningOnMono ()
        {
            return Type.GetType ("Mono.Runtime") != null;
        }

        public static bool IsRunningOnMonogame ()
        {
            return true;
        }

        public static bool IsRunningOnLinux ()
        {
            return Environment.OSVersion.Platform == PlatformID.Unix;
        }

        public static bool IsRunningOnWindows ()
        {
            return !IsRunningOnLinux ();
        }
    }
}
