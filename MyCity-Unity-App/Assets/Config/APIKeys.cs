using UnityEngine;

namespace MyCity.Config
{
    /// <summary>
    /// Secure API Keys Configuration
    /// Replace placeholder values with actual API keys from environment variables or secure storage
    /// </summary>
    public static class APIKeys
    {
        // Google Services
        public static string FirebaseAPIKey => GetSecureKey("FIREBASE_API_KEY", "YOUR_FIREBASE_API_KEY_HERE");
        public static string GoogleMapsAPIKey => GetSecureKey("GOOGLE_MAPS_API_KEY", "YOUR_GOOGLE_MAPS_API_KEY_HERE");
        public static string GooglePlacesAPIKey => GetSecureKey("GOOGLE_PLACES_API_KEY", "YOUR_GOOGLE_PLACES_API_KEY_HERE");
        
        // Third Party Services
        public static string SketchfabToken => GetSecureKey("SKETCHFAB_TOKEN", "YOUR_SKETCHFAB_TOKEN_HERE");
        public static string ThunderforestAPIKey => GetSecureKey("THUNDERFOREST_API_KEY", "YOUR_THUNDERFOREST_API_KEY_HERE");
        public static string TiandituAPIKey => GetSecureKey("TIANDITU_API_KEY", "YOUR_TIANDITU_API_KEY_HERE");
        
        /// <summary>
        /// Get API key from environment variable or use placeholder
        /// </summary>
        private static string GetSecureKey(string envVarName, string placeholder)
        {
            // Try to get from environment variable first
            string envValue = System.Environment.GetEnvironmentVariable(envVarName);
            if (!string.IsNullOrEmpty(envValue))
            {
                return envValue;
            }
            
            // Fallback to placeholder for development
            Debug.LogWarning($"API Key not found in environment variable: {envVarName}. Using placeholder.");
            return placeholder;
        }
        
        /// <summary>
        /// Validate that all required API keys are properly configured
        /// </summary>
        public static bool ValidateAPIKeys()
        {
            bool isValid = true;
            
            if (FirebaseAPIKey.Contains("YOUR_") || FirebaseAPIKey.Contains("placeholder"))
            {
                Debug.LogError("Firebase API Key not configured!");
                isValid = false;
            }
            
            if (GoogleMapsAPIKey.Contains("YOUR_") || GoogleMapsAPIKey.Contains("placeholder"))
            {
                Debug.LogError("Google Maps API Key not configured!");
                isValid = false;
            }
            
            if (GooglePlacesAPIKey.Contains("YOUR_") || GooglePlacesAPIKey.Contains("placeholder"))
            {
                Debug.LogError("Google Places API Key not configured!");
                isValid = false;
            }
            
            return isValid;
        }
    }
} 