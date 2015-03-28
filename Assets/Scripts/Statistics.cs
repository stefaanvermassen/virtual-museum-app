using UnityEngine;
using System.Collections;
using System;

public class Statistics
{
    private static Statistics statistics;
    private GoogleAnalyticsV3 googleAnalytics;
    private string lastScreen;

    private const string TIMING_CATEGORY = "SCREEN";
    private const long TIMING_INTERVAL = 50L;
    private const string BUTTON_CATEGORY = "BUTTON";
    private const string BUTTON_EVENT = "BUTTON_CLICKED";

    private Statistics()
    { 
        var ob = new GameObject();
        googleAnalytics = ob.AddComponent<GoogleAnalyticsV3>();

        lastScreen = "";
    }

    public static Statistics getInstance()
    {
        if (statistics == null)
        {
            statistics = new Statistics();
        }
        return statistics;
    }

    private void StartSession()
    {
        googleAnalytics.StartSession();
    }

    private void StopSession()
    {
        googleAnalytics.StopSession();
    }

    public void RegisterScreen(string screenName)
    {
        // Is there a screen active
        if (!String.IsNullOrEmpty(lastScreen))
        {
            StopSession();
        }
        // Activate the new screen
        lastScreen = screenName;
        StartSession();

        googleAnalytics.LogScreen(screenName);
        googleAnalytics.LogTiming(new TimingHitBuilder()
            .SetTimingCategory(TIMING_CATEGORY)
            .SetTimingInterval(TIMING_INTERVAL)
            .SetTimingName(buildName(screenName)));
    }

    private string buildName(string name)
    {
        return String.IsNullOrEmpty(lastScreen) ? name : lastScreen + "." + name;
    }

    private string buildException(string ex)
    {
        return String.IsNullOrEmpty(lastScreen) ? ex : lastScreen + ": " + ex;
    }

    public void LogException(string description)
    {
        googleAnalytics.LogException(new ExceptionHitBuilder()
            .SetExceptionDescription(buildException(description)));
    }

    public void ButtonClicked(string buttonName)
    {
        googleAnalytics.LogEvent(new EventHitBuilder()
            .SetEventCategory(BUTTON_CATEGORY)
            .SetEventAction(BUTTON_EVENT)
            .SetEventLabel(buildName(buttonName)));
    }

    public void RemoveScreen()
    {
        lastScreen = "";
        StopSession();
    }
}
