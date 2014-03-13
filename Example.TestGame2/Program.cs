/*
 * Copyright (c) 2013-2014 Tobias Schulz
 *
 * Copying, redistribution and use of the source code in this file in source
 * and binary forms, with or without modification, are permitted provided
 * that the conditions of the MIT license are met.
 */

using System;
using System.Diagnostics.CodeAnalysis;

using Platform;

namespace Examples.TestGame
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            Dependencies.CatchDllExceptions (() => new TestGame ().Run ());
        }
    }
}
