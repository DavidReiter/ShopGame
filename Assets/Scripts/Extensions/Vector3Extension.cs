using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extension
{
    public static Vector3 With(this Vector3 orginial, float ? x = null, float ? y = null, float ? z = null)
    {
        return new Vector3(x ?? orginial.x, y ?? orginial.y, z ?? orginial.z);
    }

    /// <summary>
    /// Keep position but change y to 0
    /// </summary>
    /// <param name="original"></param>
    /// <returns></returns>
    public static Vector3 Flat(this Vector3 original)
    {
        return new Vector3(original.x, 0f, original.z);
    }

    /// <summary>
    /// Shorthand for returning the normalized direction
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    public static Vector3 DirectionTo(this Vector3 source, Vector3 destination)
    {
        return Vector3.Normalize(destination - source);
    }
}
