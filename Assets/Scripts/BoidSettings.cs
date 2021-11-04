using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoidSettings", menuName = "Boids", order = 1)]
public class BoidSettings : ScriptableObject
{
    public GameObject boidPrefab;
    public int numBoids = 100;
    public float spawnRadius = 5f;
    public float maxRadius = 15f;
    public int maxNeighbors = 5;
    public bool randomNeighbors = true;


    public float minSpeed = 5f;
    public float maxSpeed = 20f;

    public float maxAccel = 1f;

    public float separationWeight = 1f; 
    public float alignmentWeight = 1f;
    public float cohesionWeight = 0.5f;
    public float noiseWeight = 1f;

    public float perceptionRadius = 5f;
    public float avoidanceRadius = 2f;
    public float avoidanceWeight = 1f;

    public float movementSmooth = 1f;
    public float rotationSmooth = 1f;
}
