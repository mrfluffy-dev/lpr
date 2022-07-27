class file
{
    public Tuple<List<zConstraints>,List<Constraints>> readFile()
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
        return new Tuple<List<zConstraints>, List<Constraints>>(zConstraintsList, constraintsList);
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