
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

Calculator("-(-((--((3,2*--5)**(2+--4)/(--((-1,5)**(3-1)+--(-2,7*--1,2))))+((--5,1**-(2+3)-(-(-6,3)**2))/((3**(2**3))-(-4,5*--1,7)**(3-2))))+(((--7,4*--2)**-(--3+2)/(((-2**2+--3,3)*(-1,5))**2))-(--(2,5+--1,2)**(3-(-2))+((--4,1*--3)**-(1+2)/((-3,5)**(2+1)+--(-1,2*--2)))))))");//24
//(--((3-7)**2+(-2,5*4)**(1+1))/(2**3**2)+-(-3,5*--2)**2)
//+(((--6,1*2)**-(1+3)-(-(-4)**2))/((2**(3**2))-(-5,5*--2)**(2-3))))
/// ((-2,5)**(1+2)+--(-3,2*--5))
// -(-((4+--7*2)**(3-1)))
static void Calculator(string input)
{
    input = RemoveBreckets(input);
    //array ordered by priority operations
    string[] operations = { "(", ")", "", "", "*", "/", "+", "-" };

    List < (int parent,
          int operation,
          int index,
          int children1,
          int children2,
          string expression,
          double? volume)> SolvingTree = new List<(int parent, int operation, int index, int children1, int children2, string expression, double? volume)> { };

    Console.WriteLine(CCreateSolvingTree(input, operations, SolvingTree));
    //Console.WriteLine(CFindIndexes(operations, input));

}

static /*decimal*/double CCreateSolvingTree(string input, string[] operations, List<(int parent, int operation, int index, int children1, int children2, string expression, double? volume)> SolvingTree)
{
    decimal result = 0;
    int numberOfIndexes = CFindIndexes(operations, input).numberOfIndexes;
    //var list = CFindIndexes(operations, input);

    while (SolvingTree.Where(op => op.operation != -1 && op.children1 != -1).Count() < numberOfIndexes) //|| SolvingTree.Count == 0
    {
        int weekestOperationIndex = CGetTheWeekestOperation(CFindIndexes(operations, SolvingTree.Count == 0 ? input : (SolvingTree.Find(i => i.index == SolvingTree.Where(p => p.operation != -1 && p.children1 == -1).Max(j => j.index)).expression)).indexAndPriority).index;
        int weekestOperationPriority = CGetTheWeekestOperation(CFindIndexes(operations, SolvingTree.Count == 0 ? input : (SolvingTree.Find(i => i.index == (SolvingTree.Where(p => p.operation != -1 && p.children1 == -1).Max(j => j.index))).expression)).indexAndPriority).priority;
        if (SolvingTree.Count == 0)
        {
            SolvingTree.Add((
                -1,
                weekestOperationPriority,
                0,
                -1,
                -1,
                RemoveBreckets(input),
                null));
        }
            if (weekestOperationPriority != 3)
            {
            int parentIndex = SolvingTree
            .Where(i => i.operation != -1 && i.children1 == -1).Max(i => i.index);
            //.Select(i => i.index)
            //.DefaultIfEmpty(0)

            string parentExpression = SolvingTree
                .First(i => i.index == parentIndex)
                .expression;
            string leftExpression = parentExpression[0..(weekestOperationIndex - (SolvingTree.Find(p => p.index == parentIndex).operation == 2 ? 1 : 0))];
            string rightExpression = parentExpression[(weekestOperationIndex + 1)..];

            SolvingTree.Add((
            parentIndex,
            CGetTheWeekestOperation(CFindIndexes(operations, leftExpression).indexAndPriority).priority,
            SolvingTree.Max(i => i.index) + 1,
            -1,
            -1,
            RemoveBreckets(leftExpression),
            null));
            SolvingTree[parentIndex] = SolvingTree[parentIndex] with { children1 = SolvingTree.Max(i => i.index) };

            SolvingTree.Add((
                parentIndex,
                CGetTheWeekestOperation(CFindIndexes(operations, rightExpression).indexAndPriority).priority,
                SolvingTree.Max(i => i.index) + 1,
                -1,
                -1,
                RemoveBreckets(rightExpression),
                null));
            SolvingTree[parentIndex] = SolvingTree[parentIndex] with { children2 = SolvingTree.Max(i => i.index) };
            
                
            }
            else
            {
                int parentIndex = SolvingTree
                    .Where(i => i.operation != -1 && i.children1 == -1).Max(i => i.index);

                string parentExpression = SolvingTree   
                    .First(i => i.index == parentIndex)
                    .expression;

                string rightExpression = parentExpression[(weekestOperationIndex + 1)..];

                SolvingTree.Add((
                    parentIndex,
                    CGetTheWeekestOperation(CFindIndexes(operations, rightExpression).indexAndPriority).priority,
                    SolvingTree.Max(i => i.index) + 1,
                    -1,
                    -1,
                    RemoveBreckets(rightExpression),
                    null));
                SolvingTree[parentIndex] = SolvingTree[parentIndex] with { children1 = SolvingTree.Max(i => i.index) };
            }

    }


    while (SolvingTree[0].volume is null)
    {
        int indexToSolve = SolvingTree.Where(i => i.volume is null).Max(i => i.index);
        if (SolvingTree[indexToSolve].children1 == -1)
        {
            SolvingTree[indexToSolve] = SolvingTree[indexToSolve] with { volume = Convert./*ToDecimal*/ToDouble((SolvingTree[indexToSolve].expression)) };
            continue;
        }
        else
        {



            if (SolvingTree[indexToSolve].operation != 3)
            {
                /*decimal*/double value1 = SolvingTree[SolvingTree[indexToSolve].children1].volume ?? 0;
                /*decimal*/double value2 = SolvingTree[SolvingTree[indexToSolve].children2].volume ?? 0;

                switch (SolvingTree[indexToSolve].operation)
                {
                    case 2:
                        SolvingTree[indexToSolve] = SolvingTree[indexToSolve] with { volume = /*(decimal)*/Math.Pow((double)value1, (double)value2) };
                        break;
                    case 4:
                        SolvingTree[indexToSolve] = SolvingTree[indexToSolve] with { volume = value1 * value2 };
                        break;
                    case 5:
                        SolvingTree[indexToSolve] = SolvingTree[indexToSolve] with { volume = value1 / value2 };
                        break;
                    case 6:
                        SolvingTree[indexToSolve] = SolvingTree[indexToSolve] with { volume = value1 + value2 };
                        break;
                    case 7:
                        SolvingTree[indexToSolve] = SolvingTree[indexToSolve] with { volume = value1 - value2 };
                        break;

                }
            }
            else
            {
                /*decimal*/double value1 = SolvingTree[SolvingTree[indexToSolve].children1].volume ?? 0;
                SolvingTree[indexToSolve] = SolvingTree[indexToSolve] with { volume = -value1 };
            }

        }
    }
    foreach (var item in SolvingTree)
    {
        Console.WriteLine($"parent: {item.parent}, operation: {item.operation}, index: {item.index}, children1: {item.children1}, children2: {item.children2}, expression: {item.expression}, volume: {item.volume}");
    }

    return SolvingTree[0].volume ?? 000;
}

//here we are getting index of operations and operations type
//format (index),(type)
static (List<(int index, int priority)> indexAndPriority, int numberOfIndexes) CFindIndexes(string[] operations, string input)
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
        //operationsIndex.Sort((a, b) => a.priority.CompareTo(b.priority));
        //int operationIndex = operationsIndex[0].index;
        //int operationPriority = operationsIndex[0].priority;
        int numberOfIndexes = operationsIndex.Where(i => i.priority != 0 && i.priority != 1).Count();

        return (operationsIndex, numberOfIndexes);
    }
    else
    {
        operationsIndex.Add((-1, -1));

        return (operationsIndex, 0);
    }

}

static string RemoveBreckets(string input)
{
    if (string.IsNullOrEmpty(input))
        return input;

    if (input[0] == '(' && input[input.Length - 1] == ')')
    {
        int countOpen = 0;
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == '(') countOpen++;
            if (input[i] == ')') countOpen--;

            // Якщо всі дужки збалансовані на кінці рядка
            if (countOpen == 0)
            {
                if (i == input.Length - 1)
                {
                    return input.Substring(1, input.Length - 2);
                }
                else
                {
                    return input;
                }
            }
        }
    }

    return input;
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

    if (operationsIndexNew[0].priority == -1 || operationsIndexNew.Count() == 1 || operationsIndexNew[0].index == operationsIndexNew.Min(i => i.index) )
    {
        return (operationsIndexNew[0].index, operationsIndexNew[0].priority);
    }
    else
    {
        int nextIndex = operationsIndexNew.Where(t => t.index < operationsIndexNew[0].index)
        .Max(t => t.index);
        int nextPriority = operationsIndexNew.First(i => i.index == operationsIndexNew.Where(t => t.index < operationsIndexNew[0].index)
        .Max(t => t.index)).priority;
        if (nextPriority == 2 && operationsIndexNew[0].priority == 3)
        {
            return (operationsIndexNew.First(i => i.index == operationsIndexNew.Where(t => t.index < operationsIndexNew[0].index).Max(t => t.index)).index, operationsIndexNew.First(i => i.index == operationsIndexNew.Where(t => t.index < operationsIndexNew[0].index).Max(t => t.index)).priority);
        }
        else
        {
            return (operationsIndexNew[0].index, operationsIndexNew[0].priority);
        }
    }

    //return (operationsIndexNew[0].index, operationsIndexNew[0].priority);

}