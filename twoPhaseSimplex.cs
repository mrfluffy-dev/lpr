using ConsoleTables;
class twoPhaseSimplex
{
    public void twoPhaseSimplexAlgo()
    {
        file file = new file();
        Tuple<List<zConstraints>, List<Constraints>> content = file.readFile();
        List<zConstraints> zConstraintsList = content.Item1;
        List<Constraints> constraintsList = content.Item2;

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
        int varCount = newTable[0].Count;
        table = prepareTable(newTable, constraintsList);

        // print table
        printTablePhaseOne(table, varCount);

        flipZ(table);
        printTablePhaseOne(table, varCount);
        bool isOptimal = isOptimalPhaseOne(table);
        while (!isOptimal)
        {
            int pivotCol = findPivotCol(table);
            List<float> ratio = ratioTest(table, pivotCol);
            int pivotRow = findPivotRow(ratio);
            table = pivotTable(table, pivotRow, pivotCol);
            printTablePhaseOne(table, varCount);
            isOptimal = isOptimalPhaseOne(table);
        }

        table = dropCol(table, varCount);
        isOptimal = isOptimalPhaseTwo(table);
        printTablePhaseTwo(table, varCount);
        while (!isOptimal)
        {
            int pivotCol = findPivotCol(table);
            List<float> ratio = ratioTest(table, pivotCol);
            int pivotRow = findPivotRow(ratio);
            table = pivotTable(table, pivotRow, pivotCol);
            printTablePhaseTwo(table, varCount);
            isOptimal = isOptimalPhaseTwo(table);
        }
        



    }

    void printTablePhaseOne(List<List<float>> table, int varCount)
    {
        int count = 1;
        string[] headers = new string[table[0].Count];
        for (int i = 0; i < table[0].Count; i++)
        {
            if (i < varCount)
            {
                headers[i] = "x" + (i + 1);
            }
            else if (i < table[0].Count - 1)
            {
                headers[i] = "e" + (count);
                headers[i+1] = "a" + (count);
                headers[i+2] = "s" + (count);
                i = i + 2;
                count++;
            }
            else
            {
                headers[i] = "rhs";
            }
        }
        var conTable = new ConsoleTable(headers);
        foreach (List<float> row in table)
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

    void printTablePhaseTwo(List<List<float>> table, int varCount)
    {
        int count = 1;
        string[] headers = new string[table[0].Count];
        for (int i = 0; i < table[0].Count; i++)
        {
            if (i < varCount)
            {
                headers[i] = "x" + (i + 1);
            }
            else if (i < table[0].Count - 1)
            {
                headers[i] = "e" + (count);
                headers[i+1] = "s" + (count);
                i = i + 1;
                count++;
            }
            else
            {
                headers[i] = "rhs";
            }
        }
        var conTable = new ConsoleTable(headers);
        foreach (List<float> row in table)
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

    List<List<float>> prepareTable(List<List<float>> table, List<Constraints> constraints)
    {
        // save all values in table in to newTable
        List<List<float>> newTable = new List<List<float>>();
        foreach (List<float> row in table)
        {
            List<float> newRow = new List<float>();
            foreach (float value in row)
            {
                newRow.Add(value);
            }
            newTable.Add(newRow);
        }
        List<int> rowsWithW = new List<int>();
        List<string> signs = new List<string>();
        foreach (Constraints constraint in constraints)
        {
            signs.Add(constraint.sign);    
        }
        // ammount of values in newTable startring from newTable[1])
        int varCount = 0;
        for (int i = 1; i < newTable.Count; i++)
        {
            varCount = varCount + newTable[i].Count;
        }
        for (int i = 0; i < signs.Count ; i++)
        {
            table[0].Add(0);
            table[0].Add(0);
            table[0].Add(0);
            int temp = (newTable[i+1].Count-1) * signs.Count;

            
            // add 3 0s to each row for every column in table
            for (int j = 0; j < signs.Count ; j++)
            {
                if (signs[i] == ">=")
                {
                    // make s = 1 
                    if (i == j)
                    {
                        table[i+1].Add(0);
                        table[i+1].Add(0);
                        table[i+1].Add(1);
                    }
                    else
                    {
                        table[i+1].Add(0);
                        table[i+1].Add(0);
                        table[i+1].Add(0);
                    }
                }
                else if (signs[i] == "<=")
                {
                    //make e and a = 1
                    if (i == j)
                    {
                        table[i+1].Add(-1);
                        table[i+1].Add(1);
                        table[i+1].Add(0);
                        rowsWithW.Add(i+1);
                    }
                    else
                    {
                        table[i+1].Add(0);
                        table[i+1].Add(0);
                        table[i+1].Add(0);
                    }
                }
                else if (signs[i] == "=")
                {
                    // make a = 1
                    if (i == j)
                    {
                        table[i+1].Add(0);
                        table[i+1].Add(1);
                        table[i+1].Add(0);
                        rowsWithW.Add(i+1);
                    }
                    else
                    {
                        table[i+1].Add(0);
                        table[i+1].Add(0);
                        table[i+1].Add(0);
                    }
                }
            }
        }
        table[0].Add(0);
        // create list of floats called wRow size table[0].Count - 1
        List<float> wRow = new List<float>();
        for (int i = 0; i < table[0].Count; i++)
        {
            wRow.Add(0);
        }
        for (int i = 0; i < table[0].Count; i++)
        {
            // if i is in rowsWithW
            if (rowsWithW.Contains(i))
            {
                for (int j = 0; j < wRow.Count; j++)
                {
                    wRow[j] = wRow[j] + (-table[i][j]);
                }
            }
        }
        //add wRow to the start of table
        table.Insert(1, wRow);
        
        // remove intex newTable.Count-1 from table and place it in the back of table
        for (int i = 2; i < table.Count; i++)
        {
            table[i].RemoveAt(newTable.Count-1);
            table[i].Add(newTable[i-1][newTable[i-1].Count-1]);

        }
        // remove the value of newTable.Count-1 from table[1] and place it in the back of table[1]
        float lastValue = table[1][newTable.Count-1];
        table[1].RemoveAt(newTable.Count-1);
        table[1].Add(lastValue);
        
        
        
        return table;
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

    //check if the table is optimal first phase
    // table is optimal if all values in table[1] are positive


    static bool isOptimalPhaseOne(List<List<float>> table)
    {
        int wIndex = 0;
        for (int i = 0; i < table[1].Count; i++)
        {
            if (table[1][i] < 0)
            {
                wIndex = i;
            }
        }
        if (wIndex == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    static bool isOptimalPhaseTwo(List<List<float>> table)
    {
        int wIndex = 0;
        for (int i = 0; i < table[0].Count; i++)
        {
            if (table[0][i] < 0)
            {
                wIndex = i;
            }
        }
        if (wIndex == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    //drop all a coloumn from the table and drop w row from the table
    static List<List<float>> dropCol(List<List<float>> table, int varCount)
    {
        table.RemoveAt(1);
        

        // for each row in table drop the varCount-1th element
        for (int i = 0; i < table.Count; i++)
        {
            for (int j = varCount + 1; j < table[i].Count - 1; j+=3)
            {
                table[i].RemoveAt(j);
                j = j - 1;
            }
        }
        return table;
    }
}