using System;
using NinevaStudios.Places.Internal;
using UnityEngine;

public class PermissionRequestCallbackProxy : AndroidJavaProxy
{
    readonly Action<string, bool> _onPermissionRequested;
    
    public PermissionRequestCallbackProxy(Action<string, bool> onPermissionRequested) : base(
        C.PermissionRequestCallbackProxy)
    {
        _onPermissionRequested = onPermissionRequested;
    }
    
    void onPermissionRequestCompleted(string permission, bool granted)
    {
        PlacesSceneHelper.Queue(() =>
        {
            _onPermissionRequested?.Invoke(permission, granted);
        });
    }
}
