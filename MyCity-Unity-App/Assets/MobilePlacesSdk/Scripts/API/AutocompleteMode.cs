using JetBrains.Annotations;

namespace NinevaStudios.Places
{
	/// <summary>
	/// Launch mode for the Android activity: either fullscreen or overlay.
	/// </summary>
	[PublicAPI]
	public enum AutocompleteMode
	{
		/// <summary>
		/// Displayed fullscreen
		/// </summary>
		Fullscreen = 0,

		/// <summary>
		/// Displayed as overlay
		/// </summary>
		Overlay = 1
	}
}