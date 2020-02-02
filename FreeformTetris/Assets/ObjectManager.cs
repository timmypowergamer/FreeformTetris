using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance;

    public GrabbableObject[] prefabs;
    const int max = 100;
    float nextSpawn = 0.0f;
    const float spawnTick = 0.65f;
    bool running = false;
	//float spawnradius = 4f;

	List<GrabbableObject> spawnedObjects = new List<GrabbableObject>();

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
		GameManager.OnGameFinished += DespawnAll;
	}

	private void OnDisable()
    {
        GameManager.OnGameStarted -= startRunning;
		GameManager.OnGameFinished -= DespawnAll;
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
                if (spawnedObjects.Count < max)
                {
                    spawnObject();
                }
            }
        }
    }

    private void spawnObject()
    {
		//Vector3 spawnLoc = Random.insideUnitCircle * spawnradius;
        var ind = Random.Range(0, prefabs.Length);
        var obj = GameObject.Instantiate(prefabs[ind], Vector3.zero, Random.rotation, this.transform);
		obj.transform.localPosition = Vector3.zero;
        var scale = Random.Range(0.9f, 1.5f);
        scale = Mathf.Pow(scale, 2);
        obj.transform.localScale = new Vector3(scale, scale, scale);
		spawnedObjects.Add(obj);
    }

	public void RemoveObject(GrabbableObject obj)
	{
		if(spawnedObjects.Contains(obj))
		{
			spawnedObjects.Remove(obj);
		}
	}

	public void AddObject(GrabbableObject obj)
	{
		if (!spawnedObjects.Contains(obj))
		{
			spawnedObjects.Add(obj);
		}
	}

	public void DespawnAll()
	{
		running = false;
		foreach(GrabbableObject obj in new List<GrabbableObject>(spawnedObjects))
		{
			RemoveObject(obj);
			Destroy(obj.gameObject);
		}
	}
}
