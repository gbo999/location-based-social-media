# 🔐 Security Guide - MyCity Project

## 🚨 CRITICAL: API Keys Security

This project contains sensitive API keys that must be secured immediately.

### ⚠️ Exposed API Keys Found:
- **Firebase API Key**: `AIzaSyAVBo5VahaacXCfr3eYz3kmmNPZyTNgyU4`
- **Google Maps API Key**: `AIzaSyDS1WYkuiRwqHKIODt1HHQ-xLazH2SRudQ`
- **Google Places API Keys**: Multiple instances
- **Sketchfab Token**: `0b7b08026bd74b599cacb269280158e9`
- **Third-party API Keys**: Thunderforest, Tianditu

### 🛡️ IMMEDIATE ACTIONS REQUIRED:

#### 1. **Revoke Exposed API Keys**
- Go to Google Cloud Console and revoke all exposed API keys
- Go to Sketchfab and revoke the exposed token
- Revoke any other third-party API keys

#### 2. **Generate New API Keys**
- Create new API keys for all services
- Set up proper restrictions (domain, IP, usage limits)
- Enable billing alerts and usage monitoring

#### 3. **Secure Configuration Setup**
- Copy `env.template` to `.env`
- Add your new API keys to the `.env` file
- Never commit `.env` file to git

#### 4. **Environment Variables**
```bash
# Set environment variables (example for macOS/Linux)
export FIREBASE_API_KEY="your_new_firebase_key"
export GOOGLE_MAPS_API_KEY="your_new_maps_key"
export GOOGLE_PLACES_API_KEY="your_new_places_key"
export SKETCHFAB_TOKEN="your_new_sketchfab_token"
```

#### 5. **Update Configuration Files**
- Replace `google-services.json` with new configuration
- Update any hardcoded API keys in the codebase
- Use the new `APIKeys.cs` configuration class

### 📁 Files to Secure:
- `Assets/google-services.json` - Firebase configuration
- `Assets/StreamingAssets/google-services-desktop.json` - Desktop Firebase config
- `Assets/Plugins/Android/FirebaseApp.androidlib/res/values/google-services.xml` - Android config
- Any files with hardcoded API keys

### 🔒 Best Practices:
1. **Never commit API keys** to version control
2. **Use environment variables** for sensitive data
3. **Set up API key restrictions** (domain, IP, usage limits)
4. **Monitor API usage** and set up alerts
5. **Regular security audits** of the codebase
6. **Use placeholder values** in code examples

### 🚨 Emergency Contacts:
- **Google Cloud Support**: For API key issues
- **Sketchfab Support**: For token issues
- **Firebase Support**: For Firebase configuration issues

### 📋 Security Checklist:
- [ ] Revoke all exposed API keys
- [ ] Generate new API keys with restrictions
- [ ] Set up environment variables
- [ ] Update configuration files
- [ ] Test application with new keys
- [ ] Set up usage monitoring and alerts
- [ ] Document security procedures
- [ ] Train team on security practices

**⚠️ These API keys are now compromised and should be considered public. Immediate action is required to secure your services.** 