using UnityEngine;

public class TimerDisplay : MonoBehaviour
{

    [Header("Timer Attributes")]
    public float totalTime;
    public float currentTime;

    [Header("Sprite Renderers")]
    public SpriteRenderer tens;
    public SpriteRenderer ones;
    public SpriteRenderer tenths;
    public SpriteRenderer bulbs;

    [Header("Sprites")]
    public Sprite[] numSprites = new Sprite[10];
    public Sprite[] bulbSprites = new Sprite[2];

    private bool paused = true; //false == unpaused, //true == paused;

    void Start()
    {
        currentTime = totalTime;
    }

    void Update()
    {
        if (currentTime > 0 && !paused)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerText();
        }
        else if (currentTime < 0)
        {
            currentTime = 0;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            AddTime(1);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            PauseTimer();
        }
    }

    void UpdateTimerText()
    {
        tens.sprite = numSprites[Mathf.FloorToInt(currentTime / 10)];
        ones.sprite = numSprites[Mathf.FloorToInt(currentTime % 10)];
        tenths.sprite = numSprites[Mathf.FloorToInt(currentTime * 10 % 10)];
    }


    //Functions to add and remove time

    public void RemoveTime(float timeRemoved)
    {
        if (currentTime > timeRemoved)
        {
            currentTime -= timeRemoved;
        }
        else
        {
            currentTime = 0;
        }
        UpdateTimerText();
    }

    public void AddTime(float timeAdded)
    {
        if (currentTime + timeAdded < totalTime)
        {
            currentTime += timeAdded;
        }
        else
        {
            currentTime = totalTime;
        }
        UpdateTimerText();
    }

    //Pause function, using paused boolean

    public void PauseTimer()
    {
        if (paused)
        {
            bulbs.sprite = bulbSprites[1];
            paused = false;
        }
        else
        {
            bulbs.sprite = bulbSprites[0];
            paused = true;
        }
    }

}
