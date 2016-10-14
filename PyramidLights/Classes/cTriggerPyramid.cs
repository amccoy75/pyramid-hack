﻿using System;
using System.Diagnostics;
using System.IO;


    class cTriggerPyramid
    {
        void trigger(string[] args)
        {
            runPython_cmd(args[0], args[1]);
        }

        public void runPython_cmd(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"C:\cygwin64\bin\ssh.exe";// relaytest.py";
            //start.FileName = "my/full/path/to/python.exe";
            start.Arguments = string.Format("{0} {1}", cmd, args);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }
            }
        }
    }



