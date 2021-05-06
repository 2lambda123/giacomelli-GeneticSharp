﻿using System.ComponentModel;
using GeneticSharp.Domain.Metaheuristics.Parameters;

namespace GeneticSharp.Domain.Metaheuristics.Primitives
{
    /// <summary>
    /// The population based Metaheuristcs allows applying distinct Metaheuristics depending on the individual index. By default, it divides the individuals into distinct phase sets proportional to the phase sizes 
    /// </summary>
    [DisplayName("Population")]
    public class PopulationMetaHeuristic : SizeBasedMetaHeuristic
    {

        public PopulationMetaHeuristic()
        {
            Init();
        }



        public PopulationMetaHeuristic(int groupSize, params IMetaHeuristic[] phaseHeuristics) : base(groupSize,
            phaseHeuristics)
        {
            Init();
        }

        private void Init()
        {
            DynamicParameter = new ExpressionMetaHeuristicParameter<int>
            {
                Scope = ParamScope.Generation,
                DynamicGenerator = (h, ctx) => ctx.LocalIndex 
            };
        }


    }
}