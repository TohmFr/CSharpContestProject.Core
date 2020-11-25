using System;
using System.Linq;
using System.Reflection;
using CSharpContestProject;
namespace CSharpContestProject.Core
{
    class Program
    {
        Assembly assembly;

        static void Main(string[] args)
        {
            using (var loader = new Loader())
            {
                loader.Run();
            }

        }
    }       
}
