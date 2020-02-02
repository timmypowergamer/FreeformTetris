using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance;

    public GrabbableObject[] prefabs;
    public int freeObjects;
    const int max = 30;
    float nextSpawn = 0.0f;
    const float spawnTick = 1.0f;
    bool running = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Detected more than one ObjectManager in the scene! : {gameObject.name}");
            return;
        }
        Instance = this;
    }

	private void OnEnable()
	{
		GameManager.OnGameStarted += startRunning;
	}

	private void OnDisable()
    {
        GameManager.OnGameStarted -= startRunning;
    }

    private void startRunning()
    {
        running = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextSpawn)
        {
            nextSpawn += spawnTick;
            if(running)
            {
                if (freeObjects < max)
                {
                    spawnObject();
                }
            }
        }
    }

    private void spawnObject()
    {
        var ind = Random.Range(0, prefabs.Length - 1);
        var obj = GameObject.Instantiate(prefabs[ind], this.transform);
        var scale = Random.Range(0.9f, 1.5f);
        scale = Mathf.Pow(scale, 2);
        obj.transform.localScale = new Vector3(scale, scale, scale);
        freeObjects++;
    }
}
