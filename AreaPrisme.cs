using System.Collections.Generic;
using UnityEngine;


public class AreaPrisme : MonoBehaviour
{

    public bool isTrainingMl = false;

    [Header("Collectibles Features")]
    public int nbOrbs = 1;
    [SerializeField] GameObject orb1 = null;
    [SerializeField] GameObject orb2 = null;
    [SerializeField] GameObject orb3 = null;
    [SerializeField] GameObject orb4 = null;
    [SerializeField] GameObject[] badPrisme = null;
    [SerializeField] int numBadPrisme = 3;
    [HideInInspector]
    public List<GameObject> goTemp;
    [Header("Area Range")]
    public float rangeX = 33f;
    public float rangeZ = 33f;

    public void Awake()
    {
        if (isTrainingMl)
        {
            Debug.Log("Train env Loaded");
        }
        else
        {
            goTemp.Add(orb1);
            goTemp.Add(orb2);
            goTemp.Add(orb3);
            goTemp.Add(orb4);
            Debug.Log("Inference env Loaded");
        }
    }

    void CreateOrbs(int num, GameObject type)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject go = Instantiate(type, new Vector3(Random.Range(-rangeX, rangeX), 0f,Random.Range(-rangeZ, rangeZ)) + transform.position,
                                        Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 0f)));
            goTemp.Add(go);
        }
    }

    void CreateBadPrisme(int num, GameObject[] type)
    {
        for (int i = 0; i < num; i++)
        {
            int randIndex = Random.Range(0, type.Length);
            GameObject go = Instantiate(type[randIndex], new Vector3(Random.Range(-rangeX, rangeX), 0f,Random.Range(-rangeZ, rangeZ)) + transform.position,
                                        Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 0f)));
            goTemp.Add(go);
        }
    }

    public void SpawmPrism()
    {
        goTemp = new List<GameObject>();
        CreateOrbs(nbOrbs, orb1);
    }

    public void SpawmAllPrism()
    {
        goTemp = new List<GameObject>();
        CreateOrbs(nbOrbs, orb1);
        CreateOrbs(nbOrbs, orb2);
        CreateOrbs(nbOrbs, orb3);
        CreateOrbs(nbOrbs, orb4);
    }

    public void SpawmBadPrisme()
    {
        CreateBadPrisme(numBadPrisme, badPrisme);
    }

    public void cleanAllCollectible()
    {
        foreach (GameObject go in goTemp)
        {
            try
            {
                OrbCollision thisOrb = go.GetComponent<OrbCollision>();
                thisOrb.isEndEpisodeDisable = true;
            }
            catch
            {
                
            }
            Destroy(go);
        }
    }
}

