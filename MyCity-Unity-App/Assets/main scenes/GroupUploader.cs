using SocialApp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GroupUploader : MonoBehaviour
{






    [SerializeField]
    private InputField GroupNameInput;

    [SerializeField]
    private InputField GroupDescription;

    public GameObject contentPanel;

    public GameObject salesPanel;

    public Toggle contentToggle;
  
    public Toggle salesToggle;




    private string GroupType;


    private void Start()
    {


        

        contentToggle.onValueChanged.AddListener(delegate {
            onValueChangedContent(contentToggle);
        });



        salesToggle.onValueChanged.AddListener(delegate {
            salesToggleValueChanged(salesToggle);
        });






    }

    private void onValueChangedContent(Toggle toggle)
    {
        if (toggle.isOn)
        {
            salesPanel.SetActive(false);
            contentPanel.SetActive(true);

        }

        else
        {
            salesPanel.SetActive(true);
            contentPanel.SetActive(false);


        }



    }

    public void salesToggleValueChanged(Toggle sales)
    {

        if (sales.isOn)
        {
            salesPanel.SetActive(true);
            contentPanel.SetActive(false);

        }

        else
        {
            salesPanel.SetActive(false);
            contentPanel.SetActive(true);


        }



    }








    private void DropdownSelected(Dropdown dropdown)
    {

        int index = dropdown.value;
        GroupType = dropdown.options[index].text;
        Debug.Log("group type is: " + GroupType);
    }

    public void onGroupDataAdded(AddGroupMessage _msg)
    {
        if (_msg.IsSuccess)
        {
          
            AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.AddGroupSuccess, CloseWindow);
            //    AppManager.FIREBASE_CONTROLLER.SendVerifcationEmail();
           // AppManager.myCityController.groupID = _msg.groupID;
            AppManager.VIEW_CONTROLLER.HideLoading();
        }
        else
        {
            PopupMessage msg = new PopupMessage();
            msg.Title = "Error";
         
            AppManager.VIEW_CONTROLLER.ShowPopupMessage(msg);
            AppManager.VIEW_CONTROLLER.HideLoading();
        }
    }

    public void CloseWindow()
    {
        AppManager.VIEW_CONTROLLER.HideGroupCreate();
        AppManager.VIEW_CONTROLLER.ShowChooseGroupPicture();
    }

    public void sendCreateGroup()
    {
        if (CheckError())
            return;

        Group group = new Group();

        group.groupName = GroupNameInput.text.Trim();
        group.description = GroupDescription.text.Trim();
        group.ownerID = AppManager.Instance.auth.CurrentUser.UserId;
        group.groupType = GroupType;
        group.tag = AppManager.myCityController.currentTag;


        AppManager.FIREBASE_CONTROLLER.AddNewGroup(group, onGroupDataAdded);
        AppManager.VIEW_CONTROLLER.ShowLoading();
    }




    private bool CheckError()
    {
        bool IsError = false;
        if (string.IsNullOrEmpty(GroupNameInput.text))
        {
            AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.EmptyEmail);
            IsError = true;
        }
        if (string.IsNullOrEmpty(GroupDescription.text))
        {
            AppManager.VIEW_CONTROLLER.ShowPopupMSG(MessageCode.EmptyFirstName);
            IsError = true;
        }
     return IsError;
    }


    public void chooseTagFood()
    {

        AppManager.myCityController.currentTag = AppSettings.Foodtag;


    }


    public void chooseTagArt()
    {

        AppManager.myCityController.currentTag = AppSettings.arttag;


    }


    public void chooseTagRel()
    {

        AppManager.myCityController.currentTag = AppSettings.religiontag;


    }
    public void chooseTagSport()
    {

        AppManager.myCityController.currentTag = AppSettings.sporttag;


    }

    



    public void chooseTagObj()
    {

        AppManager.myCityController.currentTag = AppSettings.objtag;


    }



    public void chooseTagParty()
    {

        AppManager.myCityController.currentTag = AppSettings.partytag;


    }

    public void chooseTagGame()
    {

        AppManager.myCityController.currentTag = AppSettings.gametag;


    }



    public void chooseTagNet()
    {

        AppManager.myCityController.currentTag = AppSettings.networkingtag;



    }


    public void chooseTaghome()
    {

        AppManager.myCityController.currentTag = AppSettings.hometag;


    }

    public void chooseTagcars()
    {

        AppManager.myCityController.currentTag = AppSettings.carSales;


    }

    public void chooseTagelec()
    {

        AppManager.myCityController.currentTag = AppSettings.elecsales;


    }

    public void chooseTagKitch()
    {

        AppManager.myCityController.currentTag = AppSettings.kitchSales;


    }

    public void chooseTagGard()
    {

        AppManager.myCityController.currentTag = AppSettings.gardSales;


    }


    public void chooseTagHob()
    {

        AppManager.myCityController.currentTag = AppSettings.hobSales;


    }



    public void chooseTagKids()
    {

        AppManager.myCityController.currentTag = AppSettings.kidsSales;


    }






    public void chooseTagComp()
    {

        AppManager.myCityController.currentTag = AppSettings.compSales;


    }

    public void chooseTagReal()
    {

        AppManager.myCityController.currentTag = AppSettings.realSales;


    }


    public void chooseTagent()
    {

        AppManager.myCityController.currentTag = AppSettings.entlSales;


    }


    public void chooseTagClothing()
    {

        AppManager.myCityController.currentTag = AppSettings.clothingSales;


    }


    public void chooseTagother()
    {

        AppManager.myCityController.currentTag = AppSettings.othertag;


    }




}












[System.Serializable]
    public class Group
    {
    public string groupID;
    public string ownerID;
    public string groupName;
    public string description;
    public string groupType;
    public string tag;

    }








    
