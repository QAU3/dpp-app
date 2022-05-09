using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class TaskControl : MonoBehaviour
{
    public string[] studyStrings = { "HVZDSNCKOR", "RDNVHCKOSZ", "KRCNODZSHV" };
    public int[] fontSizes;
    public Button[] buttonList;
    private List<Pairs> stringsCollection, randomizedCollection, errorCollection;
    private int currIndex;
    private string errorString;


    [Header("String monitor")]
    public TMP_Text prevString;
    public TMP_Text currString;
    public TMP_Text nextString;
    public TMP_Text indexMonitor;
    public Button nextBtn, prevBtn;

    [Header("Task configuration")]
    public string taskType;

    public CSVLogger csvLogger, csvContinue;
    // Start is called before the first frame update
    void Start()
    {
        //Init collections
        stringsCollection = new List<Pairs>();
        randomizedCollection = new List<Pairs>();
        errorCollection = new List<Pairs>();

        CreateCollection(fontSizes, studyStrings);
        //Randomize
        Shuffle<Pairs>(randomizedCollection);
        //Saving original randomized collection
        string[] columns = new string[] { "string", "font_size" };
        csvLogger=new CSVLogger(columns);
        csvContinue = new CSVLogger(new string[] {"original", "font_size", "error","font_size"});

        foreach (Pairs item in randomizedCollection)
        {
            csvLogger.AddEntry(new string[] { item.GetSentence(), item.GetFontSize().ToString() });
        }
        csvLogger.Save("original_task_"+PlayerPrefs.GetString("UserID")+".csv");

        //Init vriables
        currIndex = 0;
        errorString = "";

       

        nextBtn.onClick.AddListener(IncreaseIndex);
        prevBtn.onClick.AddListener(DecreaseIndex);

        foreach(Button b in buttonList)
        {
            b.onClick.AddListener(GetButtonChar);
        }



        //Init interface
        prevString.text = "Previous string: ---";
        currString.text = "Current string: " + randomizedCollection[currIndex].GetSentence() + " Current size: "+ randomizedCollection[currIndex].GetFontSize();
        nextString.text = "Next string: " + randomizedCollection[currIndex + 1].GetSentence() + " Nextsize: " + randomizedCollection[currIndex +1].GetFontSize();
        indexMonitor.text = "Index pos: " + currIndex;
        SetTask(randomizedCollection[currIndex].GetSentence());
    }

    private void GetButtonChar()
    {
        string value = EventSystem.current.currentSelectedGameObject.transform.GetComponentInChildren<TMP_Text>().text;
        EventSystem.current.currentSelectedGameObject.transform.GetComponent<Image>().color = Color.red;
       errorString += value;
    }

    private void DecreaseIndex()
    {
        if (currIndex > 0)
        {
            currIndex--;

        }

        if (currIndex == 0)
        {
            prevString.text = "Previous string: ---";

        }
        else
        {
          prevString.text = "Previous string:" + randomizedCollection[currIndex - 1].GetSentence()  +" Previous size: " + randomizedCollection[currIndex-1].GetFontSize();

        }

        currString.text = "Current string: " + randomizedCollection[currIndex].GetSentence() + " Current size: " + randomizedCollection[currIndex].GetFontSize();
        nextString.text = "Next string: " + randomizedCollection[currIndex + 1].GetSentence() + " Next size: " + randomizedCollection[currIndex+1].GetFontSize();
        indexMonitor.text = "Index pos: " + currIndex;
        SetTask(randomizedCollection[currIndex].GetSentence());
    }

    private void IncreaseIndex()
    {
        //Data
        errorCollection.Add(new Pairs(randomizedCollection[currIndex].GetFontSize(), errorString));
        csvContinue.AddEntry(new string[] {
            randomizedCollection[currIndex].GetSentence(),
            randomizedCollection[currIndex].GetFontSize().ToString(),
            errorString,
            randomizedCollection[currIndex].GetFontSize().ToString(),
        });
        String timeStamp = GetTimestamp(DateTime.Now);
        csvContinue.Save("result_"+ PlayerPrefs.GetString("UserID") + "_" + taskType + timeStamp + ".csv");
        errorString = "";


        //View
        if (currIndex < randomizedCollection.Count-1)
        {
            currIndex++;
        }
        if (currIndex == randomizedCollection.Count - 1)
        {
            nextString.text = "Next string: ---";

        }
        else
        {
            nextString.text = randomizedCollection[currIndex + 1].GetSentence() + " Next size: " + randomizedCollection[currIndex+1].GetFontSize();
        }
        prevString.text = "Previous string:" + randomizedCollection[currIndex - 1].GetSentence() + " Previous size: " + randomizedCollection[currIndex-1].GetFontSize();
        currString.text = "Current string: " + randomizedCollection[currIndex].GetSentence() + " Current size: " + randomizedCollection[currIndex].GetFontSize();
        indexMonitor.text = "Index pos: " + currIndex;
        SetTask(randomizedCollection[currIndex].GetSentence());

        foreach (Button b in buttonList)
        {
            b.GetComponent<Image>().color = Color.white;
        }


    }

    public static String GetTimestamp(DateTime value)
    {
        return value.ToString("MMddHHmmss");
    }

    public void SetTask(string sentence)
    {

        for (int i = 0; i < 10; i++)
        {
            buttonList[i].GetComponentInChildren<TMP_Text>().text = randomizedCollection[currIndex].GetSentence()[i].ToString();
        }
    }


    public void CreateCollection(int [] fSizes, string [] sStrings)
    {
        int count = 0;
        for (int i=0; i < fSizes.Length; i++)
        {
            stringsCollection.Add(new Pairs(fontSizes[i], sStrings[count]));
            randomizedCollection.Add(new Pairs(fontSizes[i], sStrings[count]));
            if (count < sStrings.Length-1)
            {
                count++;
            }
            else
            {
                count = 0;
            }
        }

    }


    // Update is called once per frame
    void Update()
    {
       
    }

    public static void Shuffle<T>(List<T> list)
    {

        for (int i = list.Count - 1; i > 0; i--)

        {

            int r = UnityEngine.Random.Range(0, i);

            T temp = list[i];

            list[i] = list[r];

            list[r] = temp;

        }

    }

}
