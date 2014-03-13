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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace Platform
{
    [ExcludeFromCodeCoverageAttribute]
    public static class Log
    {
        // Windows Performance
        private static string lastDebugStr = "";
        private static int lastDebugTimes = 0;
        // Lists
        private static Dictionary<string, ListDefinition> lists = new Dictionary<string, ListDefinition>();
        private static string lastListId = null;

        public static void Debug(params object[] message)
        {
            EndList();
            if (SystemInfo.IsRunningOnLinux())
            {
                DebugLinux(message);
            }
            else
            {
                DebugWindows(message);
            }
        }

        public static void DebugLinux(params object[] message)
        {
            string str = string.Join("", message);
            Console.WriteLine(str);
        }

        [Conditional ("DEBUG")]
        public static void DebugWindows(params object[] message)
        {
            return;
            string str = string.Join("", message);
            if (str == lastDebugStr)
            {
                ++lastDebugTimes;
                if (lastDebugTimes > 100)
                {
                    Console.WriteLine("[" + lastDebugTimes + "x] " + lastDebugStr);
                    lastDebugTimes = 0;
                }
            }
            else
            {
                if (lastDebugTimes > 0)
                {
                    Console.WriteLine(lastDebugTimes + "x " + lastDebugStr);
                }
                Console.WriteLine(str);
                lastDebugStr = str;
                lastDebugTimes = 0;
            }
        }

        public static void Message(params object[] message)
        {
            EndList();
            Console.WriteLine(string.Join("", message));
        }

        public static void Error(Exception ex)
        {
            EndList();
            Console.WriteLine(ex.ToString());
        }

        public static void ShowMessageBox(string text, string title)
        {
            MessageBox.Show(text, title);
        }

        private static void List(object id, object before, object after, object begin, object end)
        {
            ListDefinition def = new ListDefinition
            {
                Id = id.ToString (),
                Before = before.ToString (),
                After = after.ToString (),
                Begin = begin.ToString (),
                End = end.ToString ()
            };
            lists[def.Id] = def;
        }

        public static void BlockList(object id, object before, object after, object begin, object end)
        {
            string beforeStr = before.ToString();
            string afterStr = after + Environment.NewLine;
            string beginStr = begin + Environment.NewLine;
            string endStr = end.ToString().Length > 0 ? end + Environment.NewLine : "";
            List(id, beforeStr, afterStr, beginStr, endStr);
        }

        public static void InlineList(object id, object before, object after, object begin, object end)
        {
            List(id, before, after, begin, end + Environment.NewLine);
        }

        public static void EndList()
        {
            if (lastListId != null)
            {
                Console.Write(lists[lastListId].End);
                lastListId = null;
            }
        }

        public static void ListElement(object id, string element)
        {
            if (lists.ContainsKey(id.ToString()))
            {
                ListDefinition def = lists[id.ToString()];
                if (lastListId != id.ToString())
                {
                    EndList();
                    Console.Write(def.Begin);
                }
                Console.Write(def.Before);
                Console.Write(element.ToString());
                Console.Write(def.After);

                lastListId = id.ToString();
            }
            else
            {
                Message("Error! Invalid list ID in ListElement (", id, ", ", element, ")");
            }
        }

        public static void ListElement(object id, params object[] element)
        {
            ListElement(id, string.Join("", element));
        }

        private struct ListDefinition
        {
            public string Id;
            public string Before;
            public string After;
            public string Begin;
            public string End;
        }
    }
}
