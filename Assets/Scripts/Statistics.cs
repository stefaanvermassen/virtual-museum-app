using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// A singleton to track our statistics using google analytics
/// </summary>
public class Statistics
{
    private static Statistics statistics;
    private GoogleAnalyticsV3 googleAnalytics;
    private string lastScreen;

    private const string TIMING_CATEGORY = "SCREEN";
    private const string BUTTON_CATEGORY = "BUTTON";
    private const string BUTTON_EVENT = "BUTTON_CLICKED";

    /// <summary>
    /// Initialises the singleton
    /// </summary>
    private Statistics()
    { 
        var ob = new GameObject();
        googleAnalytics = ob.AddComponent<GoogleAnalyticsV3>();

        lastScreen = "";
    }

    /// <summary>
    /// Returns an instance of the statistics, lazy loading
    /// </summary>
    /// <returns>The Statistics instance</returns>
    public static Statistics getInstance()
    {
        if (statistics == null)
        {
            statistics = new Statistics();
        }
        return statistics;
    }

    /// <summary>
    /// Starts a google analytics session
    /// </summary>
    private void StartSession()
    {
        if (googleAnalytics != null)
        {
            googleAnalytics.StartSession();
        }
    }

    /// <summary>
    /// Stops the current google analytics session
    /// </summary>
    private void StopSession()
    {
        if (googleAnalytics != null)
        {
            googleAnalytics.StopSession();
        }
    }

    /// <summary>
    /// Register a screen to google analytics
    /// </summary>
    /// <param name="screenName">The name of the screen</param>
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

        if (googleAnalytics != null)
        {
            googleAnalytics.LogScreen(screenName);
        }
    }

    /// <summary>
    /// Build names that contains the screenname if set
    /// </summary>
    /// <param name="name">The name to build</param>
    /// <returns>The built name</returns>
    private string buildName(string name)
    {
        return String.IsNullOrEmpty(lastScreen) ? name : lastScreen + "." + name;
    }

    /// <summary>
    /// Build the exception name that contains the screenname if set
    /// </summary>
    /// <param name="exceptionDescription">The exception description</param>
    /// <returns>The build exception name</returns>
    private string buildException(string exceptionDescription)
    {
        return String.IsNullOrEmpty(lastScreen) ? exceptionDescription : lastScreen + ": " + exceptionDescription;
    }

    /// <summary>
    /// Log an exception to google analytics
    /// </summary>
    /// <param name="description">The description of the exception</param>
    public void LogException(string description)
    {
        if (googleAnalytics != null)
        {
            googleAnalytics.LogException(new ExceptionHitBuilder()
                .SetExceptionDescription(buildException(description)));
        }
    }

    /// <summary>
    /// Register a button click
    /// </summary>
    /// <param name="buttonName">The name of the clicked button</param>
    public void ButtonClicked(string buttonName)
    {
        if (googleAnalytics != null)
        {
            googleAnalytics.LogEvent(new EventHitBuilder()
                .SetEventCategory(BUTTON_CATEGORY)
                .SetEventAction(BUTTON_EVENT + ": " + buildName(buttonName)));
        }
    }

    /// <summary>
    /// Unregister the screen and stop the session
    /// </summary>
    public void RemoveScreen()
    {
        lastScreen = "";
        StopSession();
    }
}
