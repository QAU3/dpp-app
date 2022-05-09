using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskAssets : MonoBehaviour
{
    [Header("Character recognition")]
    public string[] realTaskStrings;
    public string[] practiceTaskStrings;
    public int[] fontSizesList;
    public string alphabet;

    [Header("Image recognition")]
    public Texture2D[] setPractice;
    public Texture2D[] setOne, setTwo,setThree,setFour; 

}
