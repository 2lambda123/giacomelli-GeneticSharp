﻿using System;

namespace GeneticSharp.Domain.Metaheuristics
{
    [Flags]
    public enum ProbabilityStrategy
    {
        PassToDescendents = 0,
        TestProbability = 1,
        OverwriteProbability = 2
    }
}