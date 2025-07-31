using ChartAndGraph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharts : MonoBehaviour
{
    // Start is called before the first frame update


    public PieChart pie;

    public Material material;

    void Start()
    {
        pie.DataSource.AddCategory("taigu", material);
        pie.DataSource.SetValue("taigu", 5);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
