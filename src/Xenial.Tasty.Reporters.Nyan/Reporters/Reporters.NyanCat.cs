﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Xenial.Delicious.Metadata;
using Xenial.Delicious.Scopes;

namespace Xenial.Delicious.Reporters
{
    public static class NyanCatReporter
    {
        private const int colorCount = 6 * 7;
        private static readonly int nyanCatWidth;
        private static readonly int width;
        private static int colorIndex;
        private static readonly int numberOfLines;
        private static readonly Color[]? rainbowColors;
        private static bool tick;
        private static readonly List<List<string>>? trajectories;
        private static readonly int trajectoryWidthMax;

        static NyanCatReporter()
        {
            if (!HasValidConsole)
            {
                return;
            }

            Pastel.ConsoleExtensions.Enable();

            nyanCatWidth = 11;
            width = (int)(Console.WindowWidth * 0.75);

            colorIndex = 0;
            numberOfLines = Enum.GetValues(typeof(TestOutcome)).Length;

            rainbowColors = GenerateColors();
            tick = false;

            trajectories = new List<List<string>>();

            for (var i = 0; i < numberOfLines; i++)
            {
                trajectories.Add(new List<string>());
            }

            trajectoryWidthMax = width - nyanCatWidth;

            if (trajectoryWidthMax < 0)
            {
                trajectoryWidthMax = 0;
            }
        }

        public static TastyScope RegisterNyanReporter(this TastyScope scope)
         => (scope ?? throw new ArgumentNullException(nameof(scope))).RegisterReporter(Report)
                 .RegisterReporter(ReportSummary);

        public static TastyScope Register()
            => Tasty.TastyDefaultScope.RegisterNyanReporter();

        private static bool? hasValidConsole;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "We don't care about which exception is thrown")]
        private static bool HasValidConsole
        {
            get
            {
                if (!hasValidConsole.HasValue)
                {
                    try
                    {
                        hasValidConsole = Console.WindowWidth > 0;
                    }
                    catch
                    {
                        hasValidConsole = false;
                    }
                }

                return hasValidConsole.Value;
            }
        }

        private static Task ReportSummary(IEnumerable<TestCaseResult> tests)
        {
            Console.WriteLine();
            Console.ForegroundColor = ColorScheme.Default.DefaultColor;
            Console.WriteLine($"\t{tests.Count()} total ({(int)Math.Round(TimeSpan.FromTicks(tests.Sum(t => t.Duration.Ticks)).TotalSeconds, 0, MidpointRounding.AwayFromZero)} s)");

            Console.ForegroundColor = ColorScheme.Default.SuccessColor;
            var successCount = tests.Count(t => t.TestOutcome == TestOutcome.Success);
            if (successCount > 0) { Console.WriteLine($"\t{ColorScheme.Default.SuccessIcon} {successCount} passing"); }

            Console.ForegroundColor = ColorScheme.Default.ErrorColor;
            var failedCount = tests.Count(t => t.TestOutcome == TestOutcome.Failed);
            if (failedCount > 0) { Console.WriteLine($"\t{ColorScheme.Default.ErrorIcon} {failedCount} failed"); }

            Console.ForegroundColor = ColorScheme.Default.SuccessColor;
            if (tests.Count() == successCount) { Console.WriteLine($"\t{ColorScheme.Default.SuccessIcon} All tests passed"); }

            if (failedCount > 0)
            {
                Console.ForegroundColor = ColorScheme.Default.ErrorColor;
                Console.WriteLine($"\t{ColorScheme.Default.ErrorIcon} Failed tests:");

                foreach (var fail in tests.Where(t => t.TestOutcome == TestOutcome.Failed))
                {
                    Console.WriteLine($"\t\t{fail.FullName}");
                    Console.WriteLine($"\t\t\t{fail.AdditionalMessage}");
                }
            }

            return Task.CompletedTask;
        }

        public static Task Report(TestCaseResult test)
        {
            if (!HasValidConsole)
            {
                return Task.CompletedTask;
            }

            Draw(test);

            return Task.CompletedTask;
        }

        private static Color[] GenerateColors()
        {
            var colors = new Color[colorCount];
            var progress = 0f;
            var step = 1f / colorCount;

            for (var i = 0; i < colorCount; i++)
            {
                colors[i] = Rainbow(progress);

                progress += step;
            }

            return colors;
        }

        private static Color Rainbow(float progress)
        {
            var div = (Math.Abs(progress % 1) * 6);
            var ascending = (int)((div % 1) * 255);
            var descending = 255 - ascending;

            return ((int)div) switch
            {
                0 => Color.FromArgb(255, 255, ascending, 0),
                1 => Color.FromArgb(255, descending, 255, 0),
                2 => Color.FromArgb(255, 0, 255, ascending),
                3 => Color.FromArgb(255, 0, descending, 255),
                4 => Color.FromArgb(255, ascending, 0, 255),
                _ => Color.FromArgb(255, 255, 0, descending),
            };
        }

        private static void Draw(TestCaseResult testCase)
        {
            Console.CursorTop = 0;
            Console.CursorLeft = 0;

            tick = !tick;

            for (var i = 0; i < numberOfLines; i++)
            {
                if (trajectories![i].Count > trajectoryWidthMax)
                {
                    foreach (var traj in trajectories)
                    {
                        traj.RemoveAt(0);
                    }
                }

                trajectories[i].Add(AppendRainbow());

            }

            var catIndex = 0;
            var cat = GetNyanCat(testCase).ToList();

            foreach (var traj in trajectories!)
            {
                Console.WriteLine(string.Join(string.Empty, traj) + cat[catIndex]);
                catIndex++;
            }
        }

        private static string AppendRainbow() => Rainbowify(tick ? "_" : "-");

        private static string Rainbowify(string input)
        {
            var color = rainbowColors![colorIndex % rainbowColors.Length];

            var result = Pastel.ConsoleExtensions.Pastel(input, color);

            colorIndex += 1;

            return result;
        }

        private static IEnumerable<string> GetNyanCat(TestCaseResult testCase)
        {
            yield return " _,------,";
            yield return " _|   /\\_/\\";
            yield return " ^|__" + Face(testCase);
            yield return "   \"\"  \"\" ";
        }

        private static string Face(TestCaseResult testCase)
            => testCase.TestOutcome switch
            {
                TestOutcome.NotRun => "( o .o)",
                TestOutcome.Ignored => "( - .-)",
                TestOutcome.Failed => "( x .x)",
                TestOutcome.Success => "( ^ .^)",
                _ => "( - .-)",
            };
    }
}
