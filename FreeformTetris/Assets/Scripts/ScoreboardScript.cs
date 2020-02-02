using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreboardScript : MonoBehaviour
{
    public int score;
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<TextMesh>().text = score + "%";
    }

    // Update is called once per frame
    void Update()
    {
        GetComponentInChildren<TextMesh>().text = score + "%";
    }
}
