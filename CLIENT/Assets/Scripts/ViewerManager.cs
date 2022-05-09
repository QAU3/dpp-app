using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public class ViewerManager : MonoBehaviour
{


    [Header("Displays")]
    public TMP_Text  display;
    public TMP_Text text_clone;
    public RawImage imageDisplay, rawImage;
    public GameObject imageContainer, minimuTaskContainer;
    

    [Header("Tasks")]
    public GameObject MINIMUM_TASK;
    public GameObject CHAR_TASK, IMAGE_TASK;
    public TMP_Text fontDisplay, fontDisplay2;

    /// <summary>
    /// Scale use to bethe title for scales
    /// </summary>
    public TMP_Text scaleTitleUnoChar, scaleTitleDosChar, titleForScaleUno, titleForScaleDos;
    public TMP_Text scaleTitleUnoImg, scaleTitleDosImg, scaleTitleTresImg, scaleTitleCuatroImg;


    public UPDSend udpSend;


    [Header("Assets")]
    public Texture2D[] ImageSetPractice;
    public Texture2D [] ImageSetOne, ImageSetTwo, ImageSetThree, ImageSetFour;
    public TMP_FontAsset[] fontAsset;

    [Header("Components")]
    public GameObject linearAnimations;
    public GameObject linear2Circular;


    /// <summary>
    /// New Variables
    /// Circular animation
    /// 
    /// </summary>
    [Header("Animation confg")]
    public GameObject pivot;
    public float radious, angularSpeed;
    public int stepSize;
    public GameObject A, B;

    [Header("Interface Control")]
    public Button[] sizeBtns;
    

    /// Private vars
    ///
    public CSVLogger csvLogger;
    public StringMessage stringMessage;
    public ImageMessage imageMessage;

    private GameObject CURRENT_TASK;
    private Vector3 uintyWorldPos, pivotWorldPos, displayWorldPos;
    private float x, y;
    private float fontSize, imageSize;
    private string mediaType, media, mediaLabel, currSet, userLabel;
    private Texture2D[] currImageSet;
    private string USERID = "UNKNOWN";
    private string Q_A = "";
    float pixelsPerUnit, n_pixelSteps, angle,savedSpeed, linearSpeed;
    string animType, animTypeReal, prev, curr;
    string message_type;
    List<string> stringSequence; 


    private List<GameObject> tag_targets = new List<GameObject>();
    
    void Start()
    {

        stringSequence = new List<string>();
        string[] columns = new string[] {"USERID", "mediaType", "media", "fontSize", "imageSize", "mediaLabel", "userLabel", "currSet","Q_A","time_stamp"};

        csvLogger = new CSVLogger(columns);
        stringMessage = new StringMessage("test",20f, null);


        uintyWorldPos = Camera.main.WorldToScreenPoint(new Vector3(display.transform.position.x + 1, 0, 0));
        pivotWorldPos = Camera.main.WorldToScreenPoint(new Vector3(pivot.transform.position.x, 0, 0));

        pixelsPerUnit = (uintyWorldPos.x - pivotWorldPos.x);
        n_pixelSteps = (1 / pixelsPerUnit) * stepSize;
        radious = n_pixelSteps;

        prev = "";
        savedSpeed = angularSpeed;
        angularSpeed = 0;
      AddDescendantsWithTag(transform, "Botones", tag_targets);

        foreach(GameObject obj in tag_targets)
        {
            obj.transform.GetComponent<Button>().onClick.AddListener(GetButtonString);
        }
        CHAR_TASK.SetActive(true);
        CHAR_TASK.transform.GetChild(0).gameObject.SetActive(false);


       
        PlayerPrefs.SetFloat("angularSpeed", angularSpeed);
        PlayerPrefs.SetFloat("radious", radious);
        
    }


    public IEnumerator Pause()
    {
        yield return  new WaitForSecondsRealtime(2);
        //Time.timeScale = 0;
        angularSpeed = 0;
    }

    public IEnumerator Play()
    {
        yield return new WaitForSecondsRealtime(2);
        //Time.timeScale = 1;
        angularSpeed = savedSpeed;
      
    }
    public void ResetTaskState()
    {

        CHAR_TASK.SetActive(false);
        CHAR_TASK.transform.GetChild(0).gameObject.SetActive(true);
        CHAR_TASK.transform.GetChild(1).gameObject.SetActive(false);

        IMAGE_TASK.SetActive(false);
        IMAGE_TASK.transform.GetChild(0).gameObject.SetActive(true);
        IMAGE_TASK.transform.GetChild(1).gameObject.SetActive(false);


        MINIMUM_TASK.SetActive(false);
        MINIMUM_TASK.transform.GetChild(0).gameObject.SetActive(true);

        fontDisplay.gameObject.SetActive(true);
        ResetButtonsColor();
        text_clone.text = "";
        text_clone.gameObject.SetActive(false);

        linearAnimations.SetActive(false);
        linear2Circular.SetActive(false);
        imageContainer.SetActive(false);
       

        stringMessage.Type = null;

        foreach (Button b in sizeBtns)
        {
            b.enabled = true;
        }
    }

   public void ResetButtonsColor()
    {
        foreach (GameObject obj in tag_targets)
        {
            obj.transform.GetComponent<Image>().color = Color.white;
        }

    }

   public string GetAnsFromScale(Transform parent)
    {
        string value = "";

        foreach(Transform child in parent)
        {
            if (child.GetComponent<Image>() != null)
            {
                    if (child.GetComponent<Image>().color == Color.green)
                    {

                        value += child.GetChild(0).GetComponent<TMP_Text>().text;
                }

            }
        }
        return value;
    }

    private void GetButtonString()
    {
        var obj = EventSystem.current.currentSelectedGameObject.transform;
        string value = obj.GetComponentInChildren<TMP_Text>().text;

        string question= EventSystem.current.currentSelectedGameObject.transform.parent.name;
        string task = EventSystem.current.currentSelectedGameObject.transform.parent.transform.parent.name;
       obj.GetComponent<Image>().color = Color.green;
        var parent = EventSystem.current.currentSelectedGameObject.transform.parent.transform;

        if (obj.name == "END_Q")
        {

            Q_A += "Q2_"+parent.GetChild(1).transform.GetChild(0).GetComponent<TMP_Text>().text+GetAnsFromScale(parent.GetChild(1).transform.GetChild(3).transform) +",";
            Q_A += "Q2_" + parent.GetChild(2).transform.GetChild(0).GetComponent<TMP_Text>().text + GetAnsFromScale(parent.GetChild(2).transform.GetChild(3).transform)+",";
           // Q_A += "Q2_" + parent.GetChild(3).transform.GetChild(0).GetComponent<TMP_Text>().text + GetAnsFromScale(parent.GetChild(3).transform.GetChild(3).transform) + ",";


            udpSend.sendString(mediaType + ";" + Q_A);
            SaveData();
            CURRENT_TASK.SetActive(false);
            return;
        }

        if (value == "Next")
        {

            Q_A += task + "_" + question + "_" + GetAnsFromScale(parent).Replace("Next","") + ",";

            CURRENT_TASK.transform.GetChild(0).gameObject.SetActive(false);
            CURRENT_TASK.transform.GetChild(1).gameObject.SetActive(true);


            if (question == "Q2")
            {
                EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);
            }
            //ResetButtonsColor();
            udpSend.sendString(mediaType + ";" + Q_A);
            SaveData();

        }

        if (value=="Static" || value == "Circular" || value == "Linear" || value == "Same")
        {
            ResetButtonsColor();
            obj.GetComponent<Image>().color = Color.green;

       
        }

        if (value == "1" || value == "2" || value == "3" || value == "4" || value == "5")
        {

            var tempObj = EventSystem.current.currentSelectedGameObject.transform.parent;
            foreach(Transform child in tempObj.transform)
            {
               if(child.GetComponent<Image>().color== Color.green)
                {
                    
                    child.GetComponent<Image>().color = Color.white;
                }
            }

            var grandParent=EventSystem.current.currentSelectedGameObject.transform.parent.transform.parent.transform.GetChild(0).GetComponent<TMP_Text>();

            if (Q_A.Contains(task+"_" + grandParent.text + "_" + question + "_"+value+","))
            {
              
               
                return;
            }
            else
            {
                obj.GetComponent<Image>().color = Color.green;
                
             
            }


        }

        if (value == "-")
        {
            if (fontSize > 0)
            {
                fontSize--;
                display.fontSize = fontSize;
                text_clone.fontSize = fontSize;

                if(animType == "linear")
                {
                    stringMessage.Text = media;
                    stringMessage.FontSize=fontSize;
                    gameObject.SendMessage("SetStringSize", stringMessage);
                }

                if (question == "minimum")
                {
                    
                    fontDisplay.text = "Font size: " + fontSize;
                    fontDisplay2.text = "Font size: " + fontSize;
                }
                else
                {
                    fontDisplay2.text = "Font size: " + fontSize;

                }
                obj.GetComponent<Image>().color = Color.white;


            }
        }

        if (value == "+")
        {

            fontSize++;
            display.fontSize = fontSize;
            text_clone.fontSize = fontSize;

            if (animType == "linear")
            {
                stringMessage.Text = media;
                stringMessage.FontSize = fontSize;
                gameObject.SendMessage("SetStringSize", stringMessage);
            }

            if (question == "minimum")
            {
                fontDisplay.text = "Font size: " + fontSize;
                fontDisplay2.text = "Font size: " + fontSize;
            }
            else
            {
                fontDisplay2.text = "Font size: " + fontSize;

            }
            obj.GetComponent<Image>().color = Color.white;


        }

        if (value == "Confirm")
        {
           
            Q_A += task + "_" + question + "_" + value + ","+fontSize + ",";


            if (question == "minimum")
            {
                sizeBtns[0].enabled = false;
                sizeBtns[1].enabled = false;
                sizeBtns[4].enabled = false;



            }
            else
            {
                sizeBtns[2].enabled = false;
                sizeBtns[3].enabled = false;
                sizeBtns[5].enabled = false;

            }


            udpSend.sendString(mediaType + ";" + Q_A);
            SaveData();
        }
       
    
    }

    private void AddDescendantsWithTag(Transform parent, string tag, List<GameObject> list)
    {
        foreach (Transform child in parent)
        {
            if (child.gameObject.tag == tag)
            {
                list.Add(child.gameObject);
            }
            AddDescendantsWithTag(child, tag, list);
        }
    }






    
    // Update is called once per frame
    void FixedUpdate()
    {
        
        StartAnimation();
        
    }
   

    public void SaveData()
    {
        try
        {
            csvLogger.AddEntry(new string[] {
                USERID,
                mediaType,
                media,
                fontSize.ToString(),
                imageSize.ToString(),
                mediaLabel ,
                userLabel ,
                currSet,
                Q_A,
                GetTimestamp(DateTime.Now)
        });

            csvLogger.Save(Application.persistentDataPath + "/" + USERID + ".csv");


        }
        catch (Exception ex)
        {
            print(ex.ToString());
        }
    }
  public static String GetTimestamp(DateTime value)
    {
        return value.ToString("MMddHHmmss");
    }

    public void StartAnimation()
    {

                x = pivot.transform.position.x + Mathf.Cos(angle) * radious;
                y = pivot.transform.position.y + Mathf.Sin(angle) * radious;
                display.transform.position = new Vector2(x, y);
                imageContainer.transform.position = new Vector2(x, y);
                angle += (Mathf.Deg2Rad * angularSpeed) * Time.fixedDeltaTime;

                if (angle >= 360) { angle = 0; }
        
    }

    public void SetValues(NetworkMessage message)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(ThisWillBeExecutedOnTheMainThread(message));

    }

    public void SetLabelsInImageQuestions(string previousStreamedValue)
    {
        Debug.Log(previousStreamedValue);
        switch (previousStreamedValue)
        {
            case "static-circular":
                scaleTitleUnoImg.text = "Static";
                scaleTitleDosImg.text = "Circular";

               
                scaleTitleTresImg.text = "Static";
                scaleTitleCuatroImg.text = "Circular";
                break;
             case "static-linear":
          
                scaleTitleUnoImg.text = "Static";
                scaleTitleDosImg.text = "Linear";

                scaleTitleTresImg.text = "Static";
                scaleTitleCuatroImg.text = "Linear";

           
                break;
            case "circular-linear":
                scaleTitleUnoImg.text = "Circular";
                scaleTitleDosImg.text = "Linear";

                scaleTitleTresImg.text = "Circular";
                scaleTitleCuatroImg.text = "Linear";

                break;

            case "circular-static":
                scaleTitleUnoImg.text = "Static";
                scaleTitleDosImg.text = "Circular";


                scaleTitleTresImg.text = "Static";
                scaleTitleCuatroImg.text = "Circular";

                break;
            case "linear-static":
                scaleTitleUnoImg.text = "Static";
                scaleTitleDosImg.text = "Linear";

                scaleTitleTresImg.text = "Static";
                scaleTitleCuatroImg.text = "Linear";


                break;
            case "linear-circular":
                scaleTitleUnoImg.text = "Circular";
                scaleTitleDosImg.text = "Linear";

                scaleTitleTresImg.text = "Circular";
                scaleTitleCuatroImg.text = "Linear";

                break;


        }
    }

    public void SetLabelsInCharQuestions(string previousStreamedValue)
    {

        switch (previousStreamedValue)
        {
            case "static":
                scaleTitleUnoChar.text = "Static";
                scaleTitleDosChar.text = CapitalizeString(stringSequence[stringSequence.Count - 3]);
                break;
            case "circular":
                scaleTitleUnoChar.text = "Circular";
                scaleTitleDosChar.text = CapitalizeString(stringSequence[stringSequence.Count - 3]);
                break;
            case "linear":
                scaleTitleUnoChar.text = "Linear";
                scaleTitleDosChar.text = CapitalizeString(stringSequence[stringSequence.Count - 3]);
                break;
        }


        titleForScaleUno.text= CapitalizeString(stringSequence[stringSequence.Count - 3]);
        titleForScaleDos.text=CapitalizeString(prev);
    }


    public string CapitalizeString(string str)
    {
        return char.ToUpper(str[0]) + str.Substring(1);
    }

    public int GetFontIndex(string remoteIndex)
    {
        int index= remoteIndex == "arial" ? 0 : 1;
        Debug.Log(remoteIndex);
        Debug.Log(index);
        return index;
        
    }


    public int GetFontAssetIndex(string fontType)
    {
        if (fontType == "arial")
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }
    public IEnumerator ThisWillBeExecutedOnTheMainThread(NetworkMessage message)
    {
        

        ResetTaskState();
        //SAVING AND CLEANING
        userLabel = "Auto Save()";
        SaveData();
        mediaType ="";

        message_type = "";
        Q_A = "";
        userLabel = "";
        angularSpeed = 0;
        linearSpeed = 0;
        minimuTaskContainer.SetActive(false);
        prev = curr;
        curr = message.AnimationType;
        mediaType = message.MediaType;
        USERID = message.Userid;
        switch (message.MediaType)
        {
            case "char":

               CURRENT_TASK = CHAR_TASK;
                message_type = message.MediaType;

               display.gameObject.SetActive(true);
               imageContainer.gameObject.SetActive(false);
               minimuTaskContainer.SetActive(false);
               fontSize = message.FontSize;
                media = message.Media;
                display.text = media;
                display.fontSize = fontSize;
                animType = message.AnimationType;
                display.font =fontAsset[GetFontAssetIndex(message.FontType)];


                stringSequence.Add(message.AnimationType);

                switch (message.AnimationType)
                {

                    case "static":
                        angularSpeed = 0;

                        break;
                    case "linear":
                        display.gameObject.SetActive(false);
                        stringMessage = new StringMessage(message.Media, message.FontSize, null,message.FontType);
                        linearAnimations.SetActive(true);
                        gameObject.SendMessage("SetString", stringMessage);
                        break; 
                    case "circular":
                        angularSpeed = savedSpeed;
                        break;
                    default:
                        CURRENT_TASK.SetActive(true);
                        SetLabelsInCharQuestions(prev);
                        break; 
                }



                break;
            case "reading":

                mediaType ="phra";
                CURRENT_TASK = CHAR_TASK;
                display.gameObject.SetActive(true);
                imageContainer.gameObject.SetActive(false);



                fontSize = message.FontSize;
                media = message.Media;
                display.text = media;
                display.fontSize = fontSize;
                //angularSpeed = 0;

                fontDisplay.text = $"Font size:{fontSize}";
                fontDisplay2.text = $"Font size:{fontSize}";
                display.font = fontAsset[GetFontAssetIndex(message.FontType)];
                stringSequence.Add(message.AnimationType);

                animType = message.AnimationType;
                switch (message.AnimationType)
                {

                    case "static":
                        angularSpeed = 0;

                        break;
                    case "linear":
                        display.gameObject.SetActive(false);
                        stringMessage = new StringMessage(message.Media, message.FontSize, "reading", message.FontType);
                        linearAnimations.SetActive(true);
                        gameObject.SendMessage("SetString", stringMessage);
                        break;
                    case "circular":
                        angularSpeed = savedSpeed;
                        break;
                    default:
                        CURRENT_TASK.SetActive(true);
                        SetLabelsInCharQuestions(prev);
                        break;
                }


                break;


            case "minimum":
                mediaType = "phra";

                CURRENT_TASK = MINIMUM_TASK;
                    CURRENT_TASK.SetActive(true);
                display.gameObject.SetActive(true);
                imageContainer.gameObject.SetActive(false);

                display.font = fontAsset[GetFontAssetIndex(message.FontType)];

                fontSize = message.FontSize;
                media = message.Media;
                display.text = media;
                display.fontSize = fontSize;
                //angularSpeed = 0;

                fontDisplay.text = $"Font size:{fontSize}";
                fontDisplay2.text = $"Font size:{fontSize}";


                animType = message.AnimationType;

                switch (message.AnimationType)
                {

                    case "static":
                        angularSpeed = 0;

                        break;
                    case "linear":
                        display.gameObject.SetActive(false);
                        stringMessage = new StringMessage(message.Media, message.FontSize,null, message.FontType);
                        linearAnimations.SetActive(true);
                        gameObject.SendMessage("SetString", stringMessage);
                        break;
                    case "circular":
                        angularSpeed = savedSpeed;
                        break;
                    default:
                        CURRENT_TASK.SetActive(true);
                        break;
                }

                break; 
            case "image":

                CURRENT_TASK = IMAGE_TASK;
                message_type = message.AnimationType;


                display.gameObject.SetActive(false);
                minimuTaskContainer.SetActive(false);

                imageContainer.SetActive(true);
                media = message.Media;
                mediaLabel = "splitArray[2]";
                //currSet = message.Set;
                currImageSet = ImageSetOne;
                imageSize = 800;
                imageDisplay.GetComponent<RawImage>().texture = currImageSet[int.Parse(media)];

                
                switch (message.AnimationType)
                {
                    case "static-circular":
                        linearAnimations.SetActive(false);
                        linear2Circular.SetActive(false);

                        
                        yield return StartCoroutine(Pause());
                        yield return StartCoroutine(Play());
                        yield return StartCoroutine(Pause());
                        yield return StartCoroutine(Play());
                        yield return StartCoroutine(Pause());
                        yield return StartCoroutine(Play());
                        yield return StartCoroutine(Pause());


                        imageDisplay.GetComponent<RawImage>().texture = currImageSet[16];


                        break;
                    case "circular-static":
                        linearAnimations.SetActive(false);
                        linear2Circular.SetActive(false);

                        yield return StartCoroutine(Play());
                        yield return StartCoroutine(Pause());
                        yield return StartCoroutine(Play());
                        yield return StartCoroutine(Pause());
                        yield return StartCoroutine(Play());
                        yield return StartCoroutine(Pause());
                        yield return StartCoroutine(Play());
                        yield return StartCoroutine(Pause());


                        imageDisplay.GetComponent<RawImage>().texture = currImageSet[16];

                        break;
                  
                   
                    case "linear-static":
                        imageContainer.SetActive(false);
                        linear2Circular.SetActive(false);

                        imageMessage = new ImageMessage(currImageSet[int.Parse(media)], currImageSet[16], message.AnimationType);
                        linearAnimations.SetActive(true);
                        gameObject.SendMessage("SetImage", imageMessage);
                        break;

                    case "static-linear":
                        imageContainer.SetActive(false);
                        linear2Circular.SetActive(false);

                        imageMessage = new ImageMessage(currImageSet[int.Parse(media)], currImageSet[16], message.AnimationType);
                        linearAnimations.SetActive(true);
                        gameObject.SendMessage("SetImage", imageMessage);
                        break;

                    case "linear-circular":
                        imageContainer.SetActive(false);
                        linearAnimations.SetActive(false);

                        imageMessage = new ImageMessage(currImageSet[int.Parse(media)], currImageSet[16], message.AnimationType,savedSpeed,radious);
                        gameObject.SendMessage("InitAnimation", imageMessage);
                        linear2Circular.SetActive(true);

                        break;
                    case "circular-linear":
                        imageContainer.SetActive(false);
                        linearAnimations.SetActive(false);

                        imageMessage = new ImageMessage(currImageSet[int.Parse(media)], currImageSet[16], message.AnimationType, savedSpeed, radious);
                        gameObject.SendMessage("InitAnimation",imageMessage);
                        linear2Circular.SetActive(true);
                        break;

                    default:
                        CURRENT_TASK.SetActive(true);
                        SetLabelsInImageQuestions(prev);
                        break;

                }
                break;

            default:
                message_type = message.MediaType;
                display.gameObject.SetActive(true);
                imageContainer.gameObject.SetActive(false);

                fontSize = message.FontSize;
                media = message.Media;

                display.text = media;
                display.fontSize = fontSize;
                display.font = fontAsset[GetFontIndex(message.FontType)];
                break;






        }

       
       

        yield return null;
    }

    
 
}
