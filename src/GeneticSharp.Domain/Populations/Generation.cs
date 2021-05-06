using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Infrastructure.Framework.Collections;
using GeneticSharp.Infrastructure.Framework.Texts;

namespace GeneticSharp.Domain.Populations
{
    /// <summary>
    /// Represents a generation of a population.
    /// </summary>
    [DebuggerDisplay("{Number} = {BestChromosome.Fitness}, {ChromosomesNb} chromosomes")]
    public sealed class Generation
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Populations.Generation"/> class.
        /// </summary>
        /// <param name="number">The generation number.</param>
        /// <param name="chromosomes">The chromosomes of the generation..</param>
        public Generation(int number, IList<IChromosome> chromosomes)
        {
            if (number < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(number),
                    "Generation number {0} is invalid. Generation number should be positive and start in 1.".With(number));
            }

            if (chromosomes == null || chromosomes.Count < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(chromosomes), "A generation should have at least 2 chromosomes.");
            }

            Number = number;
            CreationDate = DateTime.Now;
            Chromosomes = chromosomes;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number.
        /// </summary>
        /// <value>The number.</value>
        public int Number { get; private set; }

        /// <summary>
        /// Gets the creation date.
        /// </summary>
        public DateTime CreationDate { get; private set; }


        private int ChromosomesNb => Chromosomes.Count;

        /// <summary>
        /// Gets the chromosomes.
        /// </summary>
        /// <value>The chromosomes.</value>
        public IList<IChromosome> Chromosomes { get; internal set; }

        /// <summary>
        /// Gets the best chromosome.
        /// </summary>
        /// <value>The best chromosome.</value>
        public IChromosome BestChromosome { get; internal set; }

        public IEnumerable<IChromosome> GetBestChromosomes(int nbChromosomes)
        {
            return Chromosomes.LazyOrderBy(c => -c.Fitness ?? 0).Take(nbChromosomes);
        }

        public IEnumerable<IChromosome> GetWorstChromosomes(int nbChromosomes)
        {
            return Chromosomes.LazyOrderBy(c => c.Fitness ?? 0).Take(nbChromosomes);
        }

        #endregion

        #region Methods
        /// <summary>
        /// Ends the generation.
        /// </summary>
        /// <param name="chromosomesNumber">Chromosomes number to keep on generation.</param>
        public void End(int chromosomesNumber)
        {
            ValidateChromosomes();
            if (Chromosomes.Count > chromosomesNumber)
            {
                Chromosomes = Chromosomes.Take(chromosomesNumber).ToList();
            }
            BestChromosome = Chromosomes.MaxBy(chromosome =>  chromosome.Fitness.Value);
        }


        /// <summary>
        /// Validates the chromosome.
        /// </summary>
        /// <returns>True if all chromosomes are valid.</returns>
        private void ValidateChromosomes()
        {
            foreach (var chromosome in Chromosomes)
            {
                ValidateChromosome(chromosome);
            }
        }


        /// <summary>
        /// Validates the chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome to validate.</param>
        /// <returns>True if a chromosome is valid.</returns>
        private static void ValidateChromosome(IChromosome chromosome)
        {
            if (!chromosome.Fitness.HasValue)
            {
                throw new InvalidOperationException("There is unknown problem in current generation, because a chromosome has no fitness value.");
            }
        }

        


        #endregion
    }
}
