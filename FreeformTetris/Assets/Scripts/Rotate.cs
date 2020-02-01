using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float rotationRate = 10f;
    public Vector3 rotation = new Vector3(1, 1, 1);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotation, rotationRate * Time.deltaTime);
    }
}
