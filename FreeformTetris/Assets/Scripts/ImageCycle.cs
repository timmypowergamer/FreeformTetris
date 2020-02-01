using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ImageCycle : MonoBehaviour
{
    public string TitleScene;
    public Texture2D[] slides = new Texture2D[19];
    public float[] changeTime = new float[19];
    private int currentSlide = 1;
    private float timeSinceLast = 0.0f;
    public RawImage slidecanvas;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (timeSinceLast > changeTime[currentSlide-1] && currentSlide-1 < slides.Length)
        {
            slidecanvas.texture = slides[currentSlide-1];
            timeSinceLast = 0.0f;
            currentSlide++;
        }
        timeSinceLast += Time.deltaTime;
        Debug.Log("timeSinceLast: " + timeSinceLast.ToString());
        Debug.Log("currentSlide: " + currentSlide.ToString());
        Debug.Log("slides.length: " + slides.Length.ToString());
        if (timeSinceLast > changeTime[currentSlide-1] && currentSlide >= slides.Length)
        {
            SceneManager.LoadScene(TitleScene);
        }
    }
}
