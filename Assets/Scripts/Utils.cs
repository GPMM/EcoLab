using System;
using System.Linq;

public class Utils
{
    #region Constructors
    private Utils() { }
    #endregion

    #region Methods
    /// <summary>
    /// Sorts the received array in a random order
    /// </summary>
    /// <typeparam name="T">The array type</typeparam>
    /// <param name="array">The array to be sorted</param>
    /// <returns>The sorted array</returns>
    public static T[] RandomizeArray<T>(T[] array)
    {
        Random rnd = new Random();
        T[] randomOrder = array.OrderBy(x => rnd.Next()).ToArray();

        return randomOrder;
    }
    #endregion
}
