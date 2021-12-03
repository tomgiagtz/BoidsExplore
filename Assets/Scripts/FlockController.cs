using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockController : MonoSingleton<FlockController> {
    public BoidSettings settings;
    public List<BoidController> boids;

    FastNoiseLite noise;

    public float timeScale = 1.0f;

    void Start() {
        for (int i = 0; i < settings.numBoids; i++) {
            BoidController boid = Instantiate(settings.boidPrefab).GetComponent<BoidController>();
            boid.Initialize(settings);
            boids.Add(boid);
        }

        noise = new FastNoiseLite();
    }

    // Update is called once per frame
    void Update() {
        Time.timeScale = timeScale;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, settings.maxRadius);
    }


    public BoidController[] FindNearbyBoidsEXP(BoidController boid) {
        //create a new array of neighbors
        BoidController[] nearbyBoids = new BoidController[settings.maxNeighbors];
        int count = 0;
        //get a random int between 0 and the number of boids as our offset
        int offset = Random.Range(0, boids.Count);
        //loop through all the boids
        for (int i = 0; i < boids.Count; i++) {
            //randomly offset search based on randomNeighbor setting
            int index =  settings.randomNeighbors ? (i + offset) % boids.Count : i;

            if (settings.randomNeighbors) {
                index = (i + offset) % boids.Count;
            }
            //get the boid out of the list
            BoidController other = boids[index];
            //ignore if the other boid is the same as the current boid (doesn't check itself)
            if (other == boid) continue;

            //if inside the radius, add to the list and increment count
            if (Vector3.Distance(boid.transform.position, other.transform.position) < settings.maxRadius) {
                nearbyBoids[count] = other;
                count++;
            }
            //if num neightbors == maxNieghbors, break out of the loop
            if (count >= settings.maxNeighbors) break;
        }
        //return list of boids
        return nearbyBoids;
    }
    
    public Vector3 GetFlockNoise(Vector3 position) {
        float currVal = noise.GetNoise(position.x, position.y, position.z);

        Vector3 noiseVec = new Vector3(Mathf.Cos(currVal * Mathf.PI * 2), Mathf.Sin(currVal * Mathf.PI * 2), 0);
        return noiseVec;
    }
}
