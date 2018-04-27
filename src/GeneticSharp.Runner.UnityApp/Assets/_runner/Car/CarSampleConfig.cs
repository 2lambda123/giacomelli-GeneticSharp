﻿using UnityEngine;

[CreateAssetMenu(menuName = "GeneticSharp/Car/CarSampleConfig", order = 1)]
public class CarSampleConfig : ScriptableObject {

    [Header("Road")]
    public int PointsCount = 100;
    public float MinPointsDistance = 2;
    public float MaxPointsDistance = 4;

    public float MaxHeight = 1f;
    public float GapsRate = 0.1f;
    public float MaxGapWidth = 1f;

    public Vector2 MaxObstacleSize = new Vector2(5, 5);
    public int ObstaclesEachPoints = 5;
    public int MaxObstaclesPerPoint = 2; 

    [Header("Evaluation")]
    public float WarmupTime = 10f;
    public float TimeoutNoBetterMaxDistance = 5f;
    public float MinMaxDistanceDiff = 1f;

    [Header("Car")]
    [Range(2, 100)]
    public int VectorsCount = 8;

    [Range(1, 100)]
    public float MinVectorSize = 5;

    [Range(2, 100)]
    public float MaxVectorSize = 10;

    [Range(2, 100)]
    public int WheelsCount = 2;

    [Range(1, 10)]
    public float MaxWheelRadius = 1;

    [Range(1, 1000)]
    public float MaxWheelSpeed = 800f;
}
