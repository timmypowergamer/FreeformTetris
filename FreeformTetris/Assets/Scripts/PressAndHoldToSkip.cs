using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PressAndHoldToSkip : MonoBehaviour
{
    public string TitleScene;
    private float timeHeld = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButton("Submit") == true)
        {
            timeHeld += Time.deltaTime;
            if (timeHeld > 1)
            {
                SceneManager.LoadScene(TitleScene);
            }
        }
        else if (Input.GetButton("Submit") == false)
            timeHeld = 0;

    }
}
