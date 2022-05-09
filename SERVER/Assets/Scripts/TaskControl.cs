using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class TaskControl : MonoBehaviour
{
    public string value;
    public string[] studyStrings;
    public int[] fontSizes;
    public Button[] buttonList;
    string fontTypeOne, fontTypeTwo;
    private List<Pairs> stringsCollection, randomizedCollection, errorCollection;
    private List<StringCollection> stringsOrder; 
    public List<string> alphabetUpper, alphabetLower;

    private int currIndex;
    private string errorString, taskTypeOne, taskTypeTwo, taskTypeThree, taskTypeFour, taskTypeFive, taskTypeSix;


    [Header("String monitor")]
    public TMP_Text prevString;
    public TMP_Text currString;
    public TMP_Text nextString;
    public TMP_Text indexMonitor;
    public Button nextBtn, prevBtn, startBtn, returnBtn,setOneBtn,setTwoBtn, setThreeBtn, setMode;

    [Header("Task configuration")]
    public string taskType;
    public bool isPractice;
    public string alphabet;
    public Button instructions, ending;

    [Header("Task configuration")]
    public UDPSender updSender;
    [Header("Audio config")]
    public AudioSource audioPlayer;
    public AudioClip[] audioFiles;

    public CSVLogger csvLogger, csvContinue;
    System.Random r = new System.Random();
    // Start is called before the first frame update

    //NEWAPROACH
    public GameObject _MENU_, _CHARTASK_;
    private string QMESSAGE;

    private int SET;
    void Start()
    {
        nextBtn.onClick.AddListener(IncreaseIndex);
        prevBtn.onClick.AddListener(DecreaseIndex);
        startBtn.onClick.AddListener(StartTask);
        returnBtn.onClick.AddListener(SendToScene);
        setOneBtn.onClick.AddListener(SetSetOne);
        setTwoBtn.onClick.AddListener(SetSetTwo);
        setThreeBtn.onClick.AddListener(SetSetThree);

        setMode.onClick.AddListener(SetSeqMode);
        instructions.onClick.AddListener(delegate { SendTaskMessage("instructions"); });
        ending.onClick.AddListener(delegate { SendTaskMessage("end"); });
        startBtn.gameObject.SetActive(false);
        QMESSAGE = "Questions \n|\n|\n|\n" + char.ConvertFromUtf32(0x25BC);


    }

    private void SetSeqMode()
    {
        if (setMode.GetComponentInChildren<TMP_Text>().text == "normal") 
        {
            setMode.GetComponentInChildren<TMP_Text>().text = "inverse";
        }
        else
        {
            setMode.GetComponentInChildren<TMP_Text>().text = "normal";
        }
    }

    private void SendTaskMessage(string v)
    {
        NetworkMessage message;
        string instructions;
        if (v == "instructions")
        {
           instructions = "In the next task, you will see a squence of 13 characters. Please read aloud one by one and then answer the questions.";
            message = new NetworkMessage("message", instructions, "static", 64, "arial", "null",  PlayerPrefs.GetString("UserID"));
            updSender.sendNetworkMessage(message);
            // updSender.sendString("message_static;"+"In the next task, you will see a squence of 13 characters. Please read aloud one by one and then answer the questions.;64");
        }
        if (v == "end")
        {
            instructions = "Task finished.\n Thank you!!";
            message = new NetworkMessage("message", instructions, "static", 64, "arial", "null",  PlayerPrefs.GetString("UserID"));
            updSender.sendNetworkMessage(message);
            updSender.sendString("message_static;" + "Task finished.\n Thank you!!;64");
        }
    }

    public void StartCharTask()
    {


        // setThreeBtn.GetComponent<TMP_Text>().text = "Times";
        //fontTypeOne = "times";
        //fontTypeTwo = "arial";
        //fontTypeTwo = "arial";

        //Init collections
        stringsCollection = new List<Pairs>();
        randomizedCollection = new List<Pairs>();
        errorCollection = new List<Pairs>();
        stringsOrder = new List<StringCollection>();

        alphabetUpper = new List<string>();
        alphabetLower = new List<string>();
       

        HarcodedStrings();

        if (!isPractice)
        {
        ////Saving original randomized collection
         string[] columns = new string[] { "string", "font_size","type","time_stamp"};
         csvLogger = new CSVLogger(columns);
          csvContinue = new CSVLogger(new string[] { "original", "font_size","type", "errors", "notes","time_stamp" });
           foreach (StringCollection obj in stringsOrder)
          {
             csvLogger.AddEntry(new string[] { obj.Sentece,obj.FontSize.ToString(),obj.Type});
          }
            String timeStamp = GetTimestamp(DateTime.Now);
            csvLogger.Save("original_" + taskType + "_task_" + PlayerPrefs.GetString("UserID")+"_"+timeStamp+".csv");
        }

        ////Init vriables
        currIndex = 0;
        errorString = "";
        value = "";

        /////Adding listerner to buttons
        foreach (Button b in buttonList)
        {
            b.onClick.AddListener(GetButtonChar);
        }

        SetView(currIndex);
        SetTask(stringsOrder[currIndex].Sentece);
        startBtn.gameObject.SetActive(true);
      

    }

    private void SetSetTwo()
    {


        fontTypeOne = "times";
        fontTypeTwo = "arial";

        SET = 2;

        //if (setMode.GetComponentInChildren<TMP_Text>().text == "normal")
        //{
        //    taskTypeOne = "circular";
        //    taskTypeTwo = "static";
        //    taskTypeThree = "linear";
        //    taskTypeFour = "circular";
        //    taskTypeFive = "linear";
        //    taskTypeSix = "static";
        //}
        //else
        //{
        //    taskTypeOne = "static";
        //    taskTypeTwo = "circular";
        //    taskTypeThree = "circular";
        //    taskTypeFour = "linear";
        //    taskTypeFive = "static";
        //    taskTypeSix = "linear";
        //}

        

        setTwoBtn.GetComponentInChildren<Image>().color = Color.green;
        setOneBtn.GetComponentInChildren<Image>().color = Color.white;
        setThreeBtn.GetComponentInChildren<Image>().color = Color.white;
        StartCharTask();
    }

    private void SetSetOne()
    {
        SET = 1;
        fontTypeOne = "times";
        fontTypeTwo = "arial";
        //if (setMode.GetComponentInChildren<TMP_Text>().text == "normal")
        //{
        //    taskTypeOne = "linear";
        //    taskTypeTwo = "static";
        //    taskTypeThree = "circular";
        //    taskTypeFour = "static";
        //    taskTypeFive = "linear";
        //    taskTypeSix = "circular";
        //}
        //else
        //{
        //    taskTypeOne = "static";
        //    taskTypeTwo = "linear";
        //    taskTypeThree = "static";
        //    taskTypeFour = "circular";
        //    taskTypeFive = "circular";
        //    taskTypeSix = "linear";
        //}


        setOneBtn.GetComponentInChildren<Image>().color = Color.green;
        setTwoBtn.GetComponentInChildren<Image>().color = Color.white;
        setThreeBtn.GetComponentInChildren<Image>().color = Color.white;


        StartCharTask();
    }

    private void SetSetThree()
    {
        SET = 3;
        fontTypeOne = "times";
        fontTypeTwo = "arial";


        //if (setMode.GetComponentInChildren<TMP_Text>().text == "normal")
        //{
        //    taskTypeOne = "linear";
        //    taskTypeTwo = "circular";
        //    taskTypeThree = "linear";
        //    taskTypeFour = "static";
        //    taskTypeFive = "circular";
        //    taskTypeSix = "static";
        //}
        //else
        //{
        //    taskTypeOne = "circular";
        //    taskTypeTwo = "linear";
        //    taskTypeThree = "static";
        //    taskTypeFour = "linear";
        //    taskTypeFive = "static";
        //    taskTypeSix = "circular";
        //}





        setOneBtn.GetComponentInChildren<Image>().color = Color.white;
        setTwoBtn.GetComponentInChildren<Image>().color = Color.white;
        setThreeBtn.GetComponentInChildren<Image>().color = Color.green;


        StartCharTask();

    }

    public string ShuffleString(string alphabet)
    {
        char[] temp = alphabet.ToCharArray();
        temp.Shuffle();
        return new string(temp);
    }

    private void SendToScene()
    {
        currIndex = 0;
        errorString = "";
        value = "";
        setOneBtn.GetComponentInChildren<Image>().color = Color.white;
        setTwoBtn.GetComponentInChildren<Image>().color = Color.white;
        setThreeBtn.GetComponentInChildren<Image>().color = Color.white;

        setMode.GetComponentInChildren<TMP_Text>().text = "normal";


        foreach (Button b in buttonList)
        {
            b.GetComponent<Image>().color = Color.white;
        }

        currString.text = "---";
        prevString.text= "---";
        nextString.text = "---";
        SetTask("AAAAAAAAAAAAA");
        startBtn.gameObject.SetActive(false);

        _CHARTASK_.SetActive(false);
        _MENU_.SetActive(true);
        //SceneManager.LoadScene("Main");
    }

    private void StartTask()
    {
        NetworkMessage message;
        message = new NetworkMessage("char", stringsOrder[currIndex].Sentece,stringsOrder[currIndex].Type,stringsOrder[currIndex].FontSize, stringsOrder[currIndex].FontType,"null",  PlayerPrefs.GetString("UserID"));
        updSender.sendNetworkMessage(message);
        //updSender.sendString(stringsOrder[currIndex].Type+";"+ stringsOrder[currIndex].Sentece + ";"+ stringsOrder[currIndex].FontSize) ;
        //updSender.sendString(taskType + ";" + randomizedCollection[currIndex].GetSentence() + ";" + randomizedCollection[currIndex].GetFontSize());

    }

    private void GetButtonChar()
    {
        string value = EventSystem.current.currentSelectedGameObject.transform.GetComponentInChildren<TMP_Text>().text;
        EventSystem.current.currentSelectedGameObject.transform.GetComponent<Image>().color = Color.red;
        char c = char.Parse(value);
      
        if (errorString.Contains(c) && errorString!="")
          {
            Debug.Log(errorString.Contains(c));
              EventSystem.current.currentSelectedGameObject.transform.GetComponent<Image>().color = Color.white;
              string temp = errorString.Replace(value,"");
              errorString= temp;

        }
        else
        {
            errorString += value;
        }
         

      

        // audioPlayer.PlayOneShot(audioFiles[1],1.0f);
    }

    private void DecreaseIndex()
    {
        if (currIndex > 0)
        {
           currIndex--;

        }

      
        SetView(currIndex);
        SetTask(stringsOrder[currIndex].Sentece);
    }

    private void IncreaseIndex()
    {

        //Data
        String timeStamp = GetTimestamp(DateTime.Now);
        if (!isPractice)
        {

            csvContinue.AddEntry(new string[] {
           stringsOrder[currIndex].Sentece,
           stringsOrder[currIndex].FontSize.ToString(),
           stringsOrder[currIndex].Type,
           errorString,
           value,
           timeStamp
            });

          
            csvContinue.Save("result_" + PlayerPrefs.GetString("UserID") + "_" + taskType + ".csv");
        }

       errorString = "";
       value = "";

        ////View
        if (currIndex < stringsOrder.Count-1)
        {
            currIndex++;
        }
        else { return; }
      
        Debug.Log(currIndex);
       SetView(currIndex);
       SetTask(stringsOrder[currIndex].Sentece);

        foreach (Button b in buttonList)
        {
            b.GetComponent<Image>().color = Color.white;
        }

        //  updSender.sendString(taskType + ";" + randomizedCollection[currIndex].GetSentence() + ";" + randomizedCollection[currIndex].GetFontSize());
        //updSender.sendString(stringsOrder[currIndex].Type + ";" + stringsOrder[currIndex].Sentece + ";" + stringsOrder[currIndex].FontSize);
        NetworkMessage message;
        message = new NetworkMessage("char", stringsOrder[currIndex].Sentece, stringsOrder[currIndex].Type, stringsOrder[currIndex].FontSize, stringsOrder[currIndex].FontType, "null",  PlayerPrefs.GetString("UserID"));
        updSender.sendNetworkMessage(message);

    }

    public static String GetTimestamp(DateTime value)
    {
        return value.ToString("MMddHHmmss");
    }

    public void SetTask(string sentence)
    {
        if (sentence== QMESSAGE) 
        {
            sentence = "Question-----";
        }
        errorString = "";
        for (int i = 0; i < sentence.Length; i++)
            
        {
            buttonList[i].GetComponentInChildren<TMP_Text>().text = sentence[i].ToString();
        }
        foreach (Button b in buttonList)
        {
            b.GetComponent<Image>().color = Color.white;
        }

    }


    public void GetData(string data)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(ThisWillBeExecutedOnTheMainThread(data));

    }

    public IEnumerator ThisWillBeExecutedOnTheMainThread(string data)
    {
        Debug.Log(data);
        value = data;
        yield return null;
    }
  
    public void SetView(int curr)
    {
        int prev = curr - 1;
        int next = curr + 1;

        if (prev < 0)
        {
            prevString.text = "Previous string: ---";

        }
        else
        {
            prevString.text = "Previous string: " + stringsOrder[prev].Sentece + " Previous size: " + stringsOrder[prev].FontSize;

        }

        if (next > stringsOrder.Count-1)
        {
            audioPlayer.PlayOneShot(audioFiles[2], 1.0f);
            nextString.text = "Next string: --- ";

        }
        else
        {
            nextString.text = "Next string: " + stringsOrder[next].Sentece + " Next tsize: " + stringsOrder[next].FontSize;

        }

        currString.text = "Current string: " + stringsOrder[currIndex].Sentece + " Current size: " + stringsOrder[currIndex].FontSize;
        indexMonitor.text = "Index pos: " + currIndex;

    }

    public void HarcodedStrings()
    {


        ///8pt - 32px
        /// 10 - 40px
        /// 12 -48
        //alphabetUpper.Add(ShuffleString(alphabet.ToUpper()));
        //alphabetUpper.Add(ShuffleString(alphabet.ToUpper()));
        //alphabetUpper.Add(ShuffleString(alphabet.ToUpper()));
        //alphabetUpper.Add(ShuffleString(alphabet.ToUpper()));
        //alphabetUpper.Add(ShuffleString(alphabet.ToUpper()));
        //alphabetUpper.Add(ShuffleString(alphabet.ToUpper()));

        //alphabetLower.Add(ShuffleString(alphabet));
        //alphabetLower.Add(ShuffleString(alphabet));
        //alphabetLower.Add(ShuffleString(alphabet));
        //alphabetLower.Add(ShuffleString(alphabet));
        //alphabetLower.Add(ShuffleString(alphabet));
        //alphabetLower.Add(ShuffleString(alphabet));


        // FileStream Upper = new FileStream(Application.persistentDataPath + "/alphabetUpper.txt", FileMode.Create);
        //BinaryFormatter bf = new BinaryFormatter();
        //bf.Serialize(Upper, alphabetUpper);
        //Upper.Close();

        //FileStream Lower = new FileStream(Application.persistentDataPath + "/alphabetLower.txt", FileMode.Create);
        //bf.Serialize(Lower, alphabetLower);
        //Lower.Close();

        //using (Stream stream = File.Open(Application.persistentDataPath + "/alphabetUpper.dat", FileMode.Open))
        //{
        //    var bfFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        //    alphabetUpper = (List<string>)bfFormatter.Deserialize(stream);
        //}

        //using (Stream stream = File.Open(Application.persistentDataPath + "/alphabetLower.dat", FileMode.Open))
        //{
        //    var bfFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        //    alphabetLower = (List<string>)bfFormatter.Deserialize(stream);
        //}

        ///HARDCODE RANDOM satic circular
        alphabetUpper.Add("IXESMGJYBTHOARKDWCULZPVQNF");
        alphabetUpper.Add("CLNPUETAVXFOZKYQIHDRSMGBWJ");
        alphabetUpper.Add("YTLCUBMSAWIONVPZXHGDFKQEJR");
        alphabetUpper.Add("MZOPCWTSDHFJYKQBXARVLEIUNG");
        alphabetUpper.Add("RINBSYPAFDEGUZXJOMCHQWKLVT");
        alphabetUpper.Add("ELRNPYQSKMBUIFXGJZWHTDVACO");

        //satatic linear
        alphabetUpper.Add("BOPVCHLTGFAZQENWMSDRYUXJIK");
        alphabetUpper.Add("SYICTOUBMQVLNKFEWZRPXDJHAG");
        alphabetUpper.Add("TDPSJAEFWLOMNBUHZCQYKGRVIX");
        alphabetUpper.Add("YHWDCNPXMEKOSJTGVQZLABUIRF");
        alphabetUpper.Add("QIRXVOYEWMNFKAGPZTJBDSUHLC");
        alphabetUpper.Add("ACYNFBELSURXDMKZIPHVJWGQOT");


        //linear circular
        alphabetUpper.Add("HYNLSDWVMBCUGXTZKPOQIFAERJ");
        alphabetUpper.Add("GQYANKRCOMPJUHETFBZISDWVXL");
        alphabetUpper.Add("JTIOHDQZFRULYBMVKCENSAWPGX");
        alphabetUpper.Add("AMIZRKFBQWGNXEUYOHTVJCPDSL");
        alphabetUpper.Add("OMEKCIVPRFSJHQNBWALXZDYGTU");
        alphabetUpper.Add("WNYVIKCJTQHRXBOSEDLZMAUPFG");




        //static circular
        alphabetLower.Add("limsgydvehaqocbjputrxnkfzw");
        alphabetLower.Add("qoempbunckfilvthjzdwxyrsag");
        alphabetLower.Add("hsbfavkzcldmeyuqrgpnxwojit");
        alphabetLower.Add("yvjxpgerhiuabfdtlqzsnkocmw");
        alphabetLower.Add("xtrfcbiamlhueqsjvynpzgowdk");
        alphabetLower.Add("zhgeunampcjwovtrxlyfqkbids");

        //static linear
        alphabetLower.Add("sqabpyjwlhrucgvnzdomefxikt");
        alphabetLower.Add("gkstmizyjnedqbafhvxwrcopul");
        alphabetLower.Add("knrzfebcovdmxythusplqgjiaw");
        alphabetLower.Add("nyrgobpecfzjstuwqkmldixvha");
        alphabetLower.Add("mgkduoyrixezbapntqslfwvhcj");
        alphabetLower.Add("fplomvytdecgaxsrnuzqhbkijw");

        //linear circular
        alphabetLower.Add("kdmiconubywpqjarxefghlsvtz");
        alphabetLower.Add("kjweypglmanvtdfcxiorqhubsz");
        alphabetLower.Add("opzdwrhuklxstqajvgbnymcife");
        alphabetLower.Add("iyjuqwngthbalzcorkmsxpfedv");
        alphabetLower.Add("hntleorvsygbawcjzpfimxudqk");
        alphabetLower.Add("meqyhxgukndsftwrbzpcvojali");






        /////Tzpographz 
        ///


        ///HARDCODE RANDOM satic circular
        alphabetUpper.Add("PRWXFEUOMJSBKHADTLGVYQIZCN");
        alphabetUpper.Add("VTDKECQYZMHFARNLGUJPOXBSWI");
        alphabetUpper.Add("JVQIMLORAKNWCUXBZPYFEDTGSH");
        alphabetUpper.Add("PAJLUIZBWROVXCQKEDGSMNHFTY");
        alphabetUpper.Add("HOVZXCDUJSQAYFTBMLKRGIWPNE");
        alphabetUpper.Add("WSQRDMTBFHULAOXIVGZKYCEPNJ");

        //satatic linear
        alphabetUpper.Add("UWTRNPILBACKZXSMDFVHJOYEQG");
        alphabetUpper.Add("PTAWCQZISXNJUYDORFMHLKBEVG");
        alphabetUpper.Add("SKJEICBAUFOMWGQTHXDPLZVYNR");
        alphabetUpper.Add("TXLCOHZFURQPKWJSBGEVAMIDNY");
        alphabetUpper.Add("HLKZIAQCRMBOVEXWJFNSDUTPGY");
        alphabetUpper.Add("XEBIZDNUMAPVSKRJHOWFQLGYCT");


        //linear circular
        alphabetUpper.Add("ZRSEOGJAPKVYHWQLXNUDIBFTCM");
        alphabetUpper.Add("VARSDXEILPFHUBOTJNCMKQZYWG");
        alphabetUpper.Add("ZQLFIAPWGBMSYNOXURJHETCKVD");
        alphabetUpper.Add("ELSDMZPVQFITGNBHRKOWXCYUAJ");
        alphabetUpper.Add("NYUZLHCODXKEABQMGTWPJRISFV");
        alphabetUpper.Add("JEAICZKWMSYXGRDLHNOFVBPTQU");




        //static circular
        alphabetLower.Add("jvtwydlsorzqebpahgckmunxfi");
        alphabetLower.Add("bfktrycqdxvgwuozipnhasmjel");
        alphabetLower.Add("plijrwusbxecgyvnfzatohdmqk");
        alphabetLower.Add("lgqvasbxpkhmzjionrwutcyedf");
        alphabetLower.Add("yasiejongtwhxrkfdlmzvcupbq");
        alphabetLower.Add("tohpqiueyvdkgmsfjwzcraxbln");

        //static linear
        alphabetLower.Add("elxfocrmzwnjadqvphyubskgit");
        alphabetLower.Add("xwkmdhjgvyabnruoqpcizstelf");
        alphabetLower.Add("ljictgkdvbzxwrnyqeahmsuofp");
        alphabetLower.Add("idczamqxwoyhepsugtvkljnfbr");
        alphabetLower.Add("wvqmafhzbckgjpyuetnxiodrls");
        alphabetLower.Add("prtnicwkvyfuojlqhgzbamesxd");

        //linear circular
        alphabetLower.Add("wpucyhtnkfsbrxeidzqmjlagov");
        alphabetLower.Add("rtvljfwdimuhokqxnaczsgpbye");
        alphabetLower.Add("viojlebdqzhaupcfgxrntmswky");
        alphabetLower.Add("tbngodyrxwmshlkzivejafpquc");
        alphabetLower.Add("naxluzyepkhtriodbqwgcsjmfv");
        alphabetLower.Add("uqitvxhlyzkowrspgmfcdeajnb");

        if (setMode.GetComponentInChildren<TMP_Text>().text == "normal")
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

        switch (SET) 
        {
            case 1:
              
                /////ARIAL 
                ///
                ///HARCODED CIRCULAR
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetUpper[18].Substring(0, (int)alphabetUpper[18].Length / 2), 48, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[19].Substring(0, (int)alphabetUpper[19].Length / 2), 48, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[18].Substring((int)alphabetUpper[18].Length / 2, (int)alphabetUpper[18].Length / 2), 48, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[19].Substring((int)alphabetUpper[19].Length / 2, (int)alphabetUpper[19].Length / 2), 48, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetUpper[20].Substring(0, (int)alphabetUpper[20].Length / 2), 40, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[21].Substring(0, (int)alphabetUpper[21].Length / 2), 40, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[20].Substring((int)alphabetUpper[20].Length / 2, (int)alphabetUpper[20].Length / 2), 40, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[21].Substring((int)alphabetUpper[21].Length / 2, (int)alphabetUpper[21].Length / 2), 40, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetUpper[22].Substring(0, (int)alphabetUpper[23].Length / 2), 32, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[23].Substring(0, (int)alphabetUpper[23].Length / 2), 32, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[22].Substring((int)alphabetUpper[22].Length / 2, (int)alphabetUpper[22].Length / 2), 32, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[23].Substring((int)alphabetUpper[23].Length / 2, (int)alphabetUpper[23].Length / 2), 32, taskTypeTwo, fontTypeTwo));
                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                ////HARCODED LOWWRCASE
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetLower[18].Substring(0, (int)alphabetLower[18].Length / 2), 48, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[19].Substring(0, (int)alphabetLower[19].Length / 2), 48, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[18].Substring((int)alphabetLower[18].Length / 2, (int)alphabetLower[18].Length / 2), 48, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[19].Substring((int)alphabetLower[19].Length / 2, (int)alphabetLower[19].Length / 2), 48, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetLower[20].Substring(0, (int)alphabetLower[20].Length / 2), 40, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[21].Substring(0, (int)alphabetLower[21].Length / 2), 40, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[20].Substring((int)alphabetLower[20].Length / 2, (int)alphabetLower[20].Length / 2), 40, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[21].Substring((int)alphabetLower[21].Length / 2, (int)alphabetLower[21].Length / 2), 40, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetLower[22].Substring(0, (int)alphabetLower[22].Length / 2), 32, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[23].Substring(0, (int)alphabetLower[23].Length / 2), 32, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[22].Substring((int)alphabetLower[22].Length / 2, (int)alphabetLower[22].Length / 2), 32, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[23].Substring((int)alphabetLower[23].Length / 2, (int)alphabetLower[23].Length / 2), 32, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));


                ///HARCODED LINEAR
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetUpper[24].Substring(0, (int)alphabetUpper[24].Length / 2), 48, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[25].Substring(0, (int)alphabetUpper[25].Length / 2), 48, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[24].Substring((int)alphabetUpper[24].Length / 2, (int)alphabetUpper[24].Length / 2), 48, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[25].Substring((int)alphabetUpper[25].Length / 2, (int)alphabetUpper[25].Length / 2), 48, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetUpper[26].Substring(0, (int)alphabetUpper[26].Length / 2), 40, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[27].Substring(0, (int)alphabetUpper[27].Length / 2), 40, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[26].Substring((int)alphabetUpper[26].Length / 2, (int)alphabetUpper[26].Length / 2), 40, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[27].Substring((int)alphabetUpper[27].Length / 2, (int)alphabetUpper[27].Length / 2), 40, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetUpper[28].Substring(0, (int)alphabetUpper[28].Length / 2), 32, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[29].Substring(0, (int)alphabetUpper[29].Length / 2), 32, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[28].Substring((int)alphabetUpper[28].Length / 2, (int)alphabetUpper[28].Length / 2), 32, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[29].Substring((int)alphabetUpper[29].Length / 2, (int)alphabetUpper[29].Length / 2), 32, taskTypeFour, fontTypeTwo));
                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                ////HARCODED LOWWRCASE
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetLower[24].Substring(0, (int)alphabetLower[24].Length / 2), 48, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[25].Substring(0, (int)alphabetLower[25].Length / 2), 48, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[24].Substring((int)alphabetLower[24].Length / 2, (int)alphabetLower[24].Length / 2), 48, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[25].Substring((int)alphabetLower[25].Length / 2, (int)alphabetLower[25].Length / 2), 48, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetLower[26].Substring(0, (int)alphabetLower[26].Length / 2), 40, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[27].Substring(0, (int)alphabetLower[27].Length / 2), 40, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[26].Substring((int)alphabetLower[26].Length / 2, (int)alphabetLower[26].Length / 2), 40, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[27].Substring((int)alphabetLower[27].Length / 2, (int)alphabetLower[27].Length / 2), 40, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetLower[28].Substring(0, (int)alphabetLower[28].Length / 2), 32, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[29].Substring(0, (int)alphabetLower[29].Length / 2), 32, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[28].Substring((int)alphabetLower[28].Length / 2, (int)alphabetLower[28].Length / 2), 32, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[29].Substring((int)alphabetLower[29].Length / 2, (int)alphabetLower[29].Length / 2), 32, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));


                ///HARCODED LINEARCICULAR///////
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetUpper[30].Substring(0, (int)alphabetUpper[30].Length / 2), 48, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[31].Substring(0, (int)alphabetUpper[31].Length / 2), 48, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[30].Substring((int)alphabetUpper[30].Length / 2, (int)alphabetUpper[30].Length / 2), 48, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[31].Substring((int)alphabetUpper[31].Length / 2, (int)alphabetUpper[31].Length / 2), 48, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetUpper[32].Substring(0, (int)alphabetUpper[32].Length / 2), 40, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[33].Substring(0, (int)alphabetUpper[33].Length / 2), 40, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[32].Substring((int)alphabetUpper[32].Length / 2, (int)alphabetUpper[32].Length / 2), 40, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[33].Substring((int)alphabetUpper[33].Length / 2, (int)alphabetUpper[33].Length / 2), 40, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetUpper[34].Substring(0, (int)alphabetUpper[34].Length / 2), 32, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[35].Substring(0, (int)alphabetUpper[35].Length / 2), 32, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[34].Substring((int)alphabetUpper[34].Length / 2, (int)alphabetUpper[34].Length / 2), 32, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[35].Substring((int)alphabetUpper[35].Length / 2, (int)alphabetUpper[35].Length / 2), 32, taskTypeSix, fontTypeTwo));
                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                ////HARCODED LOWWRCASE
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetLower[30].Substring(0, (int)alphabetLower[30].Length / 2), 48, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[31].Substring(0, (int)alphabetLower[31].Length / 2), 48, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[30].Substring((int)alphabetLower[30].Length / 2, (int)alphabetLower[30].Length / 2), 48, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[31].Substring((int)alphabetLower[31].Length / 2, (int)alphabetLower[31].Length / 2), 48, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetLower[32].Substring(0, (int)alphabetLower[32].Length / 2), 40, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[33].Substring(0, (int)alphabetLower[33].Length / 2), 40, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[32].Substring((int)alphabetLower[32].Length / 2, (int)alphabetLower[32].Length / 2), 40, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[33].Substring((int)alphabetLower[33].Length / 2, (int)alphabetLower[33].Length / 2), 40, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetLower[34].Substring(0, (int)alphabetLower[34].Length / 2), 32, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[35].Substring(0, (int)alphabetLower[35].Length / 2), 32, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[34].Substring((int)alphabetLower[34].Length / 2, (int)alphabetLower[34].Length / 2), 32, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[35].Substring((int)alphabetLower[35].Length / 2, (int)alphabetLower[35].Length / 2), 32, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));





                ///TIMES
                ///HARCODED CIRCULAR
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetUpper[0].Substring(0, (int)alphabetUpper[0].Length / 2), 48, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[1].Substring(0, (int)alphabetUpper[1].Length / 2), 48, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[0].Substring((int)alphabetUpper[0].Length / 2, (int)alphabetUpper[0].Length / 2), 48, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[1].Substring((int)alphabetUpper[1].Length / 2, (int)alphabetUpper[1].Length / 2), 48, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetUpper[2].Substring(0, (int)alphabetUpper[2].Length / 2), 40, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[3].Substring(0, (int)alphabetUpper[3].Length / 2), 40, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[2].Substring((int)alphabetUpper[2].Length / 2, (int)alphabetUpper[2].Length / 2), 40, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[3].Substring((int)alphabetUpper[3].Length / 2, (int)alphabetUpper[3].Length / 2), 40, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetUpper[4].Substring(0, (int)alphabetUpper[4].Length / 2), 32, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[5].Substring(0, (int)alphabetUpper[5].Length / 2), 32, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[4].Substring((int)alphabetUpper[4].Length / 2, (int)alphabetUpper[4].Length / 2), 32, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[5].Substring((int)alphabetUpper[5].Length / 2, (int)alphabetUpper[5].Length / 2), 32, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                ////HARCODED LOWWRCASE
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetLower[0].Substring(0, (int)alphabetLower[0].Length / 2), 48, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[1].Substring(0, (int)alphabetLower[1].Length / 2), 48, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[0].Substring((int)alphabetLower[0].Length / 2, (int)alphabetLower[0].Length / 2), 48, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[1].Substring((int)alphabetLower[1].Length / 2, (int)alphabetLower[1].Length / 2), 48, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetLower[2].Substring(0, (int)alphabetLower[2].Length / 2), 40, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[3].Substring(0, (int)alphabetLower[3].Length / 2), 40, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[2].Substring((int)alphabetLower[2].Length / 2, (int)alphabetLower[2].Length / 2), 40, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[3].Substring((int)alphabetLower[3].Length / 2, (int)alphabetLower[3].Length / 2), 40, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetLower[4].Substring(0, (int)alphabetLower[4].Length / 2), 32, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[5].Substring(0, (int)alphabetLower[5].Length / 2), 32, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[4].Substring((int)alphabetLower[4].Length / 2, (int)alphabetLower[4].Length / 2), 32, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[5].Substring((int)alphabetLower[5].Length / 2, (int)alphabetLower[5].Length / 2), 32, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));




                ///HARCODED LINEAR
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetUpper[6].Substring(0, (int)alphabetUpper[6].Length / 2), 48, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[7].Substring(0, (int)alphabetUpper[7].Length / 2), 48, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[6].Substring((int)alphabetUpper[6].Length / 2, (int)alphabetUpper[6].Length / 2), 48, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[7].Substring((int)alphabetUpper[7].Length / 2, (int)alphabetUpper[7].Length / 2), 48, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetUpper[8].Substring(0, (int)alphabetUpper[8].Length / 2), 40, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[9].Substring(0, (int)alphabetUpper[9].Length / 2), 40, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[8].Substring((int)alphabetUpper[8].Length / 2, (int)alphabetUpper[8].Length / 2), 40, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[9].Substring((int)alphabetUpper[9].Length / 2, (int)alphabetUpper[9].Length / 2), 40, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetUpper[10].Substring(0, (int)alphabetUpper[10].Length / 2), 32, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[11].Substring(0, (int)alphabetUpper[11].Length / 2), 32, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[10].Substring((int)alphabetUpper[10].Length / 2, (int)alphabetUpper[10].Length / 2), 32, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[11].Substring((int)alphabetUpper[11].Length / 2, (int)alphabetUpper[11].Length / 2), 32, taskTypeFour, fontTypeOne));
                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                ////HARCODED LOWWRCASE
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetLower[6].Substring(0, (int)alphabetLower[6].Length / 2), 48, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[7].Substring(0, (int)alphabetLower[7].Length / 2), 48, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[6].Substring((int)alphabetLower[6].Length / 2, (int)alphabetLower[6].Length / 2), 48, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[7].Substring((int)alphabetLower[7].Length / 2, (int)alphabetLower[7].Length / 2), 48, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetLower[8].Substring(0, (int)alphabetLower[8].Length / 2), 40, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[9].Substring(0, (int)alphabetLower[9].Length / 2), 40, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[8].Substring((int)alphabetLower[8].Length / 2, (int)alphabetLower[8].Length / 2), 40, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[9].Substring((int)alphabetLower[9].Length / 2, (int)alphabetLower[9].Length / 2), 40, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetLower[10].Substring(0, (int)alphabetLower[10].Length / 2), 32, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[11].Substring(0, (int)alphabetLower[11].Length / 2), 32, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[10].Substring((int)alphabetLower[10].Length / 2, (int)alphabetLower[10].Length / 2), 32, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[11].Substring((int)alphabetLower[11].Length / 2, (int)alphabetLower[11].Length / 2), 32, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));


                ///HARCODED LINEARCICULAR///////
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetUpper[12].Substring(0, (int)alphabetUpper[12].Length / 2), 48, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[13].Substring(0, (int)alphabetUpper[13].Length / 2), 48, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[12].Substring((int)alphabetUpper[12].Length / 2, (int)alphabetUpper[12].Length / 2), 48, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[13].Substring((int)alphabetUpper[13].Length / 2, (int)alphabetUpper[13].Length / 2), 48, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetUpper[14].Substring(0, (int)alphabetUpper[14].Length / 2), 40, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[15].Substring(0, (int)alphabetUpper[15].Length / 2), 40, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[14].Substring((int)alphabetUpper[14].Length / 2, (int)alphabetUpper[14].Length / 2), 40, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[15].Substring((int)alphabetUpper[15].Length / 2, (int)alphabetUpper[15].Length / 2), 40, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetUpper[16].Substring(0, (int)alphabetUpper[16].Length / 2), 32, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[17].Substring(0, (int)alphabetUpper[17].Length / 2), 32, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[16].Substring((int)alphabetUpper[16].Length / 2, (int)alphabetUpper[16].Length / 2), 32, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[17].Substring((int)alphabetUpper[17].Length / 2, (int)alphabetUpper[17].Length / 2), 32, taskTypeSix, fontTypeOne));
                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                ////HARCODED LOWWRCASE
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetLower[12].Substring(0, (int)alphabetLower[12].Length / 2), 48, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[13].Substring(0, (int)alphabetLower[13].Length / 2), 48, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[12].Substring((int)alphabetLower[12].Length / 2, (int)alphabetLower[12].Length / 2), 48, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[13].Substring((int)alphabetLower[13].Length / 2, (int)alphabetLower[13].Length / 2), 48, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetLower[14].Substring(0, (int)alphabetLower[14].Length / 2), 40, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[15].Substring(0, (int)alphabetLower[15].Length / 2), 40, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[14].Substring((int)alphabetLower[14].Length / 2, (int)alphabetLower[14].Length / 2), 40, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[15].Substring((int)alphabetLower[15].Length / 2, (int)alphabetLower[15].Length / 2), 40, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetLower[16].Substring(0, (int)alphabetLower[16].Length / 2), 32, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[17].Substring(0, (int)alphabetLower[17].Length / 2), 32, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[16].Substring((int)alphabetLower[16].Length / 2, (int)alphabetLower[16].Length / 2), 32, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[17].Substring((int)alphabetLower[17].Length / 2, (int)alphabetLower[17].Length / 2), 32, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                break;
            case 2:
               
                ///TIMES
                /////OTHER FONT TYPE                ///
                ///
                ///HARCODED LINEAR---STATIC
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetUpper[24].Substring(0, (int)alphabetUpper[24].Length / 2), 48, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[25].Substring(0, (int)alphabetUpper[25].Length / 2), 48, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[24].Substring((int)alphabetUpper[24].Length / 2, (int)alphabetUpper[24].Length / 2), 48, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[25].Substring((int)alphabetUpper[25].Length / 2, (int)alphabetUpper[25].Length / 2), 48, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetUpper[26].Substring(0, (int)alphabetUpper[26].Length / 2), 40, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[27].Substring(0, (int)alphabetUpper[27].Length / 2), 40, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[26].Substring((int)alphabetUpper[26].Length / 2, (int)alphabetUpper[26].Length / 2), 40, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[27].Substring((int)alphabetUpper[27].Length / 2, (int)alphabetUpper[27].Length / 2), 40, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetUpper[28].Substring(0, (int)alphabetUpper[28].Length / 2), 32, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[29].Substring(0, (int)alphabetUpper[29].Length / 2), 32, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[28].Substring((int)alphabetUpper[28].Length / 2, (int)alphabetUpper[28].Length / 2), 32, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[29].Substring((int)alphabetUpper[29].Length / 2, (int)alphabetUpper[29].Length / 2), 32, taskTypeFour, fontTypeTwo));
                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                ////HARCODED LOWWRCASE
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetLower[24].Substring(0, (int)alphabetLower[24].Length / 2), 48, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[25].Substring(0, (int)alphabetLower[25].Length / 2), 48, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[24].Substring((int)alphabetLower[24].Length / 2, (int)alphabetLower[24].Length / 2), 48, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[25].Substring((int)alphabetLower[25].Length / 2, (int)alphabetLower[25].Length / 2), 48, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetLower[26].Substring(0, (int)alphabetLower[26].Length / 2), 40, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[27].Substring(0, (int)alphabetLower[27].Length / 2), 40, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[26].Substring((int)alphabetLower[26].Length / 2, (int)alphabetLower[26].Length / 2), 40, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[27].Substring((int)alphabetLower[27].Length / 2, (int)alphabetLower[27].Length / 2), 40, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetLower[28].Substring(0, (int)alphabetLower[28].Length / 2), 32, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[29].Substring(0, (int)alphabetLower[29].Length / 2), 32, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[28].Substring((int)alphabetLower[28].Length / 2, (int)alphabetLower[28].Length / 2), 32, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[29].Substring((int)alphabetLower[29].Length / 2, (int)alphabetLower[29].Length / 2), 32, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));


                ///HARCODED LINEAR------CICULAR///////
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetUpper[30].Substring(0, (int)alphabetUpper[30].Length / 2), 48, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[31].Substring(0, (int)alphabetUpper[31].Length / 2), 48, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[30].Substring((int)alphabetUpper[30].Length / 2, (int)alphabetUpper[30].Length / 2), 48, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[31].Substring((int)alphabetUpper[31].Length / 2, (int)alphabetUpper[31].Length / 2), 48, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetUpper[32].Substring(0, (int)alphabetUpper[32].Length / 2), 40, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[33].Substring(0, (int)alphabetUpper[33].Length / 2), 40, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[32].Substring((int)alphabetUpper[32].Length / 2, (int)alphabetUpper[32].Length / 2), 40, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[33].Substring((int)alphabetUpper[33].Length / 2, (int)alphabetUpper[33].Length / 2), 40, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetUpper[34].Substring(0, (int)alphabetUpper[34].Length / 2), 32, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[35].Substring(0, (int)alphabetUpper[35].Length / 2), 32, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[34].Substring((int)alphabetUpper[34].Length / 2, (int)alphabetUpper[34].Length / 2), 32, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[35].Substring((int)alphabetUpper[35].Length / 2, (int)alphabetUpper[35].Length / 2), 32, taskTypeSix, fontTypeTwo));
                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                ////HARCODED LOWWRCASE
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetLower[30].Substring(0, (int)alphabetLower[30].Length / 2), 48, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[31].Substring(0, (int)alphabetLower[31].Length / 2), 48, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[30].Substring((int)alphabetLower[30].Length / 2, (int)alphabetLower[30].Length / 2), 48, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[31].Substring((int)alphabetLower[31].Length / 2, (int)alphabetLower[31].Length / 2), 48, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetLower[32].Substring(0, (int)alphabetLower[32].Length / 2), 40, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[33].Substring(0, (int)alphabetLower[33].Length / 2), 40, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[32].Substring((int)alphabetLower[32].Length / 2, (int)alphabetLower[32].Length / 2), 40, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[33].Substring((int)alphabetLower[33].Length / 2, (int)alphabetLower[33].Length / 2), 40, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetLower[34].Substring(0, (int)alphabetLower[34].Length / 2), 32, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[35].Substring(0, (int)alphabetLower[35].Length / 2), 32, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[34].Substring((int)alphabetLower[34].Length / 2, (int)alphabetLower[34].Length / 2), 32, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[35].Substring((int)alphabetLower[35].Length / 2, (int)alphabetLower[35].Length / 2), 32, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));


                ///HARCODED STATIC-----CIRCULAR
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetUpper[18].Substring(0, (int)alphabetUpper[18].Length / 2), 48, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[19].Substring(0, (int)alphabetUpper[19].Length / 2), 48, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[18].Substring((int)alphabetUpper[18].Length / 2, (int)alphabetUpper[18].Length / 2), 48, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[19].Substring((int)alphabetUpper[19].Length / 2, (int)alphabetUpper[19].Length / 2), 48, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetUpper[20].Substring(0, (int)alphabetUpper[20].Length / 2), 40, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[21].Substring(0, (int)alphabetUpper[21].Length / 2), 40, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[20].Substring((int)alphabetUpper[20].Length / 2, (int)alphabetUpper[20].Length / 2), 40, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[21].Substring((int)alphabetUpper[21].Length / 2, (int)alphabetUpper[21].Length / 2), 40, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetUpper[22].Substring(0, (int)alphabetUpper[23].Length / 2), 32, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[23].Substring(0, (int)alphabetUpper[23].Length / 2), 32, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[22].Substring((int)alphabetUpper[22].Length / 2, (int)alphabetUpper[22].Length / 2), 32, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[23].Substring((int)alphabetUpper[23].Length / 2, (int)alphabetUpper[23].Length / 2), 32, taskTypeTwo, fontTypeTwo));
                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                ////HARCODED LOWWRCASE
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetLower[18].Substring(0, (int)alphabetLower[18].Length / 2), 48, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[19].Substring(0, (int)alphabetLower[19].Length / 2), 48, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[18].Substring((int)alphabetLower[18].Length / 2, (int)alphabetLower[18].Length / 2), 48, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[19].Substring((int)alphabetLower[19].Length / 2, (int)alphabetLower[19].Length / 2), 48, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetLower[20].Substring(0, (int)alphabetLower[20].Length / 2), 40, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[21].Substring(0, (int)alphabetLower[21].Length / 2), 40, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[20].Substring((int)alphabetLower[20].Length / 2, (int)alphabetLower[20].Length / 2), 40, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[21].Substring((int)alphabetLower[21].Length / 2, (int)alphabetLower[21].Length / 2), 40, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetLower[22].Substring(0, (int)alphabetLower[22].Length / 2), 32, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[23].Substring(0, (int)alphabetLower[23].Length / 2), 32, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[22].Substring((int)alphabetLower[22].Length / 2, (int)alphabetLower[22].Length / 2), 32, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[23].Substring((int)alphabetLower[23].Length / 2, (int)alphabetLower[23].Length / 2), 32, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                ///TIMES
                ///HARCODED SATIC-----LINEAR
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetUpper[6].Substring(0, (int)alphabetUpper[6].Length / 2), 48, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[7].Substring(0, (int)alphabetUpper[7].Length / 2), 48, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[6].Substring((int)alphabetUpper[6].Length / 2, (int)alphabetUpper[6].Length / 2), 48, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[7].Substring((int)alphabetUpper[7].Length / 2, (int)alphabetUpper[7].Length / 2), 48, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetUpper[8].Substring(0, (int)alphabetUpper[8].Length / 2), 40, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[9].Substring(0, (int)alphabetUpper[9].Length / 2), 40, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[8].Substring((int)alphabetUpper[8].Length / 2, (int)alphabetUpper[8].Length / 2), 40, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[9].Substring((int)alphabetUpper[9].Length / 2, (int)alphabetUpper[9].Length / 2), 40, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetUpper[10].Substring(0, (int)alphabetUpper[10].Length / 2), 32, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[11].Substring(0, (int)alphabetUpper[11].Length / 2), 32, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[10].Substring((int)alphabetUpper[10].Length / 2, (int)alphabetUpper[10].Length / 2), 32, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[11].Substring((int)alphabetUpper[11].Length / 2, (int)alphabetUpper[11].Length / 2), 32, taskTypeFour, fontTypeOne));
                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                ////HARCODED LOWWRCASE
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetLower[6].Substring(0, (int)alphabetLower[6].Length / 2), 48, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[7].Substring(0, (int)alphabetLower[7].Length / 2), 48, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[6].Substring((int)alphabetLower[6].Length / 2, (int)alphabetLower[6].Length / 2), 48, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[7].Substring((int)alphabetLower[7].Length / 2, (int)alphabetLower[7].Length / 2), 48, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetLower[8].Substring(0, (int)alphabetLower[8].Length / 2), 40, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[9].Substring(0, (int)alphabetLower[9].Length / 2), 40, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[8].Substring((int)alphabetLower[8].Length / 2, (int)alphabetLower[8].Length / 2), 40, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[9].Substring((int)alphabetLower[9].Length / 2, (int)alphabetLower[9].Length / 2), 40, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetLower[10].Substring(0, (int)alphabetLower[10].Length / 2), 32, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[11].Substring(0, (int)alphabetLower[11].Length / 2), 32, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[10].Substring((int)alphabetLower[10].Length / 2, (int)alphabetLower[10].Length / 2), 32, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[11].Substring((int)alphabetLower[11].Length / 2, (int)alphabetLower[11].Length / 2), 32, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));


                ///HARCODED LINEAR------CICULAR///////
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetUpper[12].Substring(0, (int)alphabetUpper[12].Length / 2), 48, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[13].Substring(0, (int)alphabetUpper[13].Length / 2), 48, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[12].Substring((int)alphabetUpper[12].Length / 2, (int)alphabetUpper[12].Length / 2), 48, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[13].Substring((int)alphabetUpper[13].Length / 2, (int)alphabetUpper[13].Length / 2), 48, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetUpper[14].Substring(0, (int)alphabetUpper[14].Length / 2), 40, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[15].Substring(0, (int)alphabetUpper[15].Length / 2), 40, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[14].Substring((int)alphabetUpper[14].Length / 2, (int)alphabetUpper[14].Length / 2), 40, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[15].Substring((int)alphabetUpper[15].Length / 2, (int)alphabetUpper[15].Length / 2), 40, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetUpper[16].Substring(0, (int)alphabetUpper[16].Length / 2), 32, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[17].Substring(0, (int)alphabetUpper[17].Length / 2), 32, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[16].Substring((int)alphabetUpper[16].Length / 2, (int)alphabetUpper[16].Length / 2), 32, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[17].Substring((int)alphabetUpper[17].Length / 2, (int)alphabetUpper[17].Length / 2), 32, taskTypeSix, fontTypeOne));
                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                ////HARCODED LOWWRCASE
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetLower[12].Substring(0, (int)alphabetLower[12].Length / 2), 48, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[13].Substring(0, (int)alphabetLower[13].Length / 2), 48, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[12].Substring((int)alphabetLower[12].Length / 2, (int)alphabetLower[12].Length / 2), 48, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[13].Substring((int)alphabetLower[13].Length / 2, (int)alphabetLower[13].Length / 2), 48, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetLower[14].Substring(0, (int)alphabetLower[14].Length / 2), 40, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[15].Substring(0, (int)alphabetLower[15].Length / 2), 40, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[14].Substring((int)alphabetLower[14].Length / 2, (int)alphabetLower[14].Length / 2), 40, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[15].Substring((int)alphabetLower[15].Length / 2, (int)alphabetLower[15].Length / 2), 40, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetLower[16].Substring(0, (int)alphabetLower[16].Length / 2), 32, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[17].Substring(0, (int)alphabetLower[17].Length / 2), 32, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[16].Substring((int)alphabetLower[16].Length / 2, (int)alphabetLower[16].Length / 2), 32, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[17].Substring((int)alphabetLower[17].Length / 2, (int)alphabetLower[17].Length / 2), 32, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));


                ///HARCODED CIRCULAR
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetUpper[0].Substring(0, (int)alphabetUpper[0].Length / 2), 48, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[1].Substring(0, (int)alphabetUpper[1].Length / 2), 48, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[0].Substring((int)alphabetUpper[0].Length / 2, (int)alphabetUpper[0].Length / 2), 48, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[1].Substring((int)alphabetUpper[1].Length / 2, (int)alphabetUpper[1].Length / 2), 48, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetUpper[2].Substring(0, (int)alphabetUpper[2].Length / 2), 40, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[3].Substring(0, (int)alphabetUpper[3].Length / 2), 40, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[2].Substring((int)alphabetUpper[2].Length / 2, (int)alphabetUpper[2].Length / 2), 40, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[3].Substring((int)alphabetUpper[3].Length / 2, (int)alphabetUpper[3].Length / 2), 40, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetUpper[4].Substring(0, (int)alphabetUpper[4].Length / 2), 32, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[5].Substring(0, (int)alphabetUpper[5].Length / 2), 32, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[4].Substring((int)alphabetUpper[4].Length / 2, (int)alphabetUpper[4].Length / 2), 32, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[5].Substring((int)alphabetUpper[5].Length / 2, (int)alphabetUpper[5].Length / 2), 32, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                ////STATIC------CIRCULAR
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetLower[0].Substring(0, (int)alphabetLower[0].Length / 2), 48, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[1].Substring(0, (int)alphabetLower[1].Length / 2), 48, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[0].Substring((int)alphabetLower[0].Length / 2, (int)alphabetLower[0].Length / 2), 48, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[1].Substring((int)alphabetLower[1].Length / 2, (int)alphabetLower[1].Length / 2), 48, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetLower[2].Substring(0, (int)alphabetLower[2].Length / 2), 40, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[3].Substring(0, (int)alphabetLower[3].Length / 2), 40, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[2].Substring((int)alphabetLower[2].Length / 2, (int)alphabetLower[2].Length / 2), 40, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[3].Substring((int)alphabetLower[3].Length / 2, (int)alphabetLower[3].Length / 2), 40, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetLower[4].Substring(0, (int)alphabetLower[4].Length / 2), 32, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[5].Substring(0, (int)alphabetLower[5].Length / 2), 32, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[4].Substring((int)alphabetLower[4].Length / 2, (int)alphabetLower[4].Length / 2), 32, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[5].Substring((int)alphabetLower[5].Length / 2, (int)alphabetLower[5].Length / 2), 32, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                break;


            case 3:
             
                ///TIMES
                /////OTHER FONT TYPE                ///
                ///
                ///HARCODED LINEAR------CICULAR///////
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetUpper[30].Substring(0, (int)alphabetUpper[30].Length / 2), 48, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[31].Substring(0, (int)alphabetUpper[31].Length / 2), 48, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[30].Substring((int)alphabetUpper[30].Length / 2, (int)alphabetUpper[30].Length / 2), 48, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[31].Substring((int)alphabetUpper[31].Length / 2, (int)alphabetUpper[31].Length / 2), 48, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetUpper[32].Substring(0, (int)alphabetUpper[32].Length / 2), 40, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[33].Substring(0, (int)alphabetUpper[33].Length / 2), 40, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[32].Substring((int)alphabetUpper[32].Length / 2, (int)alphabetUpper[32].Length / 2), 40, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[33].Substring((int)alphabetUpper[33].Length / 2, (int)alphabetUpper[33].Length / 2), 40, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetUpper[34].Substring(0, (int)alphabetUpper[34].Length / 2), 32, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[35].Substring(0, (int)alphabetUpper[35].Length / 2), 32, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[34].Substring((int)alphabetUpper[34].Length / 2, (int)alphabetUpper[34].Length / 2), 32, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[35].Substring((int)alphabetUpper[35].Length / 2, (int)alphabetUpper[35].Length / 2), 32, taskTypeSix, fontTypeTwo));
                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                ////HARCODED LOWWRCASE
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetLower[30].Substring(0, (int)alphabetLower[30].Length / 2), 48, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[31].Substring(0, (int)alphabetLower[31].Length / 2), 48, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[30].Substring((int)alphabetLower[30].Length / 2, (int)alphabetLower[30].Length / 2), 48, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[31].Substring((int)alphabetLower[31].Length / 2, (int)alphabetLower[31].Length / 2), 48, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetLower[32].Substring(0, (int)alphabetLower[32].Length / 2), 40, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[33].Substring(0, (int)alphabetLower[33].Length / 2), 40, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[32].Substring((int)alphabetLower[32].Length / 2, (int)alphabetLower[32].Length / 2), 40, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[33].Substring((int)alphabetLower[33].Length / 2, (int)alphabetLower[33].Length / 2), 40, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetLower[34].Substring(0, (int)alphabetLower[34].Length / 2), 32, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[35].Substring(0, (int)alphabetLower[35].Length / 2), 32, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[34].Substring((int)alphabetLower[34].Length / 2, (int)alphabetLower[34].Length / 2), 32, taskTypeFive, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[35].Substring((int)alphabetLower[35].Length / 2, (int)alphabetLower[35].Length / 2), 32, taskTypeSix, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));


                ///HARCODED STATIC-----CIRCULAR
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetUpper[18].Substring(0, (int)alphabetUpper[18].Length / 2), 48, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[19].Substring(0, (int)alphabetUpper[19].Length / 2), 48, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[18].Substring((int)alphabetUpper[18].Length / 2, (int)alphabetUpper[18].Length / 2), 48, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[19].Substring((int)alphabetUpper[19].Length / 2, (int)alphabetUpper[19].Length / 2), 48, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetUpper[20].Substring(0, (int)alphabetUpper[20].Length / 2), 40, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[21].Substring(0, (int)alphabetUpper[21].Length / 2), 40, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[20].Substring((int)alphabetUpper[20].Length / 2, (int)alphabetUpper[20].Length / 2), 40, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[21].Substring((int)alphabetUpper[21].Length / 2, (int)alphabetUpper[21].Length / 2), 40, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetUpper[22].Substring(0, (int)alphabetUpper[23].Length / 2), 32, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[23].Substring(0, (int)alphabetUpper[23].Length / 2), 32, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[22].Substring((int)alphabetUpper[22].Length / 2, (int)alphabetUpper[22].Length / 2), 32, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[23].Substring((int)alphabetUpper[23].Length / 2, (int)alphabetUpper[23].Length / 2), 32, taskTypeTwo, fontTypeTwo));
                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                ////HARCODED LOWWRCASE
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetLower[18].Substring(0, (int)alphabetLower[18].Length / 2), 48, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[19].Substring(0, (int)alphabetLower[19].Length / 2), 48, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[18].Substring((int)alphabetLower[18].Length / 2, (int)alphabetLower[18].Length / 2), 48, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[19].Substring((int)alphabetLower[19].Length / 2, (int)alphabetLower[19].Length / 2), 48, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetLower[20].Substring(0, (int)alphabetLower[20].Length / 2), 40, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[21].Substring(0, (int)alphabetLower[21].Length / 2), 40, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[20].Substring((int)alphabetLower[20].Length / 2, (int)alphabetLower[20].Length / 2), 40, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[21].Substring((int)alphabetLower[21].Length / 2, (int)alphabetLower[21].Length / 2), 40, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetLower[22].Substring(0, (int)alphabetLower[22].Length / 2), 32, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[23].Substring(0, (int)alphabetLower[23].Length / 2), 32, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[22].Substring((int)alphabetLower[22].Length / 2, (int)alphabetLower[22].Length / 2), 32, taskTypeOne, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[23].Substring((int)alphabetLower[23].Length / 2, (int)alphabetLower[23].Length / 2), 32, taskTypeTwo, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));


                ///HARCODED LINEAR---STATIC
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetUpper[24].Substring(0, (int)alphabetUpper[24].Length / 2), 48, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[25].Substring(0, (int)alphabetUpper[25].Length / 2), 48, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[24].Substring((int)alphabetUpper[24].Length / 2, (int)alphabetUpper[24].Length / 2), 48, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[25].Substring((int)alphabetUpper[25].Length / 2, (int)alphabetUpper[25].Length / 2), 48, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetUpper[26].Substring(0, (int)alphabetUpper[26].Length / 2), 40, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[27].Substring(0, (int)alphabetUpper[27].Length / 2), 40, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[26].Substring((int)alphabetUpper[26].Length / 2, (int)alphabetUpper[26].Length / 2), 40, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[27].Substring((int)alphabetUpper[27].Length / 2, (int)alphabetUpper[27].Length / 2), 40, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetUpper[28].Substring(0, (int)alphabetUpper[28].Length / 2), 32, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[29].Substring(0, (int)alphabetUpper[29].Length / 2), 32, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetUpper[28].Substring((int)alphabetUpper[28].Length / 2, (int)alphabetUpper[28].Length / 2), 32, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetUpper[29].Substring((int)alphabetUpper[29].Length / 2, (int)alphabetUpper[29].Length / 2), 32, taskTypeFour, fontTypeTwo));
                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                ////HARCODED LOWWRCASE
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetLower[24].Substring(0, (int)alphabetLower[24].Length / 2), 48, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[25].Substring(0, (int)alphabetLower[25].Length / 2), 48, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[24].Substring((int)alphabetLower[24].Length / 2, (int)alphabetLower[24].Length / 2), 48, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[25].Substring((int)alphabetLower[25].Length / 2, (int)alphabetLower[25].Length / 2), 48, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetLower[26].Substring(0, (int)alphabetLower[26].Length / 2), 40, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[27].Substring(0, (int)alphabetLower[27].Length / 2), 40, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[26].Substring((int)alphabetLower[26].Length / 2, (int)alphabetLower[26].Length / 2), 40, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[27].Substring((int)alphabetLower[27].Length / 2, (int)alphabetLower[27].Length / 2), 40, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetLower[28].Substring(0, (int)alphabetLower[28].Length / 2), 32, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[29].Substring(0, (int)alphabetLower[29].Length / 2), 32, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));

                stringsOrder.Add(new StringCollection(alphabetLower[28].Substring((int)alphabetLower[28].Length / 2, (int)alphabetLower[28].Length / 2), 32, taskTypeThree, fontTypeTwo));
                stringsOrder.Add(new StringCollection(alphabetLower[29].Substring((int)alphabetLower[29].Length / 2, (int)alphabetLower[29].Length / 2), 32, taskTypeFour, fontTypeTwo));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeTwo));


                ///TIMES
                ///HARCODED LINEAR------CICULAR///////
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetUpper[12].Substring(0, (int)alphabetUpper[12].Length / 2), 48, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[13].Substring(0, (int)alphabetUpper[13].Length / 2), 48, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[12].Substring((int)alphabetUpper[12].Length / 2, (int)alphabetUpper[12].Length / 2), 48, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[13].Substring((int)alphabetUpper[13].Length / 2, (int)alphabetUpper[13].Length / 2), 48, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetUpper[14].Substring(0, (int)alphabetUpper[14].Length / 2), 40, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[15].Substring(0, (int)alphabetUpper[15].Length / 2), 40, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[14].Substring((int)alphabetUpper[14].Length / 2, (int)alphabetUpper[14].Length / 2), 40, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[15].Substring((int)alphabetUpper[15].Length / 2, (int)alphabetUpper[15].Length / 2), 40, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetUpper[16].Substring(0, (int)alphabetUpper[16].Length / 2), 32, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[17].Substring(0, (int)alphabetUpper[17].Length / 2), 32, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[16].Substring((int)alphabetUpper[16].Length / 2, (int)alphabetUpper[16].Length / 2), 32, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[17].Substring((int)alphabetUpper[17].Length / 2, (int)alphabetUpper[17].Length / 2), 32, taskTypeSix, fontTypeOne));
                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                ////HARCODED LOWWRCASE
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetLower[12].Substring(0, (int)alphabetLower[12].Length / 2), 48, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[13].Substring(0, (int)alphabetLower[13].Length / 2), 48, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[12].Substring((int)alphabetLower[12].Length / 2, (int)alphabetLower[12].Length / 2), 48, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[13].Substring((int)alphabetLower[13].Length / 2, (int)alphabetLower[13].Length / 2), 48, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetLower[14].Substring(0, (int)alphabetLower[14].Length / 2), 40, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[15].Substring(0, (int)alphabetLower[15].Length / 2), 40, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[14].Substring((int)alphabetLower[14].Length / 2, (int)alphabetLower[14].Length / 2), 40, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[15].Substring((int)alphabetLower[15].Length / 2, (int)alphabetLower[15].Length / 2), 40, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetLower[16].Substring(0, (int)alphabetLower[16].Length / 2), 32, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[17].Substring(0, (int)alphabetLower[17].Length / 2), 32, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[16].Substring((int)alphabetLower[16].Length / 2, (int)alphabetLower[16].Length / 2), 32, taskTypeFive, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[17].Substring((int)alphabetLower[17].Length / 2, (int)alphabetLower[17].Length / 2), 32, taskTypeSix, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));


                ///HARCODED STATIC----CIRCULAR
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetUpper[0].Substring(0, (int)alphabetUpper[0].Length / 2), 48, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[1].Substring(0, (int)alphabetUpper[1].Length / 2), 48, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[0].Substring((int)alphabetUpper[0].Length / 2, (int)alphabetUpper[0].Length / 2), 48, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[1].Substring((int)alphabetUpper[1].Length / 2, (int)alphabetUpper[1].Length / 2), 48, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetUpper[2].Substring(0, (int)alphabetUpper[2].Length / 2), 40, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[3].Substring(0, (int)alphabetUpper[3].Length / 2), 40, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[2].Substring((int)alphabetUpper[2].Length / 2, (int)alphabetUpper[2].Length / 2), 40, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[3].Substring((int)alphabetUpper[3].Length / 2, (int)alphabetUpper[3].Length / 2), 40, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetUpper[4].Substring(0, (int)alphabetUpper[4].Length / 2), 32, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[5].Substring(0, (int)alphabetUpper[5].Length / 2), 32, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[4].Substring((int)alphabetUpper[4].Length / 2, (int)alphabetUpper[4].Length / 2), 32, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[5].Substring((int)alphabetUpper[5].Length / 2, (int)alphabetUpper[5].Length / 2), 32, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                ////STATIC------CIRCULAR
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetLower[0].Substring(0, (int)alphabetLower[0].Length / 2), 48, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[1].Substring(0, (int)alphabetLower[1].Length / 2), 48, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[0].Substring((int)alphabetLower[0].Length / 2, (int)alphabetLower[0].Length / 2), 48, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[1].Substring((int)alphabetLower[1].Length / 2, (int)alphabetLower[1].Length / 2), 48, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetLower[2].Substring(0, (int)alphabetLower[2].Length / 2), 40, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[3].Substring(0, (int)alphabetLower[3].Length / 2), 40, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[2].Substring((int)alphabetLower[2].Length / 2, (int)alphabetLower[2].Length / 2), 40, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[3].Substring((int)alphabetLower[3].Length / 2, (int)alphabetLower[3].Length / 2), 40, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetLower[4].Substring(0, (int)alphabetLower[4].Length / 2), 32, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[5].Substring(0, (int)alphabetLower[5].Length / 2), 32, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[4].Substring((int)alphabetLower[4].Length / 2, (int)alphabetLower[4].Length / 2), 32, taskTypeOne, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[5].Substring((int)alphabetLower[5].Length / 2, (int)alphabetLower[5].Length / 2), 32, taskTypeTwo, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));


                ///HARCODED SATIC-----LINEAR
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetUpper[6].Substring(0, (int)alphabetUpper[6].Length / 2), 48, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[7].Substring(0, (int)alphabetUpper[7].Length / 2), 48, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[6].Substring((int)alphabetUpper[6].Length / 2, (int)alphabetUpper[6].Length / 2), 48, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[7].Substring((int)alphabetUpper[7].Length / 2, (int)alphabetUpper[7].Length / 2), 48, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetUpper[8].Substring(0, (int)alphabetUpper[8].Length / 2), 40, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[9].Substring(0, (int)alphabetUpper[9].Length / 2), 40, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[8].Substring((int)alphabetUpper[8].Length / 2, (int)alphabetUpper[8].Length / 2), 40, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[9].Substring((int)alphabetUpper[9].Length / 2, (int)alphabetUpper[9].Length / 2), 40, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetUpper[10].Substring(0, (int)alphabetUpper[10].Length / 2), 32, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[11].Substring(0, (int)alphabetUpper[11].Length / 2), 32, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetUpper[10].Substring((int)alphabetUpper[10].Length / 2, (int)alphabetUpper[10].Length / 2), 32, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetUpper[11].Substring((int)alphabetUpper[11].Length / 2, (int)alphabetUpper[11].Length / 2), 32, taskTypeFour, fontTypeOne));
                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                ////HARCODED LOWWRCASE
                ///Fontsize 8
                stringsOrder.Add(new StringCollection(alphabetLower[6].Substring(0, (int)alphabetLower[6].Length / 2), 48, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[7].Substring(0, (int)alphabetLower[7].Length / 2), 48, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[6].Substring((int)alphabetLower[6].Length / 2, (int)alphabetLower[6].Length / 2), 48, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[7].Substring((int)alphabetLower[7].Length / 2, (int)alphabetLower[7].Length / 2), 48, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 10
                stringsOrder.Add(new StringCollection(alphabetLower[8].Substring(0, (int)alphabetLower[8].Length / 2), 40, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[9].Substring(0, (int)alphabetLower[9].Length / 2), 40, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[8].Substring((int)alphabetLower[8].Length / 2, (int)alphabetLower[8].Length / 2), 40, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[9].Substring((int)alphabetLower[9].Length / 2, (int)alphabetLower[9].Length / 2), 40, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                //FOntsize 12
                stringsOrder.Add(new StringCollection(alphabetLower[10].Substring(0, (int)alphabetLower[10].Length / 2), 32, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[11].Substring(0, (int)alphabetLower[11].Length / 2), 32, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));

                stringsOrder.Add(new StringCollection(alphabetLower[10].Substring((int)alphabetLower[10].Length / 2, (int)alphabetLower[10].Length / 2), 32, taskTypeThree, fontTypeOne));
                stringsOrder.Add(new StringCollection(alphabetLower[11].Substring((int)alphabetLower[11].Length / 2, (int)alphabetLower[11].Length / 2), 32, taskTypeFour, fontTypeOne));

                stringsOrder.Add(new StringCollection(QMESSAGE, 52, "char_question", fontTypeOne));


                break;
        }

        
    }
}
