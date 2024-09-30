using System.Diagnostics;

class Program
{
    private static long sum = 0;

    private static readonly object lockObject = new object();

    static void Main()
    {
        int[] sizes = [100000, 1000000, 10000000];

        int numberOfThreads = 6; // установил количество потоков

        Thread[] threads = new Thread[numberOfThreads];

        Stopwatch stopWatch = new Stopwatch();

        for (int t = 0; t < sizes.Length; t++)
        {
            int[] numbers = new int[sizes[t]];

            Console.WriteLine($"Расчёт на размер массива на {sizes[t]} элементов");

            for (int j = 0; j < sizes[t]; j++)
            {
                numbers[j] = 1;
            }

            int range = numbers.Length / numberOfThreads;

            // Суммирование массива с использованием Thread

            for (int u = 0; u < numberOfThreads; u++)
            {
                int start = u * range;
                int end = (u == numberOfThreads - 1) ? numbers.Length : start + range;
                threads[u] = new Thread(() => SumArray(start, end, numbers));
                threads[u].Start();
            }
            stopWatch.Start();

            // Жду завершения всех потоков
            for (int y = 0; y < numberOfThreads; y++)
            {
                threads[y].Join();
            }

            stopWatch.Stop();

            Console.WriteLine($"сумма: {sum}; Затраченное время с thread {stopWatch.ElapsedMilliseconds} мс");
            sum = 0;

            stopWatch.Restart();

            //Обычное суммирование массива
            sum = 0;

            stopWatch.Start();

            //sum = numbers.Sum();

            for (int l = 0;l < numbers.Length; l++)
            {
                sum += numbers[l];
            }

            stopWatch.Stop();

            Console.WriteLine($"сумма: {sum}; Затраченное время с обычным суммированием {stopWatch.ElapsedMilliseconds} мс");
            sum = 0;

            stopWatch.Restart();

            //Cуммирование массива при помощи PLinq
            stopWatch.Start();

            sum = numbers.AsParallel().Sum();

            stopWatch.Stop();

            Console.WriteLine($"сумма: {sum}; Затраченное время с PLinq {stopWatch.ElapsedMilliseconds} мс");

            stopWatch.Restart();

            sum = 0;

            Console.WriteLine(new string('.', 100));
        }
    }

    public static void SumArray(int start, int end, int[] numbers)
    {
        long localSum = 0;

        for (int i = start; i < end; i++)
        {
            localSum += numbers[i];
        }

        lock (lockObject)
        {
            sum += localSum;
        }
    }
}

