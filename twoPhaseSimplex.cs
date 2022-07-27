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
        printTable(table, varCount);

    }

    void printTable(List<List<float>> table, int varCount)
    {
        string[] headers = new string[table[0].Count];
        for (int i = 0; i < table[0].Count; i++)
        {
            if (i < varCount)
            {
                headers[i] = "x" + (i + 1);
            }
            else if (i < table[0].Count - 1)
            {
                headers[i] = "e" + (i - varCount + 1);
                headers[i+1] = "a" + (i - varCount + 1);
                headers[i+2] = "s" + (i - varCount + 1);
                i = i + 2;
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
        List<string> signs = new List<string>();
        foreach (Constraints constraint in constraints)
        {
            signs.Add(constraint.sign);    
        }
        for (int i = 0; i < signs.Count ; i++)
        {
            table[0].Add(0);
            table[0].Add(0);
            table[0].Add(0);
            // add 3 0s to each row for every column in table
            for (int j = 0; j < newTable[i+1].Count-1; j++)
            {
                if (signs[i] == ">=")
                {
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
                    if (i == j)
                    {
                        table[i+1].Add(1);
                        table[i+1].Add(1);
                        table[i+1].Add(0);
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
                    if (i == j)
                    {
                        table[i+1].Add(0);
                        table[i+1].Add(1);
                        table[i+1].Add(0);
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
        // remove intex newTable.Count-1 from table and place it in the back of table
        for (int i = 1; i < table.Count; i++)
        {
            table[i].RemoveAt(newTable.Count-1);
            table[i].Add(newTable[i][newTable.Count-1]);

        }
        
        return table;
    }
}