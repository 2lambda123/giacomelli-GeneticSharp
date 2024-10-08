﻿using System.Drawing;
using NUnit.Framework;

namespace GeneticSharp.Extensions.UnitTests.Drawing
{
    [TestFixture]
    [Category("Extensions")]
    class BitmapTest
    {
        [SetUp]
        public void InitializeTest()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }

        [Test()]
        public void Evolve_ManyGenerations_Fast()
        {
            var selection = new EliteSelection();
            var crossover = new UniformCrossover();
            var mutation = new TworsMutation();
            var chromosome = new BitmapChromosome(32, 32);
            var targetBitmap = new Bitmap(32, 32);
            var fitness = new BitmapEqualityFitness(targetBitmap);

            var population = new Population(10, 10, chromosome);

            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);

            ga.TaskExecutor = new ParallelTaskExecutor()
            {
                MinThreads = 10,
                MaxThreads = 20
            };

            ga.Termination = new GenerationNumberTermination(5);
            ga.Start();

            var c = ga.BestChromosome as BitmapChromosome;
            Assert.IsNotNull(c);

            var bitmap = c.BuildBitmap();
            Assert.IsNotNull(bitmap);
            Assert.AreEqual(32, bitmap.Width);
            Assert.AreEqual(32, bitmap.Height);
        }
    }
}
