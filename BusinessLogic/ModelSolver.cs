using BusinessLogic.Algorithms;
using Common;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class ModelSolver
    {
        public static void Solve(Model model, Algorithm algorithm)
        {
            algorithm.PutModelInCanonicalForm(model);

            algorithm.Solve(model);

            Console.Clear();
            Console.WriteLine("Here is the solution:");
            Console.WriteLine("=========================================================================================================");
        }
    }
}
