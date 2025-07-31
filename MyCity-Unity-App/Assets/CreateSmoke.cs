using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CreateSmoke : MonoBehaviour
{
    // Start is called before the first frame update

    public SpriteRenderer sprite;

    public ParticleSystem particle;


    private void Start()
    {


        var emission = particle.emission; // Stores the module in a local variable
        emission.enabled = false; // Applies the new value directly to the Particle System


        sprite.enabled = false;
        
        StartCoroutine(delay());

    }

   

     //   sprite.gameObject.SetActive(false);

        







    


    public IEnumerator delay()
    {

        yield return new WaitForSeconds(0.2f);



        var emission = particle.emission; // Stores the module in a local variable
        emission.enabled = true;

        particle.Play();

        yield return new WaitForSeconds(0.2f);

        particle.Stop();

        sprite.enabled = true;





    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
