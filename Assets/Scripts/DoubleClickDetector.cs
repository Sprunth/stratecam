using UnityEngine;

public class DoubleClickDetector
{
    private int numberOfClicks;
    private float timer;

    public bool IsDoubleClick()
    {
        var isDoubleClick = numberOfClicks == 2;
        if (isDoubleClick)
            numberOfClicks = 0;
        return isDoubleClick;
    }

    public void Update()
    {
        timer += Time.deltaTime;

        if (timer > 0.3f)
        {
            numberOfClicks = 0;
        }

        if (Input.GetMouseButtonDown(0))
        {
            numberOfClicks++;
            timer = 0.0f;
        }
    }
}