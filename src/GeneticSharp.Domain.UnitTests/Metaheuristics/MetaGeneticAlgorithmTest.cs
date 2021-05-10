﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Metaheuristics.Matching;
using GeneticSharp.Domain.Metaheuristics.Primitives;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Results;
using GeneticSharp.Domain.Terminations;
using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.MetaHeuristics
{
    [TestFixture]
    [Category("MetaHeuristics")]
    class MetaGeneticAlgorithmTest: MetaHeuristicTestBase
    {
        [Test]
        public void Compare_RegularGA_ChromosomeStub_DifferentSizes_ManyGenerations_DurationBounds()
        {
            var repeatNb = 10;
            var testParams = new List<(int size, double ratio)>
            {
                (10, 1.2), (100, 1.2), (500, 1.2), (5000, 1.2)
            };

            IFitness Fitness(int i) => new FitnessStub(i) {SupportsParallel = false};
            IChromosome AdamChromosome(int i) => new ChromosomeStub(i, i);

            var crossover = new UniformCrossover();
            
            var reinsertion = new FitnessBasedElitistReinsertion();

            Func<IEvolutionResult, IEvolutionResult, int> resultComparer = MeanEvolutionResult.CompareSequence(
                MeanEvolutionResult.CompareDuration, 
                MeanEvolutionResult.CompareFitness, 
                (r1, r2) => -MeanEvolutionResult.CompareGenerations(r1,r2));

            var heuristics = new List<IMetaHeuristic> { null, new DefaultMetaHeuristic() };

            var testResults = new List<(IEvolutionResult traditionalResult, IEvolutionResult metaEvolutionResult, ITermination termination, double ratio)>();
            foreach (var (size, ratio) in testParams)
            {
                
                var traditionalGaResult = new MeanEvolutionResult { ResultComparer = resultComparer, SkipExtremaPercentage = 0.2};
                var metaGaResult = new MeanEvolutionResult { ResultComparer = resultComparer, SkipExtremaPercentage = 0.2 };
                var nbGenerations = 10000 / size;
                var termination = new GenerationNumberTermination(nbGenerations);
                for (int i = 0; i < repeatNb; i++)
                {
                    var results = CompareMetaHeuristicsSamePopulation(1, Fitness(size), AdamChromosome(size),
                        heuristics, crossover, 100, termination, reinsertion);
                    //if (i>0)//Skip first evolution
                    //{
                        traditionalGaResult.Results.Add(results[0]);
                        metaGaResult.Results.Add(results[1]);
                    //}
                    
                }
                testResults.Add((traditionalGaResult, metaGaResult, termination, ratio));
                

            }

         testResults.ForEach(tuple=> AssertIsPerformingLessByRatio(tuple.termination, tuple.ratio, tuple.traditionalResult, tuple.metaEvolutionResult));


        }


        [Test]
        public void Compare_DefaultWithMatch_RegularGA_ChromosomeStub_DifferentSizes_ManyGenerations_HigherFitness()
        {
            var repeatNb = 5;
            var testParams = new List<(int size, double ratio)>
            {
                (500, 1), /*(1000, 1), (5000, 1)*/
            };

            IFitness Fitness(int i) => new FitnessStub(i) { SupportsParallel = false };
            IChromosome AdamChromosome(int i) => new ChromosomeStub(i, i);

            var crossover = new UniformCrossover();

            var reinsertion = new FitnessBasedElitistReinsertion();


            Func<IEvolutionResult, IEvolutionResult, int> resultComparer = MeanEvolutionResult.CompareSequence(
                MeanEvolutionResult.CompareDuration,
                MeanEvolutionResult.CompareFitness,
                (r1, r2) => -MeanEvolutionResult.CompareGenerations(r1, r2));

            var defaultMetaheuristic = new DefaultMetaHeuristic();


            var heuristics = new List<IMetaHeuristic>
            {
                null,
                //defaultMetaheuristic

            };


            foreach (var matchingTechnique in Enum.GetValues(typeof(MatchingKind)).Cast<MatchingKind>())
            {
                if (matchingTechnique!= MatchingKind.Best && matchingTechnique != MatchingKind.Worst && matchingTechnique != MatchingKind.Current && matchingTechnique != MatchingKind.Custom && matchingTechnique != MatchingKind.Child)
                {
                    var defaultMatch = new DefaultMetaHeuristic();
                    defaultMatch.MatchMetaHeuristic.WithMatches(new[] { MatchingKind.Current, matchingTechnique });
                    defaultMatch.MatchMetaHeuristic.EnableHyperSpeed = true;
                    heuristics.Add(defaultMatch);
                }
            }
           
            foreach (var (size, ratio) in testParams)
            {

                var meanResults = new List<MeanEvolutionResult>(); //;
                
                var nbGenerations = 100;
                var termination = new GenerationNumberTermination(nbGenerations);
                //var termination = new FitnessThresholdTermination(0.97);
                for (int i = 0; i < repeatNb; i++)
                {
                    var results = CompareMetaHeuristicsSamePopulation(1, Fitness(size), AdamChromosome(size),
                        heuristics, crossover, 100, termination, reinsertion);
                    //if (i>0)//Skip first evolution
                    //{
                    for (int j = 0; j < results.Count; j++)
                    {
                        if (meanResults.Count<j+1)
                        {
                            meanResults.Add(new MeanEvolutionResult { ResultComparer = resultComparer, SkipExtremaPercentage = 0.2 });
                        }

                        meanResults[j].Results.Add(results[j]);
                    }
                    
                    //}

                }

                var normalGaResult = meanResults[0];

                for (int i = 1; i < meanResults.Count-1; i++)
                {
                    Assert.GreaterOrEqual(meanResults[i].Fitness , normalGaResult.Fitness * ratio);
                }
                
                

            }




        }



        [Test]
        public void Compare_Hyperspeed_Match_ChromosomeStub_DifferentSizes_ManyGenerations_FasterWithBounds()
        {
            var repeatNb = 3;
            var testParams = new List<(int size, double ratio)>
            {
                (500, 2)//, (100, 1.1), (500, 1.1), (5000, 1.1)
            };

            IFitness Fitness(int i) => new FitnessStub(i) { SupportsParallel = false };
            IChromosome AdamChromosome(int i) => new ChromosomeStub(i, i);

            var crossover = new UniformCrossover();

            var reinsertion = new FitnessBasedElitistReinsertion();

            Func<IEvolutionResult, IEvolutionResult, int> resultComparer = MeanEvolutionResult.CompareSequence(
                MeanEvolutionResult.CompareDuration,
                MeanEvolutionResult.CompareFitness,
                (r1, r2) => -MeanEvolutionResult.CompareGenerations(r1, r2));


            var heuristics = new List<IMetaHeuristic>();


            foreach (var matchingTechnique in Enum.GetValues(typeof(MatchingKind)).Cast<MatchingKind>().Except(new []
                {
                    MatchingKind.Current,
                    MatchingKind.Custom,
                    MatchingKind.Child
                }))
            {
                var defaultMatch = new DefaultMetaHeuristic();
                defaultMatch.MatchMetaHeuristic.WithMatches(new[] { MatchingKind.Current,  matchingTechnique });
                defaultMatch.MatchMetaHeuristic.EnableHyperSpeed = false;
                heuristics.Add(defaultMatch);
                var hyperSpeedMatch = new DefaultMetaHeuristic();
                hyperSpeedMatch.MatchMetaHeuristic.WithMatches(new[] { MatchingKind.Current, matchingTechnique });
                hyperSpeedMatch.MatchMetaHeuristic.EnableHyperSpeed = true;
                heuristics.Add(hyperSpeedMatch);
            }

            foreach (var (size, ratio) in testParams)
            {

                var meanResults = new List<MeanEvolutionResult>(); //;

                var nbGenerations = 500;
                var termination = new GenerationNumberTermination(nbGenerations);
                //var termination = new FitnessThresholdTermination(0.97);
                for (int i = 0; i < repeatNb; i++)
                {
                    var results = CompareMetaHeuristicsSamePopulation(1, Fitness(size), AdamChromosome(size),
                        heuristics, crossover, 100, termination, reinsertion);
                    //if (i>0)//Skip first evolution
                    //{
                    for (int j = 0; j < results.Count; j++)
                    {
                        if (meanResults.Count < j + 1)
                        {
                            meanResults.Add(new MeanEvolutionResult { ResultComparer = resultComparer, SkipExtremaPercentage = 0.2 });
                        }

                        meanResults[j].Results.Add(results[j]);
                    }

                    //}

                }

                for (int i = 0; i < heuristics.Count/2; i++)
                {
                    AssertIsPerformingLessByRatio(termination, ratio, meanResults[2*i], meanResults[2 * i +1]);
                }
                
                Debugger.Break();

            }




        }



    }
}