/*           INFINITY CODE           */
/*     https://infinity-code.com     */

namespace InfinityCode.uPano.Controls
{
    /// <summary>
    /// The base class for the components of moving a panorama that have a sensitivity
    /// </summary>
    public abstract class SensitivityControl : PanoControl
    {
        /// <summary>
        /// Enum of panorama moving axes
        /// </summary>
        public enum Axes { PanTilt = 0, Pan = 1, Tilt = 2 }

        /// <summary>
        /// Panorama moving axes
        /// </summary>
        public Axes axes = Axes.PanTilt;

        /// <summary>
        /// Speed of change pan
        /// </summary>
        public float sensitivityPan = 100;

        /// <summary>
        /// Speed of change tilt
        /// </summary>
        public float sensitivityTilt = 100;

        /// <summary>
        /// Speed of change fov
        /// </summary>
        public float sensitivityFov = 100;
    }
}