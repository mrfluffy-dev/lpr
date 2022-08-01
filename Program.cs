
class lpr381
{
    static void Main()
    {
        Console.WriteLine("1: simplex algorithm");
        Console.WriteLine("2: two phase algorithm");
        Console.WriteLine("3: Dual Simplex Algorithm");
        Console.WriteLine("4: Branch and Bound Simplex Algorithm");
        Console.WriteLine("5: Cutting Plane Algorithmor Revised");
        Console.WriteLine("6: Chesssolver");
        Console.WriteLine("7: Create constraints");
        Console.WriteLine("8: exit");

        int menu = Convert.ToInt32(Console.ReadLine());


        switch (menu)
        {

            case 1:
                simplex simplex = new simplex();
                simplex.simplexAlgo();
                break;
            case 2:
                twoPhaseSimplex twoPhaseSimplex = new twoPhaseSimplex();
                twoPhaseSimplex.twoPhaseSimplexAlgo();
                break;
            case 3:
                //dual

                break;
            case 4:
                //Branch and Bound Simplex Algorithm
                break;
            case 5:
                //Cutting Plane Algorithmor Revised
                break;
            case 6:
                chesssolver chess = new chesssolver();
                chess.chesssolvealgo();
                break;
            case 7:
                Write write = new Write();
                write.writer();

                break;


            default:
                Console.WriteLine("invalid input");
                break;

        }

    }
}

