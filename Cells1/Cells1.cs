﻿using System;
using System.Globalization;
using System.Threading;

public class Cells1
{
	private int[] cells;
	private int N; //Розмір кристалу
	private int K; //Кількість атомів
	private double p; //Ймовірність переходу
	private Thread[] threads;
	private object lockObject = new object();
	private bool running = true; //Прапорець для зупинки потоків

	public Cells1(int n, int k, double probability)
	{
		N = n;
		K = k;
		p = probability;
		cells = new int[N];
		threads = new Thread[K];

		cells[0] = K;
	}

	public void Run()
	{
		Console.WriteLine("Cells1 simulation:");
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

			lock (lockObject)
			{
				if (m > p)
				{
					if (position < N - 1)
					{
						cells[position]--;
						position++;
						cells[position]++;
					}
				}
				else
				{
					if (position > 0)
					{
						cells[position]--;
						position--;
						cells[position]++;
					}
				}
			}

		}
	}

	public static void Main(string[] args)
	{
		int N = int.Parse(args[0]);
		int K = int.Parse(args[1]);
		double p = double.Parse(args[2], CultureInfo.InvariantCulture);

		Cells1 simulation = new Cells1(N, K, p);
		simulation.Run();
	}
}
