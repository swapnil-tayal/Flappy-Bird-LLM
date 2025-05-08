using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class MiddleScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public BirdScript bird;
    public static List<GameObject> middlePart = new List<GameObject>();
    void Start()
    {
        bird = GameObject.FindGameObjectWithTag("Bird").GetComponent<BirdScript>();
        middlePart.Add(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collosion)
    {
        //Debug.Log(transform.position.x);
        //Debug.Log(transform.position.y);
        if (collosion.gameObject.layer == 3)
        {
            bird.addScore(1);
        }
    }
}
