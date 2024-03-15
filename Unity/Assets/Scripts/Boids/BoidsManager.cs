﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsManager : MonoBehaviour
{
    public GameObject BoidPrefab;

    private BoidController[] floak;
    [SerializeField] private int numberOfBoids = 10;
    [SerializeField] private float alignConstant = 1;
    [SerializeField] private float neighborDistance = 1f;

    // Start is called before the first frame update
    void Start()
    {
        alignConstant /= 100;
        floak = new BoidController[numberOfBoids];
        for (int i = 0; i < numberOfBoids; i++)
        {
            var BoidTemp = Instantiate(BoidPrefab, new Vector3(Random.Range(this.transform.position.x - 5f, this.transform.position.x + 5f), 0f, Random.Range(this.transform.position.z - 5f, this.transform.position.z + 5f)), Quaternion.identity);
            Vector2 tempRandomVector = Random.insideUnitCircle / 100f;
            BoidTemp.gameObject.name = "Boid" + i;
            BoidTemp.GetComponent<BoidController>().setAcceleration(new Vector3(tempRandomVector.x, 0f, tempRandomVector.y));
            BoidTemp.GetComponent<BoidController>().alignConstant = this.alignConstant; 
            floak[i] = BoidTemp.GetComponent<BoidController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < numberOfBoids; i++)
        {
            floak[i].checkNeighbors(floak, neighborDistance);
            // floak[i].alignment();
            floak[i].cohesion();
        }

    }
}