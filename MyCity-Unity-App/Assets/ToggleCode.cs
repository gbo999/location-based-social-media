using SocialApp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleCode : MonoBehaviour
{

    public GameObject contentPanel;

    public GameObject salesPanel;

    public Toggle contentToggle;

    public Toggle salesToggle;


    // Start is called before the first frame update
    void Start()
    {
       /* contentToggle.onValueChanged.AddListener(delegate {
            onValueChangedContent(contentToggle);
        });



        salesToggle.onValueChanged.AddListener(delegate {
            salesToggleValueChanged(salesToggle);
        });
*/

    }




    public void chooseTagMusic()
    {

        Debug.Log("music is pressed");

        AppManager.myCityController.currentTag = AppSettings.MusicTag;


    }








    //****************************************************************************************///






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


    // Update is called once per frame
  
}
