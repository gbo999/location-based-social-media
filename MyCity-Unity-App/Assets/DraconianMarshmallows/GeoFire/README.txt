## Quick Start ##

To start off - open the test scene so you can see GeoFire storing and retrieving locations via longitude and latitude. 
This scene is located at `Assets/DraconianMarshmallows/GeoFire/example/SaveAndRetrieveLocation.unity` in your project files. 

**Please Note:**
Class and method names have not been changed while porting to C# so the following Java reference should apply to this 
version: https://geofire-java.firebaseapp.com/docs

### Setting Up the Example Scene with your Firebase Database ###

- Install the Firebase DB package for Unity for your project if you have not already. 
Instructions for installing Firebase's Unity SDKs can be found here: https://firebase.google.com/docs/unity/setup 
- You'll have to setup a location in your DB that's accessible in Unity editor. This guide: https://firebase.google.com/docs/database/unity/start should instruct you how to do so. 
- You'll either have to setup the DB location as public or follow directions under `Optional. Editor Setup for restricted access.`
to allow Firebase DB access in the editor. 
- In the example-scene, select the game-object named `LocationController`. 
- You'll see a script-component with a "Firebase Db Url" field in the Inspector. 
- Set this field to the Firebase DB location that's accessible in the editor. 
- Hit the Play button. 

You should now see the two test locations (`enemy_1`, `enemy_2`) stored and retrieved in the Console window. 
