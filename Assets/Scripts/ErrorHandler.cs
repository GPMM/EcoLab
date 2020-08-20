using System;
using UnityEngine;

public class ErrorHandler
{
    #region Constructors
    private ErrorHandler() { }
    #endregion

    #region Methods
    public static void AssertNull(object obj, string message = null)
    {
        if (obj is null)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                throw new Exception(message);
            }
            else
            {
                throw new ArgumentNullException(obj.GetType().Name);
            }
        }
    }
    public static void AssertExists(object obj, string message = null)
    {
        if (!(obj is null))
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                throw new Exception(message);
            }
            else
            {
                throw new ArgumentNullException(obj.GetType().Name);
            }
        }
    }

    public static void LogError (string message)
    {
        LogError(message, null);
    }

    // TODO: Finish implementation
    public static void LogError(string message, Exception exception)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            Debug.LogError(message);
        }

        if (!(exception is null))
        {
            Debug.LogError(exception);
        }

        Application.Quit();

        throw new Exception(message);
    }
    #endregion
}
