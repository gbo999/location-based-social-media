using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPopupController : MonoBehaviour
{

    [SerializeField] private UIElementsGroup Profile;
    
    // Start is called before the first frame update


    public void ShowProfile()
    {
        
        Profile.ChangeVisibility(true);
        

    }
    
    public void CloseProfile()
    {
        
        Profile.ChangeVisibility(false);
        

    }

    
    
    
    
    
    
    
    
    
    
    
    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
