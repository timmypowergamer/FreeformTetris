using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClampScore : MonoBehaviour
{
    public Text scoreLabel;

    // Update is called once per frame
    void Update()
    {
        Vector3 scorePos = Camera.main.WorldToScreenPoint(this.transform.position);
        scoreLabel.transform.position = scorePos;
    }
}
