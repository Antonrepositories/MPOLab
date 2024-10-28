using System;
using System.Globalization;
using System.Threading;

public class Cells2
{
	private int[] cells;
	private int N; //Розмір кристалу
	private int K; //Кількість атомів
	private double p; //Ймовірність переходу
	private Thread[] threads;
	private object[] lockObjects; //Масив для блокування клітинок
	private bool running = true; //Прапорець для зупинки потоків

	public Cells2(int n, int k, double probability)
	{
		N = n;
		K = k;
		p = probability;
		cells = new int[N];
		threads = new Thread[K];
		lockObjects = new object[N]; //Ініціалізація масиву блокувань

		for (int i = 0; i < N; i++)
		{
			lockObjects[i] = new object(); //Ініціалізація блокувань для кожної клітинки
		}

		cells[0] = K;
	}

	public void Run()
	{
		Console.WriteLine("Cells2 simulation:");
		for (int i = 0; i < K; i++)
		{
			int atomIndex = i;
			threads[i] = new Thread(() => Simulate());
			threads[i].Start();
		}

		for (int t = 0; t < 60; t++)
		{

			Thread.Sleep(1000);

			Console.WriteLine($"Second {t + 1}: " + $"[{string.Join(", ", cells)}]");
		}

		running = false;

		foreach (var thread in threads)
		{
			thread.Join();
		}

		int totalAtoms = 0;
		foreach (var count in cells)
		{
			totalAtoms += count;
		}
		Console.WriteLine($"Total atoms before simulation: {K}");
		Console.WriteLine($"Total atoms after simulation: {totalAtoms}");
	}

	private void Simulate()
	{
		int position = 0;

		while (running)
		{
			double m = new Random().NextDouble();

			if (m > p)
			{
				if (position < N - 1)
				{
					lock (lockObjects[position]) //Блокування для поточної клітинки
					{
						cells[position]--;
					}

					lock (lockObjects[position + 1]) //Блокування для наступної клітинки
					{
						cells[position + 1]++;
					}

					position++;
				}
			}
			else
			{
				if (position > 0)
				{
					lock (lockObjects[position]) //Блокування для поточної клітинки
					{
						cells[position]--;
					}

					lock (lockObjects[position - 1]) //Блокування для попередньої клітинки
					{
						cells[position - 1]++;
					}

					position--;
				}
			}
		}
	}

	public static void Main(string[] args)
	{
		int N = int.Parse(args[0]);
		int K = int.Parse(args[1]);
		double p = double.Parse(args[2], CultureInfo.InvariantCulture);

		Cells2 simulation = new Cells2(N, K, p);
		simulation.Run();
	}
}
