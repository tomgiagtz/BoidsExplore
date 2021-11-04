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
        BoidController[] nearbyBoids = new BoidController[settings.maxNeighbors];
        int count = 0;
        int offset = Random.Range(0, boids.Count);
        for (int i = 0; i < boids.Count; i++) {
            //randomly offset search basedo on randomNeighbor setting
            int index =  settings.randomNeighbors ? (i + offset) % boids.Count : i;

            if (settings.randomNeighbors) {
                index = (i + offset) % boids.Count;
            }
            BoidController other = boids[index];
            if (other == boid) continue;
            if (Vector3.Distance(boid.transform.position, other.transform.position) < settings.maxRadius) {
                nearbyBoids[count] = other;
                count++;
            }
            if (count >= settings.maxNeighbors) break;
        }

        return nearbyBoids;

    }
    
    public Vector3 GetFlockNoise(Vector3 position) {
        float currVal = noise.GetNoise(position.x, position.y, position.z);

        Vector3 noiseVec = new Vector3(Mathf.Cos(currVal * Mathf.PI * 2), Mathf.Sin(currVal * Mathf.PI * 2), 0);
        return noiseVec;
    }
}
