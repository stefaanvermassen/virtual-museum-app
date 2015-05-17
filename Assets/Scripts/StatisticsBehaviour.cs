using UnityEngine;
using System.Collections;

public class StatisticsBehaviour : MonoBehaviour {

    protected Statistics statistics;
    protected string screenName;

    protected void StartStatistics()
    {
        // Register screen
        statistics = Statistics.getInstance();
        statistics.RegisterScreen(screenName);
    }

    protected void StartStatistics(string screenName)
    {
        this.screenName = screenName;
        StartStatistics();
    }

    protected void ClosingButton(string buttonName)
    {
        if (string.IsNullOrEmpty(screenName))
        {
            Debug.LogWarning("You are trying to log a " + buttonName + " button click, but you have not yet set your screenName.\nAre you sure you called StartStatistics?");
            return;
        }

        // Report button clicked
        statistics.ButtonClicked("Save");
        End();
    }

    protected void End()
    {
        if (string.IsNullOrEmpty(screenName))
        {
            Debug.LogWarning("You are trying to log the end of screen, but you have not yet set your screenName.\nAre you sure you called StartStatistics?");
            return;
        }
        statistics.RemoveScreen();
    }
}
