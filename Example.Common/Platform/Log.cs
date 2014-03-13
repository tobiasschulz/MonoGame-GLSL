/*
 * Copyright (c) 2013-2014 Tobias Schulz
 *
 * Copying, redistribution and use of the source code in this file in source
 * and binary forms, with or without modification, are permitted provided
 * that the conditions of the MIT license are met.
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

        public static void Debug (params object[] message)
        {
            EndList ();
            if (SystemInfo.IsRunningOnLinux ()) {
                DebugLinux (message);
            }
            else {
                DebugWindows (message);
            }
        }

        public static void DebugLinux (params object[] message)
        {
            string str = string.Join ("", message);
            Console.WriteLine (str);
        }

        [Conditional ("DEBUG")]
        public static void DebugWindows (params object[] message)
        {
            return;
            string str = string.Join ("", message);
            if (str == lastDebugStr) {
                ++lastDebugTimes;
                if (lastDebugTimes > 100) {
                    Console.WriteLine ("[" + lastDebugTimes + "x] " + lastDebugStr);
                    lastDebugTimes = 0;
                }
            }
            else {
                if (lastDebugTimes > 0) {
                    Console.WriteLine (lastDebugTimes + "x " + lastDebugStr);
                }
                Console.WriteLine (str);
                lastDebugStr = str;
                lastDebugTimes = 0;
            }
        }

        public static void Message (params object[] message)
        {
            EndList ();
            Console.WriteLine (string.Join ("", message));
        }

        public static void Error (Exception ex)
        {
            EndList ();
            Console.WriteLine (ex.ToString ());
        }

        public static void ShowMessageBox (string text, string title)
        {
            MessageBox.Show (text, title);
        }

        private static void List (object id, object before, object after, object begin, object end)
        {
            ListDefinition def = new ListDefinition {
                Id = id.ToString (),
                Before = before.ToString (),
                After = after.ToString (),
                Begin = begin.ToString (),
                End = end.ToString ()
            };
            lists [def.Id] = def;
        }

        public static void BlockList (object id, object before, object after, object begin, object end)
        {
            string beforeStr = before.ToString ();
            string afterStr = after + Environment.NewLine;
            string beginStr = begin + Environment.NewLine;
            string endStr = end.ToString ().Length > 0 ? end + Environment.NewLine : "";
            List (id, beforeStr, afterStr, beginStr, endStr);
        }

        public static void InlineList (object id, object before, object after, object begin, object end)
        {
            List (id, before, after, begin, end + Environment.NewLine);
        }

        public static void EndList ()
        {
            if (lastListId != null) {
                Console.Write (lists [lastListId].End);
                lastListId = null;
            }
        }

        public static void ListElement (object id, string element)
        {
            if (lists.ContainsKey (id.ToString ())) {
                ListDefinition def = lists [id.ToString ()];
                if (lastListId != id.ToString ()) {
                    EndList ();
                    Console.Write (def.Begin);
                }
                Console.Write (def.Before);
                Console.Write (element.ToString ());
                Console.Write (def.After);

                lastListId = id.ToString ();
            }
            else {
                Message ("Error! Invalid list ID in ListElement (", id, ", ", element, ")");
            }
        }

        public static void ListElement (object id, params object[] element)
        {
            ListElement (id, string.Join ("", element));
        }

        private struct ListDefinition {
            public string Id;
            public string Before;
            public string After;
            public string Begin;
            public string End;
        }
    }
}
