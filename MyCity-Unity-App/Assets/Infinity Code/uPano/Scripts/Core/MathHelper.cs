/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;
using UnityEngine;

namespace InfinityCode.uPano
{
    /// <summary>
    /// Mathematical methods
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Degrees-to-radians conversion constant.
        /// </summary>
        public const double Deg2Rad = Math.PI / 180;

        /// <summary>
        /// Radians-to-degrees conversion constant.
        /// </summary>
        public const double Rad2Deg = 180 / Math.PI;

        /// <summary>
        /// Math.PI / 4
        /// </summary>
        public const double PID4 = Math.PI / 4;

        /// <summary>
        /// The angle between the two points in degree
        /// </summary>
        /// <param name="point1">Point 1</param>
        /// <param name="point2">Point 2</param>
        /// <returns>Angle in degree</returns>
        public static float Angle2D(Vector3 point1, Vector3 point2)
        {
            return Mathf.Atan2(point2.z - point1.z, point2.x - point1.x) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// The angle between the two points in degree
        /// </summary>
        /// <param name="p1x">Point 1 X</param>
        /// <param name="p1y">Point 1 Y</param>
        /// <param name="p2x">Point 2 X</param>
        /// <param name="p2y">Point 2 Y</param>
        /// <returns>Angle in degree</returns>
        public static float Angle2D(float p1x, float p1y, float p2x, float p2y)
        {
            return Mathf.Atan2(p2y - p1y, p2x - p1x) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// The angle between the two points in degree
        /// </summary>
        /// <param name="p1x">Point 1 X</param>
        /// <param name="p1y">Point 1 Y</param>
        /// <param name="p2x">Point 2 X</param>
        /// <param name="p2y">Point 2 Y</param>
        /// <returns>Angle in degree</returns>
        public static double Angle2D(double p1x, double p1y, double p2x, double p2y)
        {
            return Math.Atan2(p2y - p1y, p2x - p1x) * Rad2Deg;
        }
    }
}
