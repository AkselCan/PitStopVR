using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    private float currentTime = 0f;
    private bool timerIsRunning = false; 

    void Update()
    {
        if (timerIsRunning)
        {
            currentTime += Time.deltaTime; 
            DisplayTime(currentTime);
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        // Calculate total seconds (integer part)
        float seconds = Mathf.FloorToInt(timeToDisplay);
        
        // Calculate milliseconds (the fractional part of the time)
        // We get the remainder of the total time divided by 1, then multiply by 1000 
        // and divide by 10 to get the centiseconds (0-99).
        float milliseconds = (timeToDisplay * 100) % 100;

        // *** KEY MODIFICATION HERE ***
        // Format the time as "Time: SS.ms" (e.g., Time: 15.42)
        timerText.text = string.Format("Time: {0:00}.{1:00}", seconds, milliseconds);
    }

    // --- NEW PUBLIC METHODS ---

    /// <summary>
    /// Starts the countdown/count-up timer.
    /// </summary>
    public void StartTimer()
    {
        currentTime = 0f; // Reset time to 0 before starting
        timerIsRunning = true;
        Debug.Log("Timer Started.");
    }

    /// <summary>
    /// Stops the timer.
    /// </summary>
    public void StopTimer()
    {
        timerIsRunning = false;
        Debug.Log("Timer Stopped.");
    }
}