using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[SelectionBase]
public class BoidController : MonoBehaviour {


    Color debugColor;
    
    Vector3 velocity;
    BoidSettings settings;
    BoidController[] neighbors;

    public void Initialize(BoidSettings settings_) {
        settings = settings_;
        neighbors = new BoidController[settings.maxNeighbors];
        debugColor = Random.ColorHSV();
        
        Vector3 startingPos = Random.insideUnitSphere * settings.spawnRadius;
        velocity = Random.insideUnitSphere * ((settings.maxSpeed - settings.minSpeed) / 2f + settings.minSpeed);
    }
    
    
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update() {
        Tick();
        UpdateNeighbors();
    }

    Vector3 EdgeAvoidance() {
        if (transform.position.magnitude > settings.maxRadius) {
            Debug.Log("Past an edge");
            transform.position = transform.position.normalized * (settings.maxRadius - settings.avoidanceRadius / 2f);
        }
        if (transform.position.magnitude > settings.maxRadius - settings.avoidanceRadius) {
            float strength = 1 / (settings.maxRadius - transform.position.magnitude);
            Vector3 avoidanceDir = -transform.position.normalized;

            return avoidanceDir * settings.avoidanceWeight * strength;
        }
        return Vector3.zero;
    }

    Vector3 Separation() {
        Vector3 separationDir = Vector3.zero;
        foreach (BoidController neighbor in neighbors) {
            if (neighbor != null) {
                Vector3 toNeighbor = transform.position - neighbor.transform.position;
                float distance = toNeighbor.magnitude;
                if (distance < settings.avoidanceRadius) {
                    separationDir += toNeighbor.normalized * 1 / distance;
                }
            }
        }
        return separationDir.normalized * settings.separationWeight;
    }

    Vector3 Noise() {
        return FlockController.Instance.GetFlockNoise(transform.position) * settings.noiseWeight;
    }

    void Tick() {

        Vector3 acceleration = Vector3.zero;

        acceleration += EdgeAvoidance();
        acceleration += Separation();
        acceleration += Noise();

        Vector3 accelDir = acceleration.normalized;
        float mag = Mathf.Clamp(acceleration.magnitude, -settings.maxAccel, settings.maxAccel);

        velocity += mag * accelDir;
        ApplyMovement();
    }

    void ApplyMovement() {
        //apply speedlimits
        if (velocity.magnitude > settings.maxSpeed) {
            velocity = velocity.normalized * settings.maxSpeed;
        }
        if (velocity.magnitude < settings.minSpeed) {
            velocity = velocity.normalized * settings.minSpeed;
        }



        transform.position += velocity * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(velocity);
    }

    void UpdateNeighbors() {
        neighbors = FlockController.Instance.FindNearbyBoidsEXP(this);
    }


    void OnDrawGizmosSelected() {
        Debug.Log(Separation());
        Gizmos.color = debugColor;
        foreach (BoidController neighbor in neighbors) {
            if (neighbor != null) {
                Gizmos.DrawLine(transform.position, neighbor.transform.position);
            }
        }
    }

    

}