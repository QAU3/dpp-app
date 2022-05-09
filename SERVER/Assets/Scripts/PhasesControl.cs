using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

public class PhasesControl : MonoBehaviour
{
    public string PhrasesTask;
    public string value;
    string fontTypeOne, fontTypeTwo; 
    [Header("UI")]
    public Button nextBtn;
    public Button prevBtn, returnBtn, castBtn,setOneBtn,setTwoBtn,setThreeBtn, setMode, instructions, ending, rotationOne,rotationTwo,rotationThree;
    public TMP_Text prevDisplay, currDisplay, nextDisplay, indexMonitor, sizeMonitor, noLinesMonitor;

    [Header("Confg")]
    public int noLines;
    public int noRepetitions;
    public TextAsset dataFile;
    public GameObject _MENU_, _PHRASESTASK_;

    [Header("Media")]
    public AudioSource audioPlayer;
    public AudioClip[] audioFiles;

    [Header("Connection")]
    public UDPSender udpSender;

    private string[] dataLines, shuffledLines, splitedArray;
    System.Random rnd = new System.Random();
    private List<PhrasesObject> phrasesOrder;
    private List<string[]> groupedStrings;
    private string taskTypeOne, taskTypeTwo, taskTypeThree, taskTypeFour, taskTypeFive, taskTypeSix, taskThree, currSet;
    private int currIndex, phrasesRoation;
    public CSVLogger csvLogger, csvContinue;

    private string QMESSAGE;

    private void Start()
    {
        noLines = 1;
        noRepetitions = 18;
        groupedStrings = new List<string[]>();
        phrasesOrder = new List<PhrasesObject>();
       
        setOneBtn.onClick.AddListener(SetSetOne);
        setTwoBtn.onClick.AddListener(SetSetTwo);
        setThreeBtn.onClick.AddListener(SetSetThree);
        setMode.onClick.AddListener(SetSeqMode);
        
        

        nextBtn.onClick.AddListener(NextIndex);
        prevBtn.onClick.AddListener(PrevIndex);
        returnBtn.onClick.AddListener(ChangeScene);

        instructions.onClick.AddListener(delegate { SendTaskMessage("instructions"); });
        ending.onClick.AddListener(delegate { SendTaskMessage("end"); });

        rotationThree.onClick.AddListener(delegate { SetPhrasesRotation(3); });
        rotationTwo.onClick.AddListener(delegate { SetPhrasesRotation(2); });
        rotationOne.onClick.AddListener(delegate { SetPhrasesRotation(1); });
        QMESSAGE = "Questions \n|\n|\n|\n" + char.ConvertFromUtf32(0x25BC);

        castBtn.onClick.AddListener(StartTask);

        noLinesMonitor.text ="no. setences: "+ noLines;
    }

    private void SetSeqMode()
    {
        if (setMode.GetComponentInChildren<Text>().text == "normal")
        {
            setMode.GetComponentInChildren<Text>().text = "inverse";
        }
        else
        {
            setMode.GetComponentInChildren<Text>().text = "normal";
        }
    }

    private void SendTaskMessage(string v)
    {

        NetworkMessage message;
        string instructions;

        if (v == "instructions")
            {
                if (PhrasesTask != "minimum")
                {
                instructions = "In the next task, you will see 2 phrases. Please read them aloud and then answer the questions.";
                message = new NetworkMessage("message", instructions, "static", 64, "arial", "null",  PlayerPrefs.GetString("UserID"));
                udpSender.sendNetworkMessage(message);
                //udpSender.sendString("message_static;" + ";64");
                }
            }
        
        
        if (v == "end")
        {
            instructions = "ITask finished.\n Thank you!!";
            message = new NetworkMessage("message", instructions, "static", 64, "arial", "null",  PlayerPrefs.GetString("UserID"));
            udpSender.sendNetworkMessage(message);
           // udpSender.sendString("message_static;" + "Task finished.\n Thank you!!;64");
        }
    }


    private void DecreaseCounter()
    {
        if (noLines > 1) { noLines--; }
        noLinesMonitor.text = "no. setences: " + noLines;

    }

    private void IncreaseCounter()
    {
        if (noLines < 50) { noLines++; }
        noLinesMonitor.text = "no. setences: " + noLines;

    }

    private void StartTask()
    {
        NetworkMessage message;

        message = new NetworkMessage(PhrasesTask, phrasesOrder[currIndex].Phrases, phrasesOrder[currIndex].Type, phrasesOrder[currIndex].FontSize, phrasesOrder[currIndex].FontType,currSet,  PlayerPrefs.GetString("UserID"));
        udpSender.sendNetworkMessage(message);
        // udpSender.sendString(phrasesOrder[currIndex].Type+"_"+PhrasesTask + ";" + phrasesOrder[currIndex].Phrases + ";" + phrasesOrder[currIndex].FontSize + ";" + PhrasesTask);


    }

    private void ChangeScene()
    {
        currIndex = 0;
        noLines = 1;
        phrasesRoation = 1;

        setOneBtn.GetComponentInChildren<Image>().color = Color.white;
        setTwoBtn.GetComponentInChildren<Image>().color = Color.white;
        setThreeBtn.GetComponentInChildren<Image>().color = Color.white;
        rotationOne.transform.GetComponent<Image>().color = Color.white;
        rotationTwo.transform.GetComponent<Image>().color = Color.white;
        rotationThree.transform.GetComponent<Image>().color = Color.white;

        setOneBtn.gameObject.SetActive(false);
        setTwoBtn.gameObject.SetActive(false);
        setThreeBtn.gameObject.SetActive(false);

        noLinesMonitor.text = "no. setences: " + noLines;


        currDisplay.text = "---";
        prevDisplay.text = "---";
        nextDisplay.text = "---";
        castBtn.gameObject.SetActive(false);

        _PHRASESTASK_.SetActive(false);
        _MENU_.SetActive(true);
    }

    private void NextIndex()
    {

        var temp = phrasesOrder[currIndex].Phrases;
        if (temp ==QMESSAGE)
        {
            temp = "Q_A";
        }


        csvContinue.AddEntry(new string[] {
             temp,
           phrasesOrder[currIndex].FontSize.ToString(),
           phrasesOrder[currIndex].Type,
           phrasesRoation.ToString(),
           currSet,
           value,
           GetTimestamp(DateTime.Now)
    });

        csvContinue.Save("result_" + PlayerPrefs.GetString("UserID") + "_Phrases_" + PhrasesTask
            +"_"+currSet+"_"+phrasesRoation+".csv");

        if (currIndex < phrasesOrder.Count - 1)
        {
            currIndex++;
        }
        else { return; }

        SetView(currIndex);
        //udpSender.sendString(phrasesOrder[currIndex].Type + "_" + PhrasesTask  + ";" + phrasesOrder[currIndex].Phrases + ";" + phrasesOrder[currIndex].FontSize);
        NetworkMessage message;

        message = new NetworkMessage(PhrasesTask, phrasesOrder[currIndex].Phrases, phrasesOrder[currIndex].Type, phrasesOrder[currIndex].FontSize, phrasesOrder[currIndex].FontType, currSet,  PlayerPrefs.GetString("UserID"));
        udpSender.sendNetworkMessage(message);
    }

    private void PrevIndex()
    {
        if (currIndex > 0)
        {
            currIndex--;

        }


        SetView(currIndex);
    
    }

    private void SetSetTwo()
    {
        currSet = "SetTwo";


        fontTypeOne = "times";
        fontTypeTwo = "arial";


        if (setMode.GetComponentInChildren<Text>().text == "normal")
        {
            taskTypeOne = "circular";
            taskTypeTwo = "static";
            taskTypeThree = "linear";
            taskTypeFour = "circular";
            taskTypeFive = "linear";
            taskTypeSix = "static";
        }
        else
        {
            taskTypeOne = "static";
            taskTypeTwo = "circular";
            taskTypeThree = "circular";
            taskTypeFour = "linear";
            taskTypeFive = "static";
            taskTypeSix = "linear";
        }



        setTwoBtn.GetComponentInChildren<Image>().color = Color.green;
        setOneBtn.GetComponentInChildren<Image>().color = Color.white;
        setThreeBtn.GetComponentInChildren<Image>().color = Color.white;
        StartPhraseTask();
    }
    private void SetSetOne()
    {
        currSet = "SetOne";

        fontTypeOne = "times";
        fontTypeTwo = "arial";

        if (setMode.GetComponentInChildren<Text>().text == "normal")
        {
            taskTypeOne = "linear";
            taskTypeTwo = "static";
            taskTypeThree = "circular";
            taskTypeFour = "static";
            taskTypeFive = "linear";
            taskTypeSix = "circular";
        }
        else
        {
            taskTypeOne = "static";
            taskTypeTwo = "linear";
            taskTypeThree = "static";
            taskTypeFour = "circular";
            taskTypeFive = "circular";
            taskTypeSix = "linear";
        }


        setOneBtn.GetComponentInChildren<Image>().color = Color.green;
        setTwoBtn.GetComponentInChildren<Image>().color = Color.white;
        setThreeBtn.GetComponentInChildren<Image>().color = Color.white;

        StartPhraseTask();
    }
    private void SetSetThree()
    {
        currSet = "SetThree";

        fontTypeOne = "times";
        fontTypeTwo = "arial";

        if (setMode.GetComponentInChildren<Text>().text == "normal")
        {
            taskTypeOne = "linear";
            taskTypeTwo = "circular";
            taskTypeThree = "linear";
            taskTypeFour = "static";
            taskTypeFive = "circular";
            taskTypeSix = "static";
        }
        else
        {
            taskTypeOne = "circular";
            taskTypeTwo = "linear";
            taskTypeThree = "static";
            taskTypeFour = "linear";
            taskTypeFive = "static";
            taskTypeSix = "circular";
        }





        setOneBtn.GetComponentInChildren<Image>().color = Color.white;
        setTwoBtn.GetComponentInChildren<Image>().color = Color.white;
        setThreeBtn.GetComponentInChildren<Image>().color = Color.green;
        StartPhraseTask();


    }

    public void StartPhraseTask()
    {
        
        currIndex = 0;
        groupedStrings = new List<string[]>();
        phrasesOrder = new List<PhrasesObject>();
        DecodeString();
        HardcodePhrases();
        //saving
        string[] columns = new string[] { "string", "font_size", "type", "roation_case" };
        csvLogger = new CSVLogger(columns);
        csvContinue = new CSVLogger(new string[] { "original", "font_size", "type", "rotation_case", "set","notes", "time_stamp" });
        foreach (PhrasesObject obj in phrasesOrder)
        {
            csvLogger.AddEntry(new string[] { obj.Phrases, obj.FontSize.ToString(), obj.Type, phrasesRoation.ToString()});
        }
        String timeStamp = GetTimestamp(DateTime.Now);
        csvLogger.Save("original_phrases_" + PhrasesTask +
            "_task_" + PlayerPrefs.GetString("UserID") + "_" + timeStamp + "_"+phrasesRoation 
            +"_"+currSet +".csv");


        castBtn.gameObject.SetActive(true);

        SetView(currIndex);
     


    }





    public void SetView(int curr) {
        int prev = curr - 1;
        int next = curr + 1;


        if (prev < 0)
        {
            prevDisplay.text = "Previous string: ---";

        }
        else
        {
            prevDisplay.text = "Previous string:\n" + phrasesOrder[prev].Phrases;

        }

        if (next > phrasesOrder.Count - 1)
        {
            audioPlayer.PlayOneShot(audioFiles[2], 1.0f);
            nextDisplay.text = "Next string: --- ";

        }
        else
        {
            nextDisplay.text = "Next string:\n " + phrasesOrder[next].Phrases;

        }

        currDisplay.text = "Current string:\n " + phrasesOrder[curr].Phrases;

        indexMonitor.text = "Index pos: " + curr;
        sizeMonitor.text = "Current size: " + phrasesOrder[curr].FontSize;
    }
    public void DecodeString()
    {
        dataLines = dataFile.text.Split('\n');
        switch (PhrasesTask)
        {
            case "reading":
                noLines = 2;
                break;
            case "minimum":
                noLines = 1;
                break;
            case "confort":
                noLines = 1;
                break;
        }

        //shuffledLines = dataLines.OrderBy(x => rnd.Next()).ToArray();
        string[] temp = new string[noLines];
        int count = 0;
        for (int i = 0; i < dataLines.Length - 1; i++)
        {

            if (count < noLines)
            {
                temp[count] = dataLines[i];
                count++;
            }
            else
            {
                groupedStrings.Add(temp);
                count = 0;
                temp = new string[noLines];
            }


        }

    }
    public static String GetTimestamp(DateTime value)
    {
        return value.ToString("MMddHHmmss");
    }
    public void SetPhrasesRotation(int value)
    {
        phrasesRoation = value;
        rotationOne.transform.GetComponent<Image>().color = Color.white;
        rotationTwo.transform.GetComponent<Image>().color = Color.white;
        rotationThree.transform.GetComponent<Image>().color = Color.white;

        switch (value)
        {
            case 1:
                rotationOne.transform.GetComponent<Image>().color = Color.green;

                break;
            case 2:
                rotationTwo.transform.GetComponent<Image>().color = Color.green;
                break;

            case 3:
                rotationThree.transform.GetComponent<Image>().color = Color.green;
                break;
        }
        setOneBtn.gameObject.SetActive(true);
        setTwoBtn.gameObject.SetActive(true);
        setThreeBtn.gameObject.SetActive(true);


    }
    public void HardcodePhrases()
    {
        //var randomNumbers = Enumerable.Range(0, 12).OrderBy(x => rnd.Next()).Take(12).ToList();

        //foreach(int i in randomNumbers)
        //{
        //    Debug.Log(i);
        //}
        switch (PhrasesTask)
        {



            case "reading":

                switch (phrasesRoation)
                {
                    case 1:

                        ///TIMES
                        ///OTHER FONT
                        ///STARTS HERE
                        ///
                        ///FIRST HALF
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[36]), 32, taskTypeOne, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[37]), 32, taskTypeTwo, fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[38]), 40, taskTypeOne, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[39]), 40, taskTypeTwo, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[40]), 48, taskTypeOne, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[41]), 48, taskTypeTwo, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        ///SECONS HALF
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[42]), 32, taskTypeOne, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[43]), 32, taskTypeTwo, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[44]), 40, taskTypeOne, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[45]), 40, taskTypeTwo, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[46]), 48, taskTypeOne, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[47]), 48, taskTypeTwo, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));



                        ///STATIC LINEAR///

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[48]), 32, taskTypeThree, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[49]), 32, taskTypeFour, fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[50]), 40, taskTypeThree, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[51]), 40, taskTypeFour, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[52]), 48, taskTypeThree, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[53]), 48, taskTypeFour, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        ///SECONS HALF
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[54]), 32, taskTypeThree, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[55]), 32, taskTypeFour, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[56]), 40, taskTypeThree, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[57]), 40, taskTypeFour, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[58]), 48, taskTypeThree, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[59]), 48, taskTypeFour, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        ///CIRCULAR LINEAR///

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[60]), 32, taskTypeFive, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[61]), 32, taskTypeSix, fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[62]), 40, taskTypeFive, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[63]), 40, taskTypeSix, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[64]), 48, taskTypeFive, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[65]), 48, taskTypeSix, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        ///SECONS HALF
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[66]), 32, taskTypeFive, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[67]), 32, taskTypeSix, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[68]), 40, taskTypeFive, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[69]), 40, taskTypeSix, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[70]), 48, taskTypeFive, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[71]), 48, taskTypeSix, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));




                        ///TIMES
                        ///STATIC LINEAR
                        ///FIRST HALF
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[0]), 32, taskTypeOne, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[1]), 32, taskTypeTwo, fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[2]), 40, taskTypeOne, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[3]), 40, taskTypeTwo, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[4]), 48, taskTypeOne, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[5]), 48, taskTypeTwo, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        ///SECONS HALF
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[6]), 32, taskTypeOne, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[7]), 32, taskTypeTwo, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[8]), 40, taskTypeOne, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[9]), 40, taskTypeTwo, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[10]), 48, taskTypeOne, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[11]), 48, taskTypeTwo, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));



                        ///STATIC LINEAR///

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[12]), 32, taskTypeThree, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[13]), 32, taskTypeFour, fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[14]), 40, taskTypeThree, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[15]), 40, taskTypeFour, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[16]), 48, taskTypeThree, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[17]), 48, taskTypeFour, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        ///SECONS HALF
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[18]), 32, taskTypeThree, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[19]), 32, taskTypeFour, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[20]), 40, taskTypeThree, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[21]), 40, taskTypeFour, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[22]), 48, taskTypeThree, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[23]), 48, taskTypeFour, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        ///CIRCULAR LINEAR///

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[24]), 32, taskTypeFive, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[25]), 32, taskTypeSix, fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[26]), 40, taskTypeFive, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[27]), 40, taskTypeSix, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[28]), 48, taskTypeFive, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[29]), 48, taskTypeSix, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        ///SECONS HALF
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[30]), 32, taskTypeFive, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[31]), 32, taskTypeSix, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[32]), 40, taskTypeFive, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[33]), 40, taskTypeSix, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[34]), 48, taskTypeFive, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[35]), 48, taskTypeSix, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));


                        
                        break;
                    case 2:
                       
                        ///ARIAL
                        ///OTHER FONT
                        ///STARTS HERE
                        ///


                        ///FIRST HALF


                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[38]), 40, taskTypeOne, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[39]), 40, taskTypeTwo, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[40]), 48, taskTypeOne, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[41]), 48, taskTypeTwo, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[36]), 32, taskTypeOne, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[37]), 32, taskTypeTwo, fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));
                        ///SECONS HALF

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[44]), 40, taskTypeOne, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[45]), 40, taskTypeTwo, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[46]), 48, taskTypeOne, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[47]), 48, taskTypeTwo, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[42]), 32, taskTypeOne, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[43]), 32, taskTypeTwo, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        ///STATIC LINEAR///



                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[50]), 40, taskTypeThree, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[51]), 40, taskTypeFour, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[52]), 48, taskTypeThree, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[53]), 48, taskTypeFour, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[48]), 32, taskTypeThree, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[49]), 32, taskTypeFour, fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));
                        ///SECONS HALF


                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[56]), 40, taskTypeThree, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[57]), 40, taskTypeFour, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[58]), 48, taskTypeThree, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[59]), 48, taskTypeFour, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[54]), 32, taskTypeThree, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[55]), 32, taskTypeFour, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));
                        ///CIRCULAR LINEAR///



                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[62]), 40, taskTypeFive, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[63]), 40, taskTypeSix, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[64]), 48, taskTypeFive, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[65]), 48, taskTypeSix, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[60]), 32, taskTypeFive, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[61]), 32, taskTypeSix, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        ///SECONS HALF


                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[68]), 40, taskTypeFive, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[69]), 40, taskTypeSix, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[70]), 48, taskTypeFive, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[71]), 48, taskTypeSix, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[66]), 32, taskTypeFive, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[67]), 32, taskTypeSix, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        ///TIMES
                        ///STATIC LINEAR
                        ///FIRST HALF
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[2]), 40, taskTypeOne, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[3]), 40, taskTypeTwo, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[4]), 48, taskTypeOne, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[5]), 48, taskTypeTwo, fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[0]), 32, taskTypeOne, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[1]), 32, taskTypeTwo, fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        ///SECONS HALF


                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[8]), 40, taskTypeOne, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[9]), 40, taskTypeTwo, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[10]), 48, taskTypeOne, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[11]), 48, taskTypeTwo, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[6]), 32, taskTypeOne, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[7]), 32, taskTypeTwo, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        ///STATIC LINEAR///



                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[14]), 40, taskTypeThree, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[15]), 40, taskTypeFour, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[16]), 48, taskTypeThree, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[17]), 48, taskTypeFour, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[12]), 32, taskTypeThree, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[13]), 32, taskTypeFour, fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        ///SECONS HALF


                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[20]), 40, taskTypeThree, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[21]), 40, taskTypeFour, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[22]), 48, taskTypeThree, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[23]), 48, taskTypeFour, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[18]), 32, taskTypeThree, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[19]), 32, taskTypeFour, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));
                        ///CIRCULAR LINEAR///



                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[26]), 40, taskTypeFive, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[27]), 40, taskTypeSix, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[28]), 48, taskTypeFive, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[29]), 48, taskTypeSix, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[24]), 32, taskTypeFive, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[25]), 32, taskTypeSix, fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        ///SECONS HALF


                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[32]), 40, taskTypeFive, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[33]), 40, taskTypeSix, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[34]), 48, taskTypeFive, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[35]), 48, taskTypeSix, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[30]), 32, taskTypeFive, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[31]), 32, taskTypeSix, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        break;
                    case 3:

                        
                        ///ARILA
                        ///OTHER FONT
                        ///STARTS HERE
                        ///
                        ///FIRST HALF
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[40]), 48, taskTypeOne, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[41]), 48, taskTypeTwo, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[36]), 32, taskTypeOne, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[37]), 32, taskTypeTwo, fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[38]), 40, taskTypeOne, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[39]), 40, taskTypeTwo, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        ///SECONS HALF



                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[46]), 48, taskTypeOne, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[47]), 48, taskTypeTwo, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[42]), 32, taskTypeOne, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[43]), 32, taskTypeTwo, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[44]), 40, taskTypeOne, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[45]), 40, taskTypeTwo, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        ///STATIC LINEAR///





                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[52]), 48, taskTypeThree, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[53]), 48, taskTypeFour, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[48]), 32, taskTypeThree, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[49]), 32, taskTypeFour, fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[50]), 40, taskTypeThree, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[51]), 40, taskTypeFour, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));
                        ///SECONS HALF




                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[58]), 48, taskTypeThree, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[59]), 48, taskTypeFour, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[54]), 32, taskTypeThree, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[55]), 32, taskTypeFour, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[56]), 40, taskTypeThree, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[57]), 40, taskTypeFour, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));
                        ///CIRCULAR LINEAR///





                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[64]), 48, taskTypeFive, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[65]), 48, taskTypeSix, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[60]), 32, taskTypeFive, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[61]), 32, taskTypeSix, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[62]), 40, taskTypeFive, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[63]), 40, taskTypeSix, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        ///SECONS HALF




                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[70]), 48, taskTypeFive, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[71]), 48, taskTypeSix, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[66]), 32, taskTypeFive, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[67]), 32, taskTypeSix, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[68]), 40, taskTypeFive, fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[69]), 40, taskTypeSix, fontTypeTwo));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeTwo));



                        ///TIMES
                        ///STATIC LINEAR

                        ///FIRST 
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[4]), 48, taskTypeOne, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[5]), 48, taskTypeTwo, fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[0]), 32, taskTypeOne, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[1]), 32, taskTypeTwo, fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[2]), 40, taskTypeOne, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[3]), 40, taskTypeTwo, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        ///SECONS HALF



                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[10]), 48, taskTypeOne, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[11]), 48, taskTypeTwo, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[6]), 32, taskTypeOne, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[7]), 32, taskTypeTwo, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[8]), 40, taskTypeOne, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[9]), 40, taskTypeTwo, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));


                        ///STATIC LINEAR///





                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[16]), 48, taskTypeThree, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[17]), 48, taskTypeFour, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[12]), 32, taskTypeThree, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[13]), 32, taskTypeFour, fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[14]), 40, taskTypeThree, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[15]), 40, taskTypeFour, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        ///SECONS HALF



                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[22]), 48, taskTypeThree, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[23]), 48, taskTypeFour, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[18]), 32, taskTypeThree, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[19]), 32, taskTypeFour, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[20]), 40, taskTypeThree, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[21]), 40, taskTypeFour, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));
                        ///CIRCULAR LINEAR/

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[28]), 48, taskTypeFive, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[29]), 48, taskTypeSix, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[24]), 32, taskTypeFive, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[25]), 32, taskTypeSix, fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[26]), 40, taskTypeFive, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[27]), 40, taskTypeSix, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        ///SECONS HALF



                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[34]), 48, taskTypeFive, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[35]), 48, taskTypeSix, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[30]), 32, taskTypeFive, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[31]), 32, taskTypeSix, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[32]), 40, taskTypeFive, fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[33]), 40, taskTypeSix, fontTypeOne));


                        phrasesOrder.Add(new PhrasesObject(QMESSAGE, 52, "phra_question", fontTypeOne));



                        break;

                }

               
                break;
            case "minimum":
                fontTypeOne = "arial";
                fontTypeTwo = "times";

                switch (phrasesRoation)
                {
                    case 1:
                     
                       
                        ///ANOTHER FONT///
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[206]), 8, "static", fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[207]), 8, "circular", fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[208]), 8, "linear", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[209]), 8, "static", fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[210]), 8, "circular", fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[211]), 8, "linear", fontTypeTwo));

                        ///TIMES
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[200]), 8, "static", fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[201]), 8, "circular", fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[202]), 8, "linear", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[203]), 8, "static", fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[204]), 8, "circular", fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[205]), 8, "linear", fontTypeOne));



                        break;
                    case 2:
                 

                        ///ARIL

                        ///ANOTHER FONT///
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[206]), 8, "circular", fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[207]), 8, "linear", fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[208]), 8, "static", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[209]), 8, "circular", fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[210]), 8, "linear", fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[211]), 8, "static", fontTypeTwo));

                        ///TIMES
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[200]), 8, "cirular", fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[201]), 8, "linear", fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[202]), 8, "static", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[203]), 8, "circular", fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[204]), 8, "linear", fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[205]), 8, "static", fontTypeOne));

                        break;
                    case 3:
                    
                        ///ANOTHER FONT///
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[206]), 8, "linear", fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[207]), 8, "static", fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[208]), 8, "circular", fontTypeTwo));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[209]), 8, "linear", fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[210]), 8, "static", fontTypeTwo));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[211]), 8, "circular", fontTypeTwo));

                        ///TIMES
                        ///
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[200]), 8, "linear", fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[201]), 8, "static", fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[202]), 8, "circular", fontTypeOne));

                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[203]), 8, "linear", fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[204]), 8, "static", fontTypeOne));
                        phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[205]), 8, "circular", fontTypeOne));




                        break;


                }
                /* phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[19]), 8, taskTypeOne));
                 phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[20]), 8, taskTypeTwo));

                 phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[21]), 8, taskTypeOne));
                 phrasesOrder.Add(new PhrasesObject(string.Join("\n", groupedStrings[22]), 8, taskTypeTwo));
                */


                break;
            case "confort":
              
                break;
        }

      
    }
}