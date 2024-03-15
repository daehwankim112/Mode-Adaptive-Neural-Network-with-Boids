using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    [SerializeField] private LineRenderer NeighborsLine;
    private int alignmentLineIndex = 0;
    // [SerializeField] private LineRenderer cohesionLine;

    private BoidController[] visualNeighbors;
    private BoidController[] protectedNeighbors;
    // private List<BoidController> neighbors = new List<BoidController>();

    [SerializeField] private Vector3 velocity;
    [SerializeField] private Vector3 acceleration;

    public float fixedVelocity = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        NeighborsLine.positionCount = 0;
        NeighborsLine = GetComponent<LineRenderer>();
    }

    // set methods
    public void setVelocity(Vector3 velocityInput)
    {
        velocity = velocityInput;
    }

    public void setAcceleration(Vector3 accelerationInput)
    {
        acceleration = accelerationInput;
    }

    // get methods
    public Vector3 getVelocity()
    {
        return velocity;
    }


    public Vector3 getAcceleration()
    {
        return acceleration;
    }

    // Update is called once per frame
    public void updateboid()
    {
        velocity += acceleration;
        velocity = velocity.normalized * fixedVelocity;
        
        // keep the position of the boid above the terrain
        this.gameObject.transform.position = new Vector3(transform.position.x, Terrain.activeTerrain.SampleHeight(transform.position) + 0.1f, transform.position.z);
        this.gameObject.transform.position += velocity;
        acceleration = Vector3.zero;
    }

    // update neighboring boids
    public void checkNeighbors(BoidController[] boidcontrollers, float visualDistance, float protectedDistance)
    {
        int visualNeighborsSize = 0;
        int visualNeighborIndex = 0;
        int protectedNeighborSize = 0;
        int protectedNeighborIndex = 0;

        // count number of neighbors
        foreach (var boid in boidcontrollers)
        {
            if (boid != this && Vector3.Distance(this.transform.position, boid.gameObject.transform.position) < visualDistance)
            {
                visualNeighborsSize++;
            }
            if (boid != this && Vector3.Distance(this.transform.position, boid.gameObject.transform.position) < protectedDistance)
            {
                protectedNeighborSize++;
            }
        }

        // initialize neighbors array
        visualNeighbors = new BoidController[visualNeighborsSize];
        protectedNeighbors = new BoidController[protectedNeighborSize];

        // populate neighbors array
        foreach (var boid in boidcontrollers)
        {
            if (boid != this && Vector3.Distance(this.transform.position, boid.gameObject.transform.position) < visualDistance)
            {
                visualNeighbors[visualNeighborIndex] = boid.GetComponent<BoidController>();
                visualNeighborIndex++;
            }
            if (boid != this && Vector3.Distance(this.transform.position, boid.gameObject.transform.position) < protectedDistance)
            {
                protectedNeighbors[protectedNeighborIndex] = boid.GetComponent<BoidController>();
                protectedNeighborIndex++;
            }
        }

        // render lines
        drawLines();
    }



    // separate this entity with neighboring boids
    public void separation(float separationConstant)
    {
        Vector3 steering = Vector3.zero;
        foreach (var boid in protectedNeighbors)
        {
            steering += (this.transform.position - boid.transform.position) / Vector3.Magnitude(this.transform.position - boid.transform.position);
        }
        if (protectedNeighbors.Length > 0)
        {
            steering /= protectedNeighbors.Length;
        }
        // steering = Vector3.ClampMagnitude(steering, separationConstant);
        steering *= separationConstant;
        this.acceleration += steering;
    }

    // align this entity with neighboring boids
    public void alignment(float alignmentConstant)
    {
        Vector3 steering = Vector3.zero;
        foreach (var boid in visualNeighbors)
        {
            steering += boid.velocity;
        }
        if (visualNeighbors.Length > 0)
        {
            steering /= visualNeighbors.Length;
            steering -= this.velocity;
        }
        // steering = Vector3.ClampMagnitude(steering, alignmentConstant);
        steering *= alignmentConstant;
        this.acceleration += steering;
    }

    // cohesion this entity with neighboring boids
    public void cohesion(float cohesionConstant)
    {
        Vector3 steering = Vector3.zero;
        foreach (var boid in visualNeighbors)
        {
            steering += boid.transform.position;
        }
        if (visualNeighbors.Length > 0)
        {
            steering /= visualNeighbors.Length;
            steering -= this.transform.position;
        }
        // steering = Vector3.ClampMagnitude(steering, cohesionConstant);
        steering *= cohesionConstant;
        this.acceleration += steering;
    }

    public void avoidWalls(float edgeConstant, float edgeBufferConstant)
    {
        if (transform.position.x < -edgeBufferConstant)
        {
            this.acceleration.x -= edgeConstant * fixedVelocity * (transform.position.x + edgeBufferConstant);
        }
        if (transform.position.x > edgeBufferConstant)
        {
            this.acceleration.x -= edgeConstant * fixedVelocity * (transform.position.x - edgeBufferConstant);
        }
        if (transform.position.z < -edgeBufferConstant)
        {
            this.acceleration.z -= edgeConstant * fixedVelocity * (transform.position.z + edgeBufferConstant);
        }
        if (transform.position.z > edgeBufferConstant)
        {
            this.acceleration.z -= edgeConstant * fixedVelocity * (transform.position.z - edgeBufferConstant);
        }
    }


    // draw neighboring lines
    public void drawLines()
    {
        // draw lines for visual neighbors
        Vector3[] linePositions_v = new Vector3[visualNeighbors.Length * 2];
        for (int i = 0; i < visualNeighbors.Length * 2; i++)
        {
            linePositions_v[i] = visualNeighbors[i/2].gameObject.transform.position;
            linePositions_v[++i] = this.transform.position;
        }
        NeighborsLine.positionCount = visualNeighbors.Length * 2;
        if (visualNeighbors.Length > 0)
        {
            NeighborsLine.SetPositions(linePositions_v);
        }
    }
}
