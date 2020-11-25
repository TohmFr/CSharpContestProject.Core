using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*******
 * Read input from Console
 * Use: Console.WriteLine to output your result to STDOUT.
 * Use: Console.Error.WriteLine to output debugging information to STDERR;
 *       
 * ***/

namespace CSharpContestProject
{
	class Program
	{
		static void Main(string[] args)
		{
			string line;


            var nbLine = line = Console.ReadLine();
            while ((line = Console.ReadLine()) != null)
            {
                //
                // Lisez les données et effectuez votre traitement */
                //
                if (EstBisextile(int.Parse(line)))
                {
                    Console.WriteLine("BISSEXTILE");
                }
                else
                {
                    Console.WriteLine("NON BISSEXTILE");
                }

            }

            // Vous pouvez aussi effectuer votre traitement ici après avoir lu toutes les données 
        }

        static bool EstBisextile (int annee)
        {
            return (annee % 4 == 0 && annee % 100 !=0) 
                    || annee % 400 == 0;
        }
    }
}