using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[SelectionBase]
public class BoidController : MonoBehaviour {


    Color debugColor;
    
    Vector3 velocity;
    BoidSettings settings;
    BoidController[] neighbors;

    void Start() {
        Animator anim = GetComponent<Animator>();
        anim.SetFloat("Offset", Random.Range(0.7f, 1.3f));
    }

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
        //if the boid is outside the spawn radius, move it back in to the distance of our avoidance radius
        if (transform.position.magnitude > settings.maxRadius) {
            transform.position = transform.position.normalized * (settings.maxRadius - settings.avoidanceRadius / 2f);
        }
        //if the boid is inside the spawn radius, but within it's avoidance distance
        if (transform.position.magnitude > settings.maxRadius - settings.avoidanceRadius) {
            //get the distance from the boid to the edge of the spawn radius
            float distance = settings.maxRadius - transform.position.magnitude;
            //set the strength to the inverse square of the distance
            float strength = 1 / (distance * distance);
            //invert the direction
            Vector3 avoidanceDir = -transform.position.normalized;

            //return multiplyied with our strength
            return avoidanceDir * settings.avoidanceWeight * strength;
        }
        return Vector3.zero;
    }

    Vector3 Separation() {
        //our result vector, initialized to 0,0,0
        Vector3 separationDir = Vector3.zero;
        //for each neighbor
        foreach (BoidController neighbor in neighbors) {
            if (neighbor != null) {
                //vector between the neighbor and this boid
                Vector3 toNeighbor = transform.position - neighbor.transform.position;
                //magnitude is the distance between the two
                float distance = toNeighbor.magnitude;
                if (distance < settings.avoidanceRadius) {
                    //use inverse square law to determine strength
                    separationDir += toNeighbor.normalized * 1 / distance * distance;
                }
            }
        }
        //still gets scaled linearally by the weight
        return separationDir.normalized * settings.separationWeight;
    }


    //adds all velocities of neighbors to get the average velocity
    Vector3 Alignment() {
        Vector3 alignmentDir = Vector3.zero;
        //keeping track of how many neighbors regardless of max neighbors
        int neighborCount = 0;

        // add all velocties of valid neighbors
        foreach (BoidController neighbor in neighbors) {
            if (neighbor != null) {
                alignmentDir += neighbor.velocity;
                neighborCount++;
            }
        }
        //take average of all neighbors velocity
        alignmentDir /= neighborCount;
        //scale by weight
        return alignmentDir.normalized * settings.alignmentWeight;
    }

    Vector3 Cohesion() {
        Vector3 centerOfMass = Vector3.zero;
        int neighborCount = 0;
        foreach (BoidController neighbor in neighbors) {
            if (neighbor != null) {
                centerOfMass += neighbor.transform.position;
                neighborCount++;
            }
        }
        if (neighborCount > 0) {
            centerOfMass /= neighborCount;
            return (centerOfMass - transform.position).normalized * settings.cohesionWeight;
        }
        return Vector3.zero;
    }

    Vector3 Noise() {
        return FlockController.Instance.GetFlockNoise(transform.position) * settings.noiseWeight;
    }

    void Tick() {

        Vector3 acceleration = Vector3.zero;

        acceleration += EdgeAvoidance();
        acceleration += Separation();
        acceleration += Cohesion();
        acceleration += Alignment();
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

        Vector3 targetPos = transform.position + velocity;

        

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * settings.movementSmooth);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(velocity), Time.deltaTime * settings.rotationSmooth);
    }

    void UpdateNeighbors() {
        neighbors = FlockController.Instance.FindNearbyBoidsEXP(this);
    }


    void OnDrawGizmos() {
        Gizmos.color = debugColor;
        foreach (BoidController neighbor in neighbors) {
            if (neighbor != null) {
                Gizmos.DrawLine(transform.position, neighbor.transform.position);
            }
        }
    }

    

}
