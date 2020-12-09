﻿using System;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.AutoConfig;
using GeneticSharp.Extensions.Tsp;
using GeneticSharp.Infrastructure.Framework.Threading;
using NUnit.Framework;

namespace GeneticSharp.Extensions.UnitTests.AutoConfig
{
    [TestFixture]
    [Category("Extensions")]
    class AutoConfigTest
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
            var mutation = new UniformMutation(true);
            var chromosome = new AutoConfigChromosome();
            chromosome.InitializeGenes();
            var targetChromosome = new TspChromosome(10);
            targetChromosome.InitializeGenes();
            var targetFitness = new TspFitness(10, 0, 100, 0, 100);
            var fitness = new AutoConfigFitness(targetFitness, targetChromosome)
            {
                PopulationMinSize = 20,
                PopulationMaxSize = 20,
                Termination = new TimeEvolvingTermination(TimeSpan.FromSeconds(5))
            };

            var population = new Population(10, 10, chromosome);

            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
            {
                TaskExecutor = new ParallelTaskExecutor() {MinThreads = 10, MaxThreads = 20},
                Termination = new GenerationNumberTermination(10)
            };



            ga.Start();

            Assert.NotNull(ga.BestChromosome);            
        }

        [Test()]
        public void GenerateGene_InvalidIndex_Exception()
        {
            var target = new AutoConfigChromosome();
            target.InitializeGenes();
            var actual = Assert.Catch(() => target.GenerateGene(9));
            Assert.AreEqual("Invalid AutoConfigChromosome gene index.", actual.Message);
        }
    }
}
