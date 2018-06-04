using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

namespace proxy
{
   class Program
   {
      static void Main(string[] args)
      {
         var argList = ParseArgs(args);
         var sourceFolders = GetSourceFolders(argList);

         FilterArgs(argList);

         foreach (var folder in sourceFolders)
         {
            Console.WriteLine(folder);

            ProcessFolder(argList, folder);
         }

         return;
      }

      static List<Argument> ParseArgs(string[] args)
      {
         var argList = new List<Argument>();

         foreach (var s in args)
         {
            if (s.First() == '-') {
               argList.Add(new Argument{name = s.ToLower(), value = ""});
            }
            else
            {
               var arg = argList.Last();
               arg.value = s;
               argList[argList.Count - 1] = arg;
            }
         }
         
         return argList;
      }

      static List<string> GetSourceFolders(List<Argument> arguments)
      {
         var folders = new List<string>();

         foreach (var arg in arguments)
         {
            if (arg.name == "-sourcedir") folders.Add(arg.value);
         }

         return folders;
      }
      
      static void FilterArgs(List<Argument> arguments)
      {
         arguments.RemoveAll(arg => arg.name == "-sourcedir" || arg.name == "-inputfile");
      }

      static void ProcessFolder(List<Argument> arguments, string folder)
      {
         Parallel.ForEach(Directory.EnumerateFiles(folder, "*.msh", SearchOption.AllDirectories),
            entry => ProcessFile(arguments, entry as string));
      }

      static void ProcessFile(List<Argument> arguments, string file)
      {
         Process process = new Process();
         
         process.StartInfo.FileName = "pc_modelMunge";
         process.StartInfo.Arguments = BuildArgString(arguments, file);
         process.StartInfo.RedirectStandardOutput = true;
         process.StartInfo.RedirectStandardError = true;
         process.StartInfo.UseShellExecute = false;

         var stdOut = new StringBuilder();
         process.OutputDataReceived += (sender, args) => stdOut.AppendLine(args.Data);
         
         var stdErr = new StringBuilder();
         process.ErrorDataReceived += (sender, args) => stdErr.AppendLine(args.Data);

         process.Start();

         process.BeginOutputReadLine();
         process.BeginErrorReadLine();

         process.WaitForExit();

         Console.Write(stdOut.ToString());
         Console.Error.Write(stdErr.ToString());
      }

      static string BuildArgString(List<Argument> arguments, string file)
      {
         var argString = new StringBuilder();

         argString.AppendFormat("-inputfile \"{0}\" ", file);

         foreach (var arg in arguments)
         {
            if (arg.value.Length != 0)
               argString.AppendFormat("{0} \"{1}\" ", arg.name, arg.value);
            else
               argString.AppendFormat("{0} ", arg.name);
         }

         return argString.ToString();
      }
   }

   struct Argument
   {
      public string name;
      public string value;
   }
}
