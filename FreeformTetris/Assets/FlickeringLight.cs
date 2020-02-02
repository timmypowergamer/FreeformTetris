using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    Light light;
    float timeout = 0;
    float flickerTimeout = 0;
    float flickerMag = 0;
    float flickerRate = 0;

    // Start is called before the first frame update
    void Start()
    {
        timeout = Random.Range(0.1f, 3);
        flickerMag = Random.Range(0.7f, 0.9f);
        flickerRate = Random.Range(0, 1);
        light = gameObject.GetComponentInChildren<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        timeout -= Time.deltaTime;
        flickerTimeout -= Time.deltaTime;
        if (timeout <= 0)
        {
            flickerMag = flickerMag == 1 ? Random.Range(0.7f, 0.9f) : 1;
            timeout = flickerMag == 1 ? Random.Range(2, 8) : Random.Range(0.5f, 3);
            flickerRate = Random.Range(0.05f, 0.1f);
        }
        else if (flickerTimeout <= 0)
        {
            flickerTimeout = flickerRate;
            light.intensity = light.intensity == 1 ? flickerMag : 1;
        }
    }
}
