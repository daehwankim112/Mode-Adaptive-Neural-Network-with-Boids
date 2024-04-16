using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SIGGRAPH_2018;

public class BoidsManager : MonoBehaviour
{
    public GameObject BoidPrefab;
    public GameObject WolfPrefab;
    public GameObject WolvesContainer;

    public BoidController[] Floak;
    public BioAnimation_Wolf[] Wolves;


    [SerializeField] private int numberOfBoids = 10;
    [SerializeField] private float separationConstant = 1;
    [SerializeField] private float alignmentConstant = 1;
    [SerializeField] private float cohesionConstant = 1;
    [SerializeField] private float synchronizationConstant = 1;
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
        Floak = new BoidController[numberOfBoids];
        Wolves = new BioAnimation_Wolf[numberOfBoids];
        for (int i = 0; i < numberOfBoids; i++)
        {
            var BoidTemp = Instantiate(BoidPrefab, new Vector3(Random.Range(this.transform.position.x - 10f, this.transform.position.x + 10f), 0f, Random.Range(this.transform.position.z - 10f, this.transform.position.z + 10f)), Quaternion.identity);
            Vector2 tempRandomVector = Random.insideUnitCircle / 100f;
            BoidTemp.gameObject.name = "Boid" + i;
            BoidTemp.GetComponent<BoidController>().setAcceleration(new Vector3(tempRandomVector.x, 0f, tempRandomVector.y));
            BoidTemp.GetComponent<BoidController>().fixedVelocity = this.fixedVelocity;
            BoidTemp.transform.parent = this.transform;
            Floak[i] = BoidTemp.GetComponent<BoidController>();
            var WolfTemp = Instantiate(WolfPrefab, BoidTemp.transform.position, BoidTemp.transform.rotation);
            WolfTemp.SetActive(true);
            WolfTemp.gameObject.name = "Wolf" + i;
            WolfTemp.GetComponent<BioAnimation_Wolf>().BoidTarget = BoidTemp.transform;
            WolfTemp.transform.parent = WolvesContainer.transform;
            Wolves[i] = WolfTemp.GetComponent<BioAnimation_Wolf>();
            Floak[i].entity = Wolves[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < numberOfBoids; i++)
        {
            Floak[i].checkNeighbors(Floak, visualNeighborDistance, protectedNeighborDistance);
            Floak[i].alignment(alignmentConstant);
            Floak[i].cohesion(cohesionConstant);
            Floak[i].separation(separationConstant);
            Floak[i].synchronize(synchronizationConstant, Wolves[i]);
            Floak[i].updateboid();
            Floak[i].avoidWalls(edgeConstant, edgeBufferConstant);

            Wolves[i].BoidTarget = Floak[i].gameObject.transform;
        }

    }
}
