// See https://aka.ms/new-console-template for more information
using System.Diagnostics;

Console.WriteLine("Hello, World!");

int[] sizeOfNumbersRange = { 100000, 1000000, 10000000 };

Stopwatch stopWatch = new();

List<Func<List<int>, int>> sumDelegates = new()
{
    SumOrdinary,
    SumByThreads,
    SumByThreadsLinq
};


foreach (int range in sizeOfNumbersRange)
{
    List<int> numbers = new();

    for (int i = 0; i < range; i++)
    {
        numbers.Add(new Random().Next(0, 50));
    }

    foreach (var sumDeleg in sumDelegates)
    {
        stopWatch.Start();
        int result = sumDeleg(numbers);
        stopWatch.Stop();
        Console.WriteLine($"Sum based on {sumDeleg.Method.Name}. Sum: {result}. Time elapsed: {stopWatch.ElapsedMilliseconds} ms.");
        Console.WriteLine("");
        Console.WriteLine("");
    }
}



static int SumOrdinary(List<int> numbers)
{
    int result = 0;
    foreach (var n in numbers)
    {
        result += n;
    }
    return result;
}

static int SumByThreads(List<int> numbers)
{
    int result = 0;

    object lockObj = new();

    List<Thread> threads = new();
    int proccessorCount = Environment.ProcessorCount;
    int banchSize = (int)Math.Ceiling((double)numbers.Count / proccessorCount);

    for (int i = 0; i < proccessorCount; i++)
    {
        int start = i * banchSize;
        int end = Math.Min((i + 1) * banchSize, numbers.Count);

        Thread thread = new(() =>
        {
            int locSum = 0;

            for (int j = start; j < end; j++)
            {
                locSum += numbers[j];
            }
            lock (lockObj)
            {
                result += locSum;
            }
        });
        threads.Add(thread);
    }

    foreach (var t in threads)
    {
        t.Start();
    }

    foreach (var t in threads)
    {
        t.Join();
    }

    return result;
}

static int SumByThreadsLinq(List<int> numbers)
{
    int result = numbers.AsParallel().Sum();
    return result;
}