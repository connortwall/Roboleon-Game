using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FireSpawner : MonoBehaviour
{
    
    [Header("Fire Prefabs to use")]
    public List<GameObject> firePrefabs;
    [Header("Tuning")]
    public Vector2 zone = Vector2.one;
    public float spaceBetweenFires = 1;
    public float prefabScale = 1;

    private int k = 2;
    private List<Vector2> samples;
    [HideInInspector]
    //public bool active = false; //use for saving later   

    void Update(){
        if (Input.GetKeyDown(KeyCode.F)){ //print task list
            StartFire();
        }
    }

    public List<GameObject> StartFire()
    {
        List<GameObject> fireObjects = new List<GameObject>();
        samples = Poisson.GeneratePoint(spaceBetweenFires, zone, k);
        if(samples != null)
        {   
            int index;
            foreach(Vector2 sample in samples)
            {
                
                //instantiate fire
                index = Random.Range(0, firePrefabs.Count);
                GameObject fire = Instantiate(firePrefabs[index], new Vector3(sample.x, 0, sample.y)+transform.position, Quaternion.identity)as GameObject;
                Assert.IsNotNull(fire.GetComponent<FireObject>());
                fire.transform.Rotate(0, Random.Range(0, 360), 0);
                fire.transform.localScale = Vector3.one * prefabScale;
                //add fire to fire list 
                fireObjects.Add(fire);
            }
        }
        // if by chance no fires are added to the scene in each sample (i.e count = 0) spawn a fire in the middle of the hub
        if (fireObjects.Count <= 0)
        {
            // instantiate fire in middle of fire spawner zone
            GameObject fire = Instantiate(firePrefabs[0], new Vector3(zone.x/2, 0, zone.y/2)+transform.position, Quaternion.identity)as GameObject;
            fireObjects.Add(fire);
        }
        
        if (fireObjects.Count > 0) return fireObjects;
        else return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((new Vector3(zone.x, 0, zone.y) / 2)+transform.position, new Vector3(zone.x, 0, zone.y));
        
    }

}