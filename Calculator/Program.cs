
using System.Linq.Expressions;

Calculator("(3-7)**2**8**3");//24
static void Calculator(string input)
{
    //array ordered by priority operations
    string[] operations = { "(", ")", "", "", "*", "/", "+", "-" };

    List<(int? parent,
          int? operation,
          int? index,
          int? children1,
          int? children2,
          string? expression,
          int ? volume)> SolvingTree = new List<(int? parent, int? operation, int? index, int? children1, int? children2, string? expression, int? volume)> { };

    CCreateSolvingTree(input, operations, SolvingTree);
    //Console.WriteLine(CFindIndexes(operations, input));

}

static decimal CCreateSolvingTree(string input, string[] operations, List<(int? parent, int? operation, int? index, int? children1, int? children2, string? expression, int? volume)> SolvingTree)
{
    decimal result = 0;
    int numberOfIndexes = CFindIndexes(operations, input).numberOfIndexes;
    //var list = CFindIndexes(operations, input);

    while (SolvingTree.Count(op => op.operation is not null) < numberOfIndexes) //|| SolvingTree.Count == 0
    {
        int weekestOperationIndex = CFindIndexes(operations, SolvingTree.Find(i => i.index == (SolvingTree.Max(j => j.index))).expression).index;
        int weekestOperationPriority = CFindIndexes(operations, SolvingTree.Find(i => i.index == (SolvingTree.Max(j => j.index))).expression).priority;
        if (SolvingTree.Count == 0)
        {
            SolvingTree.Add((
                null, 
                weekestOperationPriority,
                0,
                null,
                null,
                input,
                null));            
            
            if (weekestOperationPriority != 3)
            {
                SolvingTree.Add((
                    0,
                    CFindIndexes(operations, SolvingTree.Find(p => p.index == 0).expression).priority,
                    SolvingTree.Max(i => i.index) + 1,
                    null,
                    null,
                    SolvingTree.Find(p => p.index == 0).expression[0..(CFindIndexes(operations, SolvingTree.Find(p => p.index == 0).expression).index - (SolvingTree.Find(p => p.index == 0).operation == 2 ? 1 : 0))],
                    null));
                SolvingTree[0] = SolvingTree[0] with { children1 = SolvingTree.Max(i => i.index) };

                SolvingTree.Add((
                    0,
                    CFindIndexes(operations, SolvingTree.Find(p => p.index == 0).expression).priority,
                    SolvingTree.Max(i => i.index) + 1,  
                    null,
                    null,
                    SolvingTree.Find(p => p.index == 0).expression[(CFindIndexes(operations, SolvingTree.Find(p => p.index == 0).expression).index + 1)..],
                    null));
                SolvingTree[0] = SolvingTree[0] with { children2 = SolvingTree.Max(i => i.index) };
            }
            else
            {
                SolvingTree.Add((
                    0,
                    CFindIndexes(operations, SolvingTree.Find(p => p.index == 0).expression).priority,
                    SolvingTree.Max(i => i.index) + 1,
                    null,
                    null,
                    SolvingTree.Find(p => p.index == 0).expression[(CFindIndexes(operations, SolvingTree.Find(p => p.index == 0).expression).index + 1)..],
                    null)); 
                SolvingTree[0] = SolvingTree[0] with { children2 = SolvingTree.Max(i => i.index) };
            }

                
        }
        else
        {
            SolvingTree.Add((0, null, SolvingTree.Max(i => i.index) + 1, null, null, null));
            SolvingTree[0] = SolvingTree[0] with { children1 = SolvingTree.Max(i => i.index) };
        }

    }

    return 0;
}

//here we are getting index of operations and operations type
//format (index),(type)
static (int index, int priority, int numberOfIndexes) CFindIndexes(string[] operations, string input)
{
    List<(int index, int priority)> operationsIndex = new List<(int index, int priority)> { };
    for (int i = 0; i < operations.Length; i++)
    {
        for (int j = 0; j < input.Length; j++)
        {
            if (operations[i] == (input[j]).ToString())
            {
                if (j > 0)
                {
                    if (Array.Exists(operations, element => element == (input[j - 1]).ToString()) && input[j - 1].ToString() != ")" && input[j].ToString() != "(")
                    {

                        if (input[j].ToString() == "-")
                        {
                            operationsIndex.Add((j, 3));
                            continue;
                        }
                        else if (input[j].ToString() == "*" && input[j - 1].ToString() == "*")
                        {
                            //to remove the first * and add the second one with higher priority
                            operationsIndex.RemoveAll(op => op.index == j - 1);
                            operationsIndex.Add((j, 2));
                            continue;
                        }
                    }

                    else
                    {
                        operationsIndex.Add((j, i));
                        continue;
                    }
                }

                else
                {
                    if (input[j].ToString() == "-")
                    {
                        operationsIndex.Add((j, 3));
                        continue;
                    }
                    else
                    {
                        operationsIndex.Add((j, i));
                        continue;
                    }


                }

            }
        }
    }

    if (operationsIndex.Count > 0)
    {
        int operationIndex = operationsIndex[0].index;
        int operationPriority = operationsIndex[0].priority;
        int numberOfIndexes = operationsIndex.Count;
        return (operationIndex, operationPriority, numberOfIndexes);
    }
    else
    {
        operationsIndex.Add((-1, -1));

        return (operationsIndex[0].index, operationsIndex[0].priority, 0);
    }




}

static (int index, int priority) CGetTheWeekestOperation(List<(int index, int priority)> operationsIndex)
{

    List<(int index, int priority)> operationsIndexNew = operationsIndex.ToList();

    while (operationsIndexNew.Any(pr => pr.priority == 0) || operationsIndexNew.Any(pr => pr.priority == 1))
    {
        List<(int index, int priority)> operationsIndexOrderedByIndex = operationsIndexNew.OrderBy(op => op.index).ToList();

        foreach (var item in operationsIndexNew)
        {
            if (item.priority > 1)
            {
                operationsIndexOrderedByIndex.Remove(item);
            }
        }

        int count0 = 0;
        int count1 = 0;

        for (int i = 0; operationsIndexOrderedByIndex.Count > i && operationsIndexOrderedByIndex[i].priority != 1; i++)
        {
            for (int j = 0; j < operationsIndexOrderedByIndex.Count; j++)
            {

                if (operationsIndexOrderedByIndex[j].priority == 0)
                {
                    count0++;
                }
                if (operationsIndexOrderedByIndex[j].priority == 1)
                {
                    count1++;
                }
                if (count0 == count1)
                {
                    count0 = 0;
                    count1 = 0;
                    if (operationsIndexOrderedByIndex[i].index == 0 && operationsIndexOrderedByIndex[j].index == operationsIndexNew.Max().index)
                    {
                        operationsIndexNew.RemoveAll(rem => rem.index == operationsIndexOrderedByIndex[i].index || rem.index == operationsIndexOrderedByIndex[j].index);
                        break;
                    }
                    else
                    {
                        operationsIndexNew.RemoveAll(rem => rem.index >= operationsIndexOrderedByIndex[i].index && rem.index <= operationsIndexOrderedByIndex[j].index);
                        break;
                    }

                }
            }
            break;
        }
    }

    operationsIndexNew = operationsIndexNew.OrderByDescending(op => op.priority).ToList();
    //List<(int index, int priority)> operationsIndexNewToReturn = operationsIndexNew.ToList();
    //operationsIndexNewToReturn.RemoveAll(op => op.index != operationsIndexNew[0].index);
    return (operationsIndexNew[0].index, operationsIndexNew[0].priority);

}