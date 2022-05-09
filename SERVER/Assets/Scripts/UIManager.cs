using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    public Button addUser_btn, charStatic_btn, imageStatic_btn, imagePractice_btn, charPractice_btn, phrases_btn, minimum_btn,confort_btn;
    public TMP_Text currUser_txt;
    public TMP_InputField inputField;
    public ServerDiscovery serverDiscovery;
    string user;

    [Header("NewApproach")]
    public TaskControl charTaskControl;
    public ImageControl imageTaskControl;
    public PhasesControl phrasesControl; 
    public GameObject _MENU_, _CHARTASK_, _IMAGETASK_,_PHRASESTASk_;
    public TaskAssets taskAssets;
    public UDPSender udpSender; 
    private void Awake()
    {
        if (PlayerPrefs.GetString("UserID") == "")
        {
            PlayerPrefs.SetString("UserID","test");

        }
    }
    void Start()
    {

        //Buttons and listeners
        //practice 
        //charPractice_btn.onClick.AddListener(delegate { ChangeScene("charPractice"); });
        //imagePractice_btn.onClick.AddListener(delegate { ChangeScene("imagePractice"); });
        //Tasks
        //charStatic_btn.onClick.AddListener(delegate { ChangeScene("charStatic"); });
        //charDyn_btn.onClick.AddListener(delegate { ChangeScene("charDynamic"); });
        //imageStatic_btn.onClick.AddListener(delegate { ChangeScene("imageStatic"); });
        //imageDynamic_btn.onClick.AddListener(delegate { ChangeScene("imageDynamic"); });


        ///NEW APPROACH
        imagePractice_btn.onClick.AddListener(delegate { ChangeScene("image_static_practice"); });
        imageStatic_btn.onClick.AddListener(delegate { ChangeScene("image_static"); });
        //Tasks
        charPractice_btn.onClick.AddListener(delegate { ChangeScene("char_static_practice"); });
        charStatic_btn.onClick.AddListener(delegate { ChangeScene("char_static"); });

        //PHASES
        phrases_btn.onClick.AddListener(delegate { ChangeScene("phra_reading"); });
        minimum_btn.onClick.AddListener(delegate { ChangeScene("phra_minimum"); });
        confort_btn.onClick.AddListener(delegate { ChangeScene("phra_confort"); });

        addUser_btn.onClick.AddListener(SaveUsrName);
        user= PlayerPrefs.GetString("UserID");
        inputField.onEndEdit.AddListener(GetUserInput);
        currUser_txt.text = "Current user: " + user;

    }

    private void HandleFontSize(bool arg0)
    {
        Debug.Log(arg0);
    }

    private void GetUserInput(string arg0)
    {
       
        PlayerPrefs.SetString("UserID", arg0);

    }

    private void SaveUsrName()
    {
        currUser_txt.text = "Current user: " + PlayerPrefs.GetString("UserID"); ;
        udpSender.sendString(PlayerPrefs.GetString("UserID"));
    }

    private void ChangeScene(string sceneName)
    {
      ////serverDiscovery.gameObject.SetActive(false);
        //SceneManager.LoadScene(sceneName);
        _MENU_.SetActive(false);
        //NEwApproach
        string[] splitString = sceneName.Split(char.Parse("_"));
        switch (splitString[0])
        {
            case "char":
                _CHARTASK_.SetActive(true);
                //init
                charTaskControl.taskType =splitString[0]+"_"+splitString[1];
                charTaskControl.isPractice = false;
                charTaskControl.studyStrings = taskAssets.realTaskStrings;
                charTaskControl.fontSizes = taskAssets.fontSizesList;
                charTaskControl.alphabet = taskAssets.alphabet;

                

                if (splitString.Length==3)
                {
                    charTaskControl.isPractice = true;
                    charTaskControl.studyStrings = taskAssets.practiceTaskStrings;
                }
                
                ///START
                //charTaskControl.StartCharTask();

                break;
            case "image":
                _IMAGETASK_.SetActive(true);

                //init
                imageTaskControl.taskType= splitString[0] + "_" + splitString[1];
                imageTaskControl.isPractice = false;
                imageTaskControl.ImageSetOne = taskAssets.setOne;
                imageTaskControl.ImageSetTwo = taskAssets.setTwo;
                imageTaskControl.ImageSetThree = taskAssets.setThree;
                imageTaskControl.ImageSetFour = taskAssets.setFour;

                imageTaskControl.setBtn1.gameObject.SetActive(true);
                imageTaskControl.setBtn2.gameObject.SetActive(true);
                imageTaskControl.setBtn3.gameObject.SetActive(true);
                imageTaskControl.setBtn4.gameObject.SetActive(true);

            /*    if (splitString[1] == "static")
                {
                    imageTaskControl.setBtn3.gameObject.SetActive(false);
                    imageTaskControl.setBtn4.gameObject.SetActive(false);
                }
                else
                {
                    imageTaskControl.setBtn1.gameObject.SetActive(false);
                    imageTaskControl.setBtn2.gameObject.SetActive(false);
                }*/
                


                if (splitString.Length == 3)
                {
                    imageTaskControl.isPractice = true;
                    imageTaskControl.ImageSetOne = taskAssets.setPractice;

                    imageTaskControl.setBtn1.gameObject.SetActive(true);
                    imageTaskControl.setBtn2.gameObject.SetActive(false);
                    imageTaskControl.setBtn3.gameObject.SetActive(false);
                    imageTaskControl.setBtn4.gameObject.SetActive(false);

                }

                //imageTaskControl.StartImageTask();
                break;
            case "phra":
                _PHRASESTASk_.SetActive(true);
                switch (splitString[1])
                {
                    case "reading":
                        break;
                    case "minimum":
                        break;
                    case "confort":
                        break; 
                }
                phrasesControl.PhrasesTask = splitString[1];


                break;



        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GetRemoteIP(string data)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(CustomCatcher(data));

    }

    public IEnumerator CustomCatcher(string data)
    {
        Debug.Log("REMOTE IP:"+data + "SAVED");
        PlayerPrefs.SetString("RemoteIP", data);
        udpSender.init();
        yield return null;
    }

}
