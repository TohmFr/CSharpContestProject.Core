using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CSharpContestProject.Core
{
    public class Loader : IDisposable
    {
        Assembly assembly;
        MethodInfo mainMethod;

        TextWriter ConsoleOut;
        TextWriter ConsoleErr;
        TextReader ConsoleIn;

        StringBuilder sbOut;
        StringBuilder sbErr;
        TextWriter InterceptConsoleOut;
        TextWriter InterceptConsoleErr;
        StreamReader InterceptConsoleIn = null;

        Task mainTask;
        List<(string input, string output)> testsFiles;
        public void Run()
        {
            LoadAssembly();            
            FindMain();
            LoadTestsFiles();

            LaunchTests();
        }

       

        internal void LoadAssembly()
        {
            var directory =  Directory.GetCurrentDirectory();
            var files = Directory.GetFiles(directory, "*.dll")
                                 .Select(file => new FileInfo(file))
                                 .ToList()
                                 .OrderByDescending(file => file.LastWriteTime)
                                 .ToList();
            var testProgram = files.First(file => file.Name.Contains("CSharpContest") && !file.Name.Contains(".Core"));

            assembly = Assembly.LoadFrom(testProgram.FullName);

            //assembly = Assembly.LoadFrom();
        }

       

        internal void FindMain()
        {
            foreach(var type in assembly.GetTypes())
            {
                var methode = type.GetMethod("Main", BindingFlags.Static | BindingFlags.NonPublic);
                if(methode != null)
                {
                    mainMethod = methode;
                }
            }
        }

        internal void LoadTestsFiles()
        {
            var directory = Directory.GetCurrentDirectory();
            var files = Directory.GetFiles(directory, "input*.txt");

            testsFiles = new List<(string input, string output)>();
            foreach(var file in files)
            {
                testsFiles.Add((file, file.Replace("input", "output")));
            }

        }

        internal void LaunchTests()
        {
            WriteInformation("############################################################################################");
            WriteInformation(assembly.FullName);
            WriteInformation("############################################################################################");
            foreach (var file in testsFiles)
            {
                LaunchTest(file.input, file.output);
            }
        }

        internal void LaunchTest(string input, string output)
        {
            WriteInformation("");
            WriteInformation($"in:\t{input}");
            WriteInformation($"out:\t{output}");
            InterceptConsole();
            LaunchMain();
            LoadInput(input);
            mainTask.Wait();
            DisplayResult(output);

        }

        private void DisplayResult(string output)
        {
            WriteDebug(sbErr.ToString());
            var result = sbOut.ToString().Split('\n');

            var expectedResult = File.ReadAllText(output).Split('\n');

            for(var i = 0; i<expectedResult.Length; i++)
            {
                var lineExpected = expectedResult[i].Trim('\r');
                var lineResult = i < result.Length ? result[i].Trim('\r') : String.Empty;

                if(lineExpected == lineResult)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleOut.Write("[ OK ]");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    ConsoleOut.Write("[ KO ]");
                }
                Console.ForegroundColor = ConsoleColor.Gray;
                ConsoleOut.Write("\t[");
                Console.ForegroundColor = ConsoleColor.White;
                ConsoleOut.Write(lineExpected);
                Console.ForegroundColor = ConsoleColor.Gray;
                ConsoleOut.Write("]\t[");
                Console.ForegroundColor = ConsoleColor.White;
                ConsoleOut.Write(lineResult);
                Console.ForegroundColor = ConsoleColor.Gray;
                ConsoleOut.Write("]\n");
            }
        }

        internal void LaunchMain()
        {
            mainTask = Task.Factory.StartNew(() => mainMethod.Invoke(obj: null, parameters: new object[] { new String[0] }));
        }
        internal void InterceptConsole()
        {
            sbOut = new StringBuilder();
            sbErr = new StringBuilder();

            InterceptConsoleOut = new StringWriter(sbOut);
            InterceptConsoleErr = new StringWriter(sbErr);

            Console.SetOut(InterceptConsoleOut);
            Console.SetError(InterceptConsoleErr);
        }
        internal void LoadInput(string input)
        {
            try
            {
                InterceptConsoleIn?.Dispose();
            }
            catch
            {

            }
            InterceptConsoleIn = new System.IO.StreamReader(input);
            Console.SetIn(InterceptConsoleIn);

        }

        private void WriteInformation(string text)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            ConsoleOut.WriteLine(text);
        }
        private void WriteDebug(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            ConsoleOut.WriteLine(text);
        }
        public Loader()
        {
            SaveConsole();
        }

        private void SaveConsole()
        {
            ConsoleIn = Console.In;
            ConsoleErr = Console.Error;
            ConsoleOut = Console.Out;
        }

        public void Dispose()
        {
            Console.SetIn(ConsoleIn);
            Console.SetOut(ConsoleOut);
            Console.SetError(ConsoleErr);
            try
            {
                InterceptConsoleIn.Dispose();
            }
            catch
            {

            }
        }
    }
}
