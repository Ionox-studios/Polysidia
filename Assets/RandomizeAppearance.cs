using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class RandomizeAppearance : MonoBehaviour
{
    public GameObject[] cyberParts;
    public GameObject[] clothesParts;
    public GameObject[] hairParts;
    public GameObject[] scarParts;
    
    public float chanceToHaveElement = 0.5f;

    void Start()
    {

    }

    void RandomizePart(GameObject[] partsArray,bool alwaysInclude = false)
    {
        foreach (GameObject part in partsArray)
            {
                part.SetActive(false);
            }
        // Randomly choose to include or not include a part
        if (alwaysInclude || Random.value > chanceToHaveElement)
        {
            int randomIndex = Random.Range(0, partsArray.Length);
            partsArray[randomIndex].SetActive(true);
        }
        else if (Random.value > chanceToHaveElement)
        {
            int randomIndex = Random.Range(0, partsArray.Length);
            partsArray[randomIndex].SetActive(true);
        }


    }
    public void Randomize()
    {
    RandomizePart(cyberParts);
    RandomizePart(clothesParts, true);
    RandomizePart(hairParts);
    RandomizePart(scarParts);
    }

}

