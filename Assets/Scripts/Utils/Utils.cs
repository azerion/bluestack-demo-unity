using System;

public static class Utils
{
    /// <summary>
    /// Converts Enum Value to different Enum Value (by Value Name)
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum to convert to.</typeparam>
    /// <param name="source">The source enum to convert from.</param>
    /// <returns>Converted Enum Value</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static TEnum ConvertTo<TEnum>(this Enum source)
    {
        if (!typeof(TEnum).IsEnum)
            throw new ArgumentException("Destination type is not enum");
        try
        {
            return (TEnum)Enum.Parse(typeof(TEnum), source.ToString(), ignoreCase: true);
        }
        catch (ArgumentException aex)
        {
            throw new InvalidOperationException
            (
                $"Could not convert {source.GetType().ToString()} [{source.ToString()}] to {typeof(TEnum).ToString()}",
                aex
            );
        }
    }

    /// <summary>
    /// Gets next Enum Value from the list
    /// </summary>
    /// <returns>Next Enum Value</returns>
    /// <exception cref="ArgumentException"></exception>
    public static T Next<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum)
            throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

        T[] Arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(Arr, src) + 1;
        return (Arr.Length == j) ? Arr[0] : Arr[j];
    }
}