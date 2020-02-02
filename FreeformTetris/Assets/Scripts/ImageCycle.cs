using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ImageCycle : MonoBehaviour
{
    public string TitleScene;
    public Texture2D[] slides = new Texture2D[19];
    public Texture2D[] altslides = new Texture2D[19];
    public float[] changeTime = new float[19];
    public bool[] useAlt = new bool[19];
    public int currentSlide = 1;
    private bool usingAlt = false;
    private float timeSinceLast = 0.0f;
    private float timeSinceAlt = 0.0f;
    private float altTime = 1.0f;
    public RawImage slidecanvas;
    public AudioClip blast;
    public AudioClip crash;
    public AudioClip thud;
    public AudioClip sciFi;
    public AudioClip flying;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (timeSinceLast > changeTime[currentSlide] && currentSlide < slides.Length)
        {
            slidecanvas.texture = slides[currentSlide];
            if (currentSlide == 3) SoundManager.instance.playSingle(crash);
            if (currentSlide == 5) SoundManager.instance.playSingle(flying);
            if (currentSlide == 6) SoundManager.instance.playSingle(thud);
            if (currentSlide == 18) SoundManager.instance.playSingle(blast);
            if (currentSlide == 19) SoundManager.instance.playSingle(sciFi);
            timeSinceLast = 0.0f;
            currentSlide++;
        }

       // if (currentSlide == 1)
       //     slidecanvas.color = new Color32(255, 255, 255, 100)


        if (useAlt[currentSlide-1] == true)
        {
            if (usingAlt == false && timeSinceAlt > altTime)
            {
                slidecanvas.texture = altslides[currentSlide-1];
                usingAlt = true;
                timeSinceAlt = 0.0f;
            }
            if (usingAlt == true && timeSinceAlt > altTime)
            {
                slidecanvas.texture = slides[currentSlide-1];
                usingAlt = false;
                timeSinceAlt = 0.0f;
            }
            timeSinceAlt += Time.deltaTime;
       }

        timeSinceLast += Time.deltaTime;
        if (timeSinceLast > changeTime[currentSlide] && currentSlide >= slides.Length)
        {
            SceneManager.LoadScene(TitleScene);
        }
    }
}
