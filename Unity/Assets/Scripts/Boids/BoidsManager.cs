using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsManager : MonoBehaviour
{
    public GameObject BoidPrefab;

    private BoidController[] floak;
    [SerializeField] private int numberOfBoids = 10;
    [SerializeField] private float separationConstant = 1;
    [SerializeField] private float alignmentConstant = 1;
    [SerializeField] private float cohesionConstant = 1;
    [SerializeField] private float visualNeighborDistance = 2f;
    [SerializeField] private float protectedNeighborDistance = 1f;
    [SerializeField] private float fixedVelocity = 0.01f;
    [SerializeField] private float edgeConstant = 2f;
    [SerializeField] private float edgeBufferConstant = 18f;


    // Start is called before the first frame update
    void Start()
    {
        separationConstant /= 100;
        alignmentConstant /= 100;
        cohesionConstant /= 100;
        floak = new BoidController[numberOfBoids];
        for (int i = 0; i < numberOfBoids; i++)
        {
            var BoidTemp = Instantiate(BoidPrefab, new Vector3(Random.Range(this.transform.position.x - 10f, this.transform.position.x + 10f), 0f, Random.Range(this.transform.position.z - 10f, this.transform.position.z + 10f)), Quaternion.identity);
            Vector2 tempRandomVector = Random.insideUnitCircle / 100f;
            BoidTemp.gameObject.name = "Boid" + i;
            BoidTemp.GetComponent<BoidController>().setAcceleration(new Vector3(tempRandomVector.x, 0f, tempRandomVector.y));
            BoidTemp.GetComponent<BoidController>().fixedVelocity = this.fixedVelocity; 
            floak[i] = BoidTemp.GetComponent<BoidController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < numberOfBoids; i++)
        {
            floak[i].checkNeighbors(floak, visualNeighborDistance, protectedNeighborDistance);
            floak[i].alignment(alignmentConstant);
            floak[i].cohesion(cohesionConstant);
            floak[i].separation(separationConstant);
            floak[i].updateboid();
            floak[i].avoidWalls(edgeConstant, edgeBufferConstant);
        }

    }
}
