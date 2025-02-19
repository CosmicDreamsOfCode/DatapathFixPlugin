﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace DatapathFix {
    internal class Program {
        static void Main(string[] args) {
            string currentPath = Assembly.GetExecutingAssembly().Location;
            string origPath = currentPath.Replace(".exe", ".orig.exe");

            if (File.Exists("tmp") && File.Exists(origPath)) {
                string dataPathArg = File.ReadAllText("tmp");

                // EA Desktop will always launch without arguments
                if (args.Length == 0) {
                    File.Move(currentPath, currentPath.Replace(".exe", ".old"));
                    File.Move(origPath, currentPath);

                    // Old games require .par file with same name
                    string parPath = origPath.Replace(".exe", ".par");
                    if (File.Exists(parPath)) {
                        File.Delete(parPath);
                    }

                    Console.WriteLine($"Starting '{Path.GetFileName(currentPath)}' with mods");
                    AnyKeyToContinue();

                    try {
                        Process.Start(new ProcessStartInfo {
                            FileName = currentPath,
                            WorkingDirectory = Environment.CurrentDirectory,
                            Arguments = dataPathArg,
                            UseShellExecute = false
                        });
                    }
                    catch (Exception e) {
                        Console.WriteLine($"Error While Launching:");
                        Console.WriteLine(e);
                        AnyKeyToContinue();
                    }
                }

                // if arguments are present, assume it was Frosty attempting to launch. Start game.orig.exe to prompt EAD/etc to launch the game.
                else {
                    Console.WriteLine("Arguments present, assuming it was Frosty attempting to launch");
                    Console.WriteLine($"Starting '{Path.GetFileName(origPath)}' to prompt EA Desktop to launch the game");
                    AnyKeyToContinue();

                    try {
                        Process.Start(new ProcessStartInfo {
                            FileName = origPath,
                            WorkingDirectory = Environment.CurrentDirectory,
                            UseShellExecute = false
                        });
                    }
                    catch (Exception e) {
                        Console.WriteLine($"Error While Launching:");
                        Console.WriteLine(e);
                        AnyKeyToContinue();
                    }
                }
            }
            else {
                if (!File.Exists("tmp"))
                    Console.WriteLine($"Error: 'tmp' does not exist");
                if (!File.Exists(origPath))
                    Console.WriteLine($"Error: '{Path.GetFileName(origPath)}' does not exist");
                AnyKeyToContinue();
            }

            void AnyKeyToContinue() {
#if DEBUG
                Console.WriteLine("");
                Console.Write("Press Any Key to Continue...");
                Console.ReadKey();
#endif
            }
        }
    }
}
