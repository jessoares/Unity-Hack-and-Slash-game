using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterList : MonoBehaviour
{
    public GameObject[] characterList;
    public int index;
    private void Start()
    {
        index = PlayerPrefs.GetInt("Character Selected");
        characterList = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            characterList[i] = transform.GetChild(i).gameObject;
        }
        foreach (GameObject go in characterList)
        {
            go.SetActive(false);
            if(characterList[index])
            {
                characterList[index].SetActive(true);
            }
        }
    }
   
    







}