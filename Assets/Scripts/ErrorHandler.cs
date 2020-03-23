using System;
using UnityEngine;

public class ErrorHandler
{
    #region Constructors
    private ErrorHandler() { }
    #endregion

    #region Methods
    public static void LogError (string message)
    {
        LogError(message, null);
    }

    // TODO: Finish implementation
    public static void LogError(string message, Exception exception)
    {
        Debug.LogError(message);

        Application.Quit();

        throw new Exception(message);
    }
    #endregion
}
