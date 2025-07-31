using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachText : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private GameObject Distance;

    private Transform _transform;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Distance.transform.position.x, Distance.transform.position.y + 1.5f,
            Distance.transform.position.z + 3f);
    }
}