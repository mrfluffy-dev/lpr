using ConsoleTables;
class lpr381
{
    static void Main()
    {
        List<zConstraints> zConstraintsList = new List<zConstraints>();
        List<Constraints> constraintsList = new List<Constraints>();
        //read file "input.txt" line by line 
        string line;
        StreamReader file = new StreamReader("input.txt");
        while ((line = file.ReadLine()) != null)
        {
            // if line contains "min" or "max"
            if (line.Contains("min") || line.Contains("max"))
            {
                zConstraints constraints = new zConstraints();
                if (line.Contains("min"))
                {
                    constraints.minMax = "min";
                }
                else
                {
                    constraints.minMax = "max";
                }
                // remove "min" or "max" from line
                line = line.Remove(0, line.IndexOf(constraints.minMax) + constraints.minMax.Length);
                // split line by " "
                string[] lineArray = line.Split(' ');
                //remove empty elements from array
                lineArray = lineArray.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                // add variables to constraints.variables after parsing them to float
                List<float> variables = new List<float>();
                foreach (string variable in lineArray)
                {
                    variables.Add(float.Parse(variable));
                    
                }
                constraints.values = variables;
                // add constraints to list
                zConstraintsList.Add(constraints);
            }
            else
            {
                // split line by " "
                string[] lineArray = line.Split(' ');
                //remove empty elements from array
                lineArray = lineArray.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                // for each element in array
                Constraints constraints = new Constraints();
                List<float> variables = new List<float>();
                foreach (string variable in lineArray)
                {
                    if (variable.Contains("=") || variable.Contains(">") || variable.Contains("<"))
                    {
                        constraints.sign = variable;
                    }
                    else
                    {
                        variables.Add(float.Parse(variable));
                    }
                }
                constraints.values = variables;
                constraintsList.Add(constraints);

            }
        }
        file.Close();
        // print constraintsList
        foreach (zConstraints constraints in zConstraintsList)
        {
            Console.WriteLine(constraints.minMax + " " + string.Join(" ", constraints.values));
        }
        foreach (Constraints constraints in constraintsList)
        {
            Console.WriteLine(constraints.sign + " " + string.Join(" ", constraints.values));
        }
        List<List<float>> table = new List<List<float>>();
        table.Add(zConstraintsList[0].values);
        // add all constraints in constraintsList to table
        foreach (Constraints constraints in constraintsList)
        {
            table.Add(constraints.values);
        }
        List<List<float>> newTable = new List<List<float>>();
        // add all rows in table to newTable
        foreach (List<float> row in table)
        {
            // add all columns in row to newTable without reference to table
            List<float> newRow = new List<float>();
            for (int i = 0; i < row.Count; i++)
            {
                newRow.Add(row[i]);
                
            }
            newTable.Add(newRow);
        }
        
        // add s variable to table
        for (int i = 0; i < table.Count; i++)
        {
            if (i == 0)
            {
                for (int j = 0; j < table[i].Count + 1; j++)
                {
                    newTable[i].Add(0);
                }
            }
            else
            {
                for (int j = 0; j < table[i].Count-1; j++)
                {
                    //save the last value in newTable in to lastVal and remove it from newTanle
                    float lastVal = newTable[i][newTable[i].Count - 1];
                    newTable[i].RemoveAt(newTable[i].Count - 1);
                    if (i == j+1)
                    {
                        newTable[i].Add(1);
                    }
                    else
                    {
                        newTable[i].Add(0);
                    }
                    newTable[i].Add(lastVal);
                }
            }
        }
        // print table
        printTable(newTable);
        bool optimal = false;
        newTable = flipZ(newTable);
        while (!optimal)
        {
            int pivotCol = findPivotCol(newTable);
            List<float> ratio = ratioTest(newTable, pivotCol);
            int pivotRow = findPivotRow(ratio);
            printTable(newTable);
            newTable = pivotTable(newTable, pivotRow, pivotCol);
            Console.WriteLine("");
            if (checkOptimal(newTable))
            {
                printTable(newTable);
                optimal = true;
            }
        }

    }


    static void printTable(List<List<float>> newTable)
    {
        string[] headers = new string[newTable[0].Count];
        double half = newTable[0].Count / 2;
        int halfInt = (int)Math.Floor(half);
        for (int i = 0; i < newTable[0].Count; i++)
        {
            if (i != newTable[0].Count - 1 && i < halfInt)
            {
                headers[i] = "x" + (i + 1);
            }
            else if (i != newTable[0].Count - 1 && i >= halfInt)
            {
                // add "s" to headers starting at 1
                headers[i] = "s" + ((i - newTable[0].Count  / 2)+1);
                //headers[i] = "s" + (i + 1);
            }
            else
            {
                headers[i] = "rhs";
            }            
        }
        var conTable = new ConsoleTable(headers);
        // for each row in table add row to conTable
        foreach (List<float> row in newTable)
        {
            // convert row to object array
            object[] rowArray = new object[row.Count];
            for (int i = 0; i < row.Count; i++)
            {
                rowArray[i] = row[i];
            }
            conTable.AddRow(rowArray);
        }
        conTable.Write(Format.Alternative);
    }



    static bool checkOptimal(List<List<float>> table)
    {
        int zIndex = 0;
        for (int i = 0; i < table[0].Count; i++)
        {
            if (table[0][i] < 0)
            {
                zIndex = i;
            }
        }
        if (zIndex == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    static List<List<float>> flipZ(List<List<float>> table)
    {
        for (int i = 0; i < table[0].Count; i++)
        {
            if (table[0][i] > 0)
            {
                table[0][i] = -table[0][i];
            }
        }
        return table;
    }

    static int findPivotCol(List<List<float>> table)
    {
        float largest = 0;
        int largestIndex = 0;
        for (int i = 0; i < table[0].Count; i++)
        {
            if (table[0][i] < largest)
            {
                largest = table[0][i];
                largestIndex = i;
            }
        }
        return largestIndex;
    }

    static List<float> ratioTest(List<List<float>> table, int testCol)
    {
        List<float> ratios = new List<float>();
        for (int i = 1; i < table.Count; i++)
        {
            if (table[i][testCol] != 0)
            {
                ratios.Add(table[i][table[i].Count - 1] / table[i][testCol]);
            }
            else
            {
                ratios.Add(float.MaxValue);
            }
        }
        return ratios;
    }


    static int findPivotRow(List<float> ratios)
    {
        float smallest = float.MaxValue;
        int smallestIndex = 0;
        for (int i = 0; i < ratios.Count; i++)
        {
            if (ratios[i] < smallest && ratios[i] > 0.0)
            {
                smallest = ratios[i];
                smallestIndex = i;
            }
        }
        return smallestIndex;
    }


    static List<List<float>> pivotTable(List<List<float>> table, int pivotRow, int pivotCol)
    {
        //clone table in to newTable
        List<List<float>> newTable = new List<List<float>>();
        foreach (List<float> row in table)
        {
            List<float> newRow = new List<float>();
            for (int i = 0; i < row.Count; i++)
            {
                newRow.Add(row[i]);
            }
            newTable.Add(newRow);
        }
        var pivotPoint = newTable[pivotRow + 1][pivotCol];
        // divide pivot row by pivot point
        for (int i = 0; i < newTable[pivotRow + 1].Count; i++)
        {
            newTable[pivotRow + 1][i] = newTable[pivotRow + 1][i] / pivotPoint;
        }
        // current possition-(pivot_row*new_table pivot_point)
        for (int i = 0; i < newTable.Count; i++)
        {
            if (i != pivotRow + 1)
            {
                var currentPossition = newTable[i][pivotCol];
                for (int j = 0; j < newTable[i].Count; j++)
                {
                    newTable[i][j] = newTable[i][j] - (currentPossition * newTable[pivotRow + 1][j]);
                }
            }
        }
        return newTable;

    }



}

class zConstraints
{
    public string minMax;
    public List<float> values; 
}   

class Constraints
{
    public List<float> values;

    public string sign;
}