// Copyright (c) 2026 Pilz GmbH & Co. KG
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice (including the next paragraph) shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// SPDX-License-Identifier: MIT

using Pilz.PITreader.PitTransponderManager.Model;

namespace Pilz.PITreader.PitTransponderManager.Import
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                RunInteractive();
                return;
            }

            if (args[0] == "--online" || args[0] == "-online")
            {
                if (args.Length < 2)
                {
                    Console.Error.WriteLine("Usage: PtmCsvTransponderImport --online <csv-file>");
                    Environment.Exit(1);
                }

                RunOnline(args[1]);
            }
            else
            {
                if (args.Length < 2)
                {
                    Console.Error.WriteLine("Usage: PtmCsvTransponderImport <csv-file> <output.ptm> [input.ptm]");
                    Environment.Exit(1);
                }

                string csvPath = args[0];
                string outputPath = args[1];
                string? inputPath = args.Length > 2 ? args[2] : null;

                RunOffline(csvPath, outputPath, inputPath);
            }
        }

        private static void RunOffline(string csvPath, string outputPath, string? inputPath)
        {
            ValidateFileExists(csvPath, "CSV file");

            PtmExportContainer? inputData = null;
            if (!string.IsNullOrEmpty(inputPath))
            {
                ValidateFileExists(inputPath, "Input PTM file");
                inputData = PtmSerializer.Load(inputPath);
            }

            var result = CsvImporter.Import(csvPath, inputData);
            PtmSerializer.Save(result, outputPath);
            Console.WriteLine($"Written to {outputPath}");
        }

        private static void RunOnline(string csvPath)
        {
            ValidateFileExists(csvPath, "CSV file");

            using var appController = new PtmApplication();

            if (!appController.IsRunning)
            {
                Console.WriteLine("Starting PIT Transponder Manager...");
                appController.Start();
                if (!appController.WaitForRunning(TimeSpan.FromSeconds(15)))
                {
                    Console.Error.WriteLine("Error: PIT Transponder Manager did not start within 15 seconds.");
                    Environment.Exit(1);
                }
            }

            var client = new PtmRestApi();
            var inputData = client.GetExportAsync().GetAwaiter().GetResult();

            var result = CsvImporter.Import(csvPath, inputData);
            var apiResult = client.DoImportAsync(result, true).GetAwaiter().GetResult();

            if (apiResult?.ImportErrorMessages?.Count > 0)
            {
                Console.Error.WriteLine("Import errors:");
                foreach (var msg in apiResult.ImportErrorMessages)
                    Console.Error.WriteLine($"  {msg.ErrorMessage}");
                Environment.Exit(1);
            }

            Console.WriteLine("Import to PIT Transponder Manager successful.");
        }

        private static void RunInteractive()
        {
            Console.WriteLine("=== PIT Transponder Manager - CSV Import ===");
            Console.WriteLine();
            Console.WriteLine("This tool reads transponder UIDs from a CSV file, computes Security IDs");
            Console.WriteLine("and generates a .ptm file for import into PIT Transponder Manager (PTM).");
            Console.WriteLine();
            Console.WriteLine("Expected CSV format (header row required, separator: ',' or ';'):");
            Console.WriteLine();
            Console.WriteLine("  uid;startDate;endDate");
            Console.WriteLine("  04A1B2C3D4E5F6;;");
            Console.WriteLine("  0102030405060708;2025-1-1;2026-12-31");
            Console.WriteLine();
            Console.WriteLine("  uid          - Transponder UID as hex (required, 4-10 bytes)");
            Console.WriteLine("  startDate    - (optional) Validity start, YYYY-M-D");
            Console.WriteLine("  endDate      - (optional) Validity end, YYYY-M-D");
            Console.WriteLine();
            Console.WriteLine("UID byte order should be MSB-first, but is auto-detected where possible.");
            Console.WriteLine();
            Console.WriteLine("This tool can also be called non-interactive with parameters.");
            Console.WriteLine("Direct import into PIT Transponder Manager: PtmCsvTransponderImport --online <csv-file>");
            Console.WriteLine("Just conversion from CSV to ptm file:       PtmCsvTransponderImport <csv-file> <output.ptm> [input.ptm]");
            Console.WriteLine(new string('-', 60));
            Console.WriteLine();

            string csvPath = PromptFilePath("Drag CSV file here or type path: ");
            ValidateFileExists(csvPath, "CSV file");

            string defaultOutput = Path.Combine(
                Path.GetDirectoryName(csvPath) ?? ".",
                Path.GetFileNameWithoutExtension(csvPath) + ".ptm");

            string outputPath = PromptWithDefault($"Output .ptm file path [{defaultOutput}]: ", defaultOutput);

            PtmExportContainer? inputData = null;
            Console.Write("Merge with existing .ptm file? (y/N): ");
            var mergeAnswer = Console.ReadLine()?.Trim();
            if (string.Equals(mergeAnswer, "y", StringComparison.OrdinalIgnoreCase))
            {
                string inputPath = PromptFilePath("Drag existing .ptm file here or type path: ");
                ValidateFileExists(inputPath, "Input PTM file");
                inputData = PtmSerializer.Load(inputPath);
            }

            Console.WriteLine();
            var result = CsvImporter.Import(csvPath, inputData);
            PtmSerializer.Save(result, outputPath);
            Console.WriteLine($"Written to {outputPath}");

            Console.WriteLine();
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        private static string PromptFilePath(string prompt)
        {
            Console.Write(prompt);
            var input = Console.ReadLine()?.Trim() ?? string.Empty;
            // Strip surrounding quotes (drag-and-drop in Windows Explorer)
            if (input.Length >= 2 && input[0] == '"' && input[^1] == '"')
                input = input[1..^1];
            return input;
        }

        private static string PromptWithDefault(string prompt, string defaultValue)
        {
            Console.Write(prompt);
            var input = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(input))
                return defaultValue;
            if (input.Length >= 2 && input[0] == '"' && input[^1] == '"')
                input = input[1..^1];
            return input;
        }

        private static void ValidateFileExists(string path, string description)
        {
            if (!File.Exists(path))
            {
                Console.Error.WriteLine($"Error: {description} not found: {path}");
                Environment.Exit(1);
            }
        }
    }
}
