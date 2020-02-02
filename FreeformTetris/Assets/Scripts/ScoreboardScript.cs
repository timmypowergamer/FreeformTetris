﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ScoreboardScript : MonoBehaviour
{
    public int score;
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<TextMeshProUGUI>().text = score + "%";
    }

    // Update is called once per frame
    void Update()
    {
        GetComponentInChildren<TextMeshProUGUI>().text = score + "%";
    }
}
