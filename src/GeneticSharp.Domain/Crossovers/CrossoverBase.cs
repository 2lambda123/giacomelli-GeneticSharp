﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HelperSharp;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Domain.Crossovers
{
	/// <summary>
	/// A base class for crossovers.
	/// </summary>
    public abstract class CrossoverBase : ICrossover
    {
        #region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="GeneticSharp.Domain.Crossovers.CrossoverBase"/> class.
		/// </summary>
		/// <param name="parentsNumber">The number of parents need for cross.</param>
		/// <param name="childrenNumber">The number of children generated by cross.</param>
        protected CrossoverBase(int parentsNumber, int childrenNumber)
        {
	        ParentsNumber = parentsNumber;
			ChildrenNumber = childrenNumber;
        }
        #endregion

		#region Properties
		/// <summary>
		/// Gets the number of parents need for cross.
		/// </summary>
		/// <value>The parents number.</value>
        public int ParentsNumber { get; private set; }

		/// <summary>
		/// Gets the number of children generated by cross.
		/// </summary>
		/// <value>The children number.</value>
		public int ChildrenNumber  { get; private set; }
		#endregion

		#region Methods
		/// <summary>
		/// Cross the specified parents generating the children.
		/// </summary>
		/// <param name="parents">Parents.</param>
		/// <returns>The offspring (children) of the parents.</returns>
        public IList<IChromosome> Cross(IList<IChromosome> parents)
        {
            ExceptionHelper.ThrowIfNull("parents", parents);

            if (parents.Count != ParentsNumber)
            {
                throw new ArgumentOutOfRangeException("parents", "The number of parents should be the same of ParentsNumber.");
            }

            return PerformCross(parents);
        }

		/// <summary>
		/// Performs the cross with specified parents generating the children.
		/// </summary>
		/// <param name="parents">Parents.</param>
		/// <returns>The offspring (children) of the parents.</returns>
        protected abstract IList<IChromosome> PerformCross(IList<IChromosome> parents);
        #endregion
    }
}
