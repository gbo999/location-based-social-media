using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadiusController : MonoBehaviour
{
    // Start is called before the first frame update

    public Slider radiusSlider;

    private double lngi;
    private double lati;



    public float radiusKM = 1f;




    /// <summary>
    /// Number of segments
    /// </summary>
    public int segments = 32;

    /// <summary>
    /// This method is called when a user clicks on a map
    /// </summary>
    private void OnMapClick()
    {
        // Get the coordinates under cursor
      
        OnlineMapsControlBase.instance.GetCoords(out lngi, out lati);

        // Create a new marker under cursor
     //   OnlineMapsMarkerManager.CreateItem(lng, lat, "Marker " + OnlineMapsMarkerManager.CountItems);

        OnlineMaps map = OnlineMaps.instance;

        // Get the coordinate at the desired distance
        double nlng, nlat;
        OnlineMapsUtils.GetCoordinateInDistance(lngi, lati, radiusKM, 90, out nlng, out nlat);

        
        

        double tx1, ty1, tx2, ty2;

        // Convert the coordinate under cursor to tile position
        map.projection.CoordinatesToTile(lngi, lati, 20, out tx1, out ty1);

        // Convert remote coordinate to tile position
        map.projection.CoordinatesToTile(nlng, nlat, 20, out tx2, out ty2);

        // Calculate radius in tiles
        double r = tx2 - tx1;

        // Create a new array for points
        OnlineMapsVector2d[] points = new OnlineMapsVector2d[segments];

        // Calculate a step
        double step = 360d / segments;
        double lng, lat;
        // Calculate each point of circle
        for (int i = 0; i < segments; i++)
        {
            double px = tx1 + Math.Cos(step * i * OnlineMapsUtils.Deg2Rad) * r;
            double py = ty1 + Math.Sin(step * i * OnlineMapsUtils.Deg2Rad) * r;
            map.projection.TileToCoordinates(px, py, 20, out lng, out lat);
            points[i] = new OnlineMapsVector2d(lng, lat);
        }

        OnlineMapsDrawingElementManager.RemoveAllItems();

        // Create a new polygon to draw a circle
        OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingPoly(points, Color.red, 3));

        OnlineMapsControlBase.instance.OnMapClick -= OnMapClick;

        OnlineMapsControlBase.instance.OnMapClick += getRadius;

    }



    public void RadiusChange(float value)
    {

        OnlineMaps map = OnlineMaps.instance;


        Debug.Log(value);


        // Get the coordinate at the desired distance
        double nlng, nlat;
        OnlineMapsUtils.GetCoordinateInDistance(lngi, lati, value, 90, out nlng, out nlat);

        double tx1, ty1, tx2, ty2;

        // Convert the coordinate under cursor to tile position
        map.projection.CoordinatesToTile(lngi, lati, 20, out tx1, out ty1);

        // Convert remote coordinate to tile position
        map.projection.CoordinatesToTile(nlng, nlat, 20, out tx2, out ty2);

        // Calculate radius in tiles
        double r = tx2 - tx1;

        // Create a new array for points
        OnlineMapsVector2d[] points = new OnlineMapsVector2d[segments];

        // Calculate a step
        double step = 360d / segments;
        double lng, lat;
        // Calculate each point of circle
        for (int i = 0; i < segments; i++)
        {
            double px = tx1 + Math.Cos(step * i * OnlineMapsUtils.Deg2Rad) * r;
            double py = ty1 + Math.Sin(step * i * OnlineMapsUtils.Deg2Rad) * r;
            map.projection.TileToCoordinates(px, py, 20, out lng, out lat);
            points[i] = new OnlineMapsVector2d(lng, lat);
        }

        OnlineMapsDrawingElementManager.RemoveAllItems();

        // Create a new polygon to draw a circle
        OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingPoly(points, Color.red, 3));

    }



    void Start()
    {
        OnlineMapsControlBase.instance.OnMapClick += OnMapClick;
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void getRadius()
    {
        double lngTOcheck, latToCheck;

        OnlineMapsControlBase.instance.GetCoords(out lngTOcheck, out latToCheck);

        float lngf = (float)lngi;
        float latf = (float)lati;
        float lngToCheckf = (float)lngTOcheck;
        float latToCheckf = (float)latToCheck;

        Debug.Log("distance in km "+ OnlineMapsUtils.DistanceBetweenPointsD(new Vector2 (lngf,latf), new Vector2(lngToCheckf, latToCheckf)));


    }



}
