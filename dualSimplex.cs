using ConsoleTables;

class dualSimplex
{
    public void dualSimplexAlgo()
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
        // print every row in table
        printTable(table, varCount);
        
    }


    List<List<float>> prepareTable(List<List<float>> table, List<Constraints> constraints)
    {
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

            // if sign[i] == "=" copy table[i] to table[i+1] without overwriting table[i+1]
            if (signs[i] == "=")
            {
                List<float> tempRow = new List<float>();
                foreach (float value in table[i+1])
                {
                    tempRow.Add(value);
                }
                table.Add(tempRow);
                // move last row of table to table[i+2]
                for (int j = table.Count - 1; j > i+2; j--)
                {
                    List<float> row = table[j];
                    table[j] = table[j-1];
                    table[j-1] = row;
                }
                signs.Add("2=");
                for (int j = signs.Count - 1; j > i + 1; j--)
                {
                    string sign = signs[j];
                    signs[j] = signs[j-1];
                    signs[j-1] = sign;
                }
                newTable.Insert(i+1, newTable[i+1]);
            }

            if (signs[i] == ">=")
            {
                // multiply table[i+1] by -1
                for (int j = 0; j < table[i+1].Count; j++)
                {
                    table[i+1][j] = table[i+1][j] * -1;
                }
            }

            // add 3 0s to each row for every column in table
            for (int j = 0; j < signs.Count ; j++)
            {
                if (signs[i] == ">=")
                {
                    // make s = 1 
                    if (i == j)
                    {
                        table[i+1].Add(0);
                        table[i+1].Add(1);
                    }
                    else
                    {
                        table[i+1].Add(0);
                        table[i+1].Add(0);
                    }
                }
                else if (signs[i] == "<=")
                {
                    //make e = 1
                    if (i == j)
                    {
                        table[i+1].Add(-1);
                        table[i+1].Add(0);
                    }
                    else
                    {
                        table[i+1].Add(0);
                        table[i+1].Add(0);
                    }
                }
                else if (signs[i] == "=")
                {
                    // make a = 1
                    if (i == j)
                    {
                        table[i+1].Add(-1);
                        table[i+1].Add(0);
                    }
                    else
                    {
                        table[i+1].Add(0);
                        table[i+1].Add(0);
                    }
                }
                else if (signs[i] == "2=")
                {
                    // make a = 1
                    if (i == j)
                    {
                        table[i+1].Add(0);
                        table[i+1].Add(1);
                    }
                    else
                    {
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
            table[i].RemoveAt(newTable[i].Count-1);
            if (signs[i-1] == ">=" || signs[i-1] == "2=")
            {
                table[i].Add(newTable[i][newTable[i].Count-1] * -1);
            }
            else
            {
                table[i].Add(newTable[i][newTable[i].Count-1]);
            }

        }
        // remove the value of newTable.Count-1 from table[1] and place it in the back of table[1]
        return table;
    }

    void printTable(List<List<float>> table, int varCount)
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
}