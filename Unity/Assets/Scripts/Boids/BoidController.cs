using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    // [SerializeField] private LineRenderer seperationLine;
    [SerializeField] private LineRenderer alignmentLine;
    private int alignmentLineIndex = 0;
    // [SerializeField] private LineRenderer cohesionLine;

    private BoidController[] neighbors;
    // private List<BoidController> neighbors = new List<BoidController>();

    [SerializeField] private Vector3 velocity;
    [SerializeField] private Vector3 acceleration;

    public float maxVelocity = 0.01f;
    public float separationConstant = 1;
    public float alignmentConstant = 1;
    public float cohesionConstant = 1;

    // Start is called before the first frame update
    void Start()
    {
        alignmentLine.positionCount = 0;
        alignmentLine = GetComponent<LineRenderer>();
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
        velocity = Vector3.ClampMagnitude(velocity, maxVelocity);
        
        // keep the position of the boid above the terrain
        this.gameObject.transform.position = new Vector3(transform.position.x, Terrain.activeTerrain.SampleHeight(transform.position) + 0.1f, transform.position.z);
        this.gameObject.transform.position += velocity;
        acceleration = Vector3.zero;
    }

    // update neighboring boids
    public void checkNeighbors(BoidController[] boidcontrollers, float distance)
    {
        int neighborsSize = 0;
        int neighborIndex = 0;
        foreach (var boid in boidcontrollers)
        {
            if (boid != this && Vector3.Distance(this.transform.position, boid.gameObject.transform.position) < distance)
            {
                neighborsSize++;
            }
        }
        neighbors = new BoidController[neighborsSize];
        foreach (var boid in boidcontrollers)
        {
            if (boid != this && Vector3.Distance(this.transform.position, boid.gameObject.transform.position) < distance)
            {
                neighbors[neighborIndex] = boid.GetComponent<BoidController>();
                neighborIndex++;
            }
        }

        drawLines();
    }


    // separate this entity with neighboring boids
    public void separation()
    {
        Vector3 steering = Vector3.zero;
        foreach (var boid in neighbors)
        {
            steering += (this.transform.position - boid.transform.position) / Vector3.Magnitude(this.transform.position - boid.transform.position);
        }
        if (neighbors.Length > 0)
        {
            steering /= neighbors.Length;
        }
        // steering = Vector3.ClampMagnitude(steering, separationConstant);
        steering *= separationConstant;
        this.acceleration += steering;
    }

    // align this entity with neighboring boids
    public void alignment()
    {
        Vector3 steering = Vector3.zero;
        foreach (var boid in neighbors)
        {
            steering += boid.velocity;
        }
        if (neighbors.Length > 0)
        {
            steering /= neighbors.Length;
            steering -= this.velocity;
        }
        // steering = Vector3.ClampMagnitude(steering, alignmentConstant);
        steering *= alignmentConstant;
        this.acceleration += steering;
    }

    // cohesion this entity with neighboring boids
    public void cohesion()
    {
        Vector3 steering = Vector3.zero;
        foreach (var boid in neighbors)
        {
            steering += boid.transform.position;
        }
        if (neighbors.Length > 0)
        {
            steering /= neighbors.Length;
            steering -= this.transform.position;
        }
        // steering = Vector3.ClampMagnitude(steering, cohesionConstant);
        steering *= cohesionConstant;
        this.acceleration += steering;
    }

    // draw neighboring lines
    public void drawLines()
    {
        // draw lines for alignment neighbors
        Vector3[] linePositions = new Vector3[neighbors.Length * 2];
        for (int i = 0; i < neighbors.Length * 2; i++)
        {
            linePositions[i] = neighbors[i/2].gameObject.transform.position;
            linePositions[++i] = this.transform.position;
        }
        alignmentLine.positionCount = neighbors.Length * 2;
        if (neighbors.Length > 0)
        {
            alignmentLine.SetPositions(linePositions);
        }
    }
}
