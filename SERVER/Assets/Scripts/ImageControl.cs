using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Events;


public class ImageControl : MonoBehaviour
{
    [Header("Preloading")]
    public string[] ImageLabels;
    public Texture2D[] ImageSetPractice, ImageSetOne, ImageSetTwo, ImageSetThree, ImageSetFour;
    public List<ImageObject> imagesCollection, randomizedCollection, errorCollection;


    [Header("Imageviewers")]
    public RawImage preImage;
    public RawImage currImage;
    public RawImage nextImage;

    [Header("Control")]
    public TMP_Text indexMonitor;
    public TMP_Text labelMonitor,setMonitor;
    public Button preBtn, nextBtn, startBtn, returnBtn,setBtn1, setBtn2,setBtn3,setBtn4,setBtn5,setBtn6;
    public Button instructions, ending;

    [Header("Task configuration")]
    public string taskType;
    string taskTypeOne, taskTypeTwo,taskTypeThree;
    public bool isPractice;
    public string Q_A; 

    [Header("Task configuration")]
    public UDPSender udpSender;

   [Header("Audio settinds")]
   public AudioSource audioPlayer;
   public AudioClip[] audioFiles;

    public bool isSetOne;
    private int currIndex;
    public Texture2D[] currImageSet;
    public string userLabel,userImageSize,currSet;

    [Header("New Approach")]
    public GameObject _MENU_;
    public GameObject _IMAGETASK_;

    public static System.Random rng = new System.Random();

    public CSVLogger csvLogger, csvContinue;
    // Start is called before the first frame update
    public void Start()
    {
        //Buttons

        nextBtn.onClick.AddListener(NextItem);
        preBtn.onClick.AddListener(PrevItem);
        returnBtn.onClick.AddListener(ChangeScene);
        startBtn.onClick.AddListener(StartTask);
        setBtn1.onClick.AddListener(delegate { SetImageSet(1); });
        setBtn2.onClick.AddListener(delegate { SetImageSet(2); });
        setBtn3.onClick.AddListener(delegate { SetImageSet(3); });
        setBtn4.onClick.AddListener(delegate { SetImageSet(4); });
        setBtn5.onClick.AddListener(delegate { SetImageSet(5); });
        setBtn6.onClick.AddListener(delegate { SetImageSet(6); });

        instructions.onClick.AddListener(delegate { SendTaskMessage("instructions"); });
        ending.onClick.AddListener(delegate { SendTaskMessage("end"); });
        startBtn.gameObject.SetActive(false);
    }
    private void SendTaskMessage(string v)
    {
        NetworkMessage message;
        string instructions; 
        if (v == "instructions")
        {
            //mediaType,media,animationType,fontSiye,fontTzpe,set, userid
            instructions = "In the next task, you will see an image that is going to be static and then it will move for a few seconds. This will be repeated a few times per image. After that you will have to select in which one do you see more details.";
            message = new NetworkMessage("message",instructions,"static",64f,"arial","null","robot");
            udpSender.sendNetworkMessage(message);

            //udpSender.sendString("message_static;" + "In the next task, you will see an image that is going to be static and then it will move for a few seconds. This will be repeated 3 times per image. After that you will have to select in which one do you see more details.;64");
        }
        if (v == "end")
        {
            instructions = "Task finished.\n Thank you!!";
            message = new NetworkMessage("message", instructions, "static", 64f, "arial", "null", "robot");
            udpSender.sendNetworkMessage(message);
            //udpSender.sendString("message_static;" + "Task finished.\n Thank you!!;64");
        }
    }

 

    public void StartImageTask()
    {
        currIndex = 0;
        userLabel = "";
        userImageSize = "";
       

    }

    private void SetImageSet(int set)
    {
        switch (set)
        {
            case 1:
                setBtn1.GetComponentInChildren<Image>().color = Color.green;
                setBtn2.GetComponentInChildren<Image>().color = Color.white;
                setBtn3.GetComponentInChildren<Image>().color = Color.white;
                setBtn4.GetComponentInChildren<Image>().color = Color.white;
                setBtn5.GetComponentInChildren<Image>().color = Color.white;
                setBtn6.GetComponentInChildren<Image>().color = Color.white;


                currImageSet = ImageSetOne;
                currSet = isPractice ? "SetPractice" : "SetOne";
                taskTypeOne = "static-circular";
                taskTypeTwo = "static-linear";
                taskTypeThree = "linear-circular";
               
                break;
            case 2:
                setBtn1.GetComponentInChildren<Image>().color = Color.white;
                setBtn2.GetComponentInChildren<Image>().color = Color.green;
                setBtn3.GetComponentInChildren<Image>().color = Color.white;
                setBtn4.GetComponentInChildren<Image>().color = Color.white;
                setBtn5.GetComponentInChildren<Image>().color = Color.white;
                setBtn6.GetComponentInChildren<Image>().color = Color.white;

                currImageSet = ImageSetOne;
                currSet = "SetOne";
                taskTypeOne = "circular-static";
                taskTypeTwo = "linear-static";
                taskTypeThree = "circular-linear";
                break;
            case 3:
                setBtn1.GetComponentInChildren<Image>().color = Color.white;
                setBtn2.GetComponentInChildren<Image>().color = Color.white;
                setBtn3.GetComponentInChildren<Image>().color = Color.green;
                setBtn4.GetComponentInChildren<Image>().color = Color.white;
                setBtn5.GetComponentInChildren<Image>().color = Color.white;
                setBtn6.GetComponentInChildren<Image>().color = Color.white;

                currImageSet = ImageSetOne;
                currSet = "SetOne";
                taskTypeOne = "static-linear";
                taskTypeTwo = "linear-circular";
                taskTypeThree =  "static-circular";

                break;
            case 4:
                setBtn1.GetComponentInChildren<Image>().color = Color.white;
                setBtn2.GetComponentInChildren<Image>().color = Color.white;
                setBtn3.GetComponentInChildren<Image>().color = Color.white;
                setBtn4.GetComponentInChildren<Image>().color = Color.green;
                setBtn5.GetComponentInChildren<Image>().color = Color.white;
                setBtn6.GetComponentInChildren<Image>().color = Color.white;

                currImageSet = ImageSetOne;
                currSet = "SetOne";
                taskTypeOne = "linear-static";
                taskTypeTwo = "circular-linear";
                taskTypeThree = "circular-static";
                break;
            case 5:
                setBtn1.GetComponentInChildren<Image>().color = Color.white;
                setBtn2.GetComponentInChildren<Image>().color = Color.white;
                setBtn3.GetComponentInChildren<Image>().color = Color.white;
                setBtn4.GetComponentInChildren<Image>().color = Color.white;
                setBtn5.GetComponentInChildren<Image>().color = Color.green;
                setBtn6.GetComponentInChildren<Image>().color = Color.white;


                currImageSet = ImageSetOne;
                currSet = "SetOne";
                taskTypeOne = "circular-linear";
                taskTypeTwo = "circular-static";
                taskTypeThree =  "linear-static";
                break;
            case 6:
                setBtn1.GetComponentInChildren<Image>().color = Color.white;
                setBtn2.GetComponentInChildren<Image>().color = Color.white;
                setBtn3.GetComponentInChildren<Image>().color = Color.white;
                setBtn4.GetComponentInChildren<Image>().color = Color.white;
                setBtn5.GetComponentInChildren<Image>().color = Color.white;
                setBtn6.GetComponentInChildren<Image>().color = Color.green;
                currImageSet = ImageSetOne;
                currSet = "SetOne";
                taskTypeOne = "linear-linear";
                taskTypeTwo = "static-circular";
                taskTypeThree = "static-linear";
                break;
        }
        setMonitor.text = currSet;
        startBtn.gameObject.SetActive(true);
        StartImageTask();
        DeployLocalImages();
    }

    private void StartTask()
    {
        string media = randomizedCollection[currIndex].Position.ToString();
        string animType = randomizedCollection[currIndex].Label;
        NetworkMessage message = new NetworkMessage("image", media, animType, 0, "arial", currSet, PlayerPrefs.GetString("UserID"));
        udpSender.sendNetworkMessage(message);
        //udpSender.sendString(randomizedCollection[currIndex].Label + ";" + randomizedCollection[currIndex].Position + ";" + randomizedCollection[currIndex].Label + ";" + currSet);
        //udpSender.sendString(randomizedCollection[currIndex].Type + ";" + randomizedCollection[currIndex].Position + ";" + randomizedCollection[currIndex].Label + ";" + currSet) ;
    }
    private void ChangeScene()
    {

        currIndex = 0;
        userLabel = "";
        userImageSize = "";
        currImageSet = null;
        setBtn1.GetComponentInChildren<Image>().color = Color.white;
        setBtn2.GetComponentInChildren<Image>().color = Color.white;
        setBtn3.GetComponentInChildren<Image>().color = Color.white;
        setBtn4.GetComponentInChildren<Image>().color = Color.white;
        preImage.GetComponentInChildren<RawImage>().texture = null;
        currImage.GetComponentInChildren<RawImage>().texture = null;
        nextImage.GetComponentInChildren<RawImage>().texture = null;
        startBtn.gameObject.SetActive(false);

        _MENU_.SetActive(true);
        _IMAGETASK_.SetActive(false);

        //SceneManager.LoadScene("Main");
    }

    private void PrevItem()
    {
        if (currIndex > 0)
        {
            currIndex--;
        }
        SetView(currIndex - 1, currIndex, currIndex + 1,currImageSet);
    }

    private void NextItem()
    {
        String timeStamp = GetTimestamp(DateTime.Now);

        if (!isPractice)
        {
            csvContinue.AddEntry(new string[] {
            randomizedCollection[currIndex].Position.ToString(),
            randomizedCollection[currIndex].Label,
            Q_A,
            userImageSize,
            timeStamp
        });

            csvContinue.Save("result_" + PlayerPrefs.GetString("UserID") + "_" + taskType + "_" + currSet+ ".csv");
        }
        userImageSize= "";
        userLabel = "";

        if (currIndex < randomizedCollection.Count - 1)
        {
            currIndex++;
            
        }
        else { return; }
        SetView(currIndex - 1, currIndex, currIndex + 1, currImageSet);
       
        string media = randomizedCollection[currIndex].Position.ToString();
        string animType = randomizedCollection[currIndex].Label;
        NetworkMessage message = new NetworkMessage("image", media,animType,0,"arial",currSet,PlayerPrefs.GetString("UserID"));
        udpSender.sendNetworkMessage(message);
        // udpSender.sendString(taskType + ";" + randomizedCollection[currIndex].Position + ";" + randomizedCollection[currIndex].Label + ";" + currSet);
        //udpSender.sendString(randomizedCollection[currIndex].Label + ";" + randomizedCollection[currIndex].Position + ";" + randomizedCollection[currIndex].Label + ";" + currSet);
    }

    public void DeployLocalImages()
    {
        currIndex = 0;
        userLabel = "";
        userImageSize = "";

        imagesCollection = new List<ImageObject>();
        randomizedCollection = new List<ImageObject>();
        errorCollection = new List<ImageObject>();
        //CreateCollections(ImageLabels);
        //Shuffle<ImageObject>(randomizedCollection);
        //randomizedCollection.ShuffleCrypto();
        HarcodedCollections();
        SetView(currIndex-1, currIndex, currIndex+1, currImageSet);
        if (!isPractice)
        {
            string[] columns = new string[] { "index_order", "label" };
            csvLogger = new CSVLogger(columns);
            csvContinue = new CSVLogger(new string[] { "original_index_order", "original_label", "questions", "user_size","time_stamp" });

            foreach (ImageObject item in randomizedCollection)
            {
                csvLogger.AddEntry(new string[] { item.Position.ToString(), item.Label });
            }
            String timeStamp = GetTimestamp(DateTime.Now);
            csvLogger.Save(Application.persistentDataPath+"/original_" + taskType + "_task_" + PlayerPrefs.GetString("UserID") + "_" + currSet + "_" + timeStamp + ".csv");

        }

        isSetOne = !isSetOne;

    }



    public void SetView(int prev, int curr, int next,Texture2D [] ImageSet)
    {

        if (prev < 0)
        {
            preImage.GetComponentInChildren<RawImage>().texture =null;

        }
        else
        {
            preImage.GetComponentInChildren<RawImage>().texture = ImageSet[randomizedCollection[prev].Position];

        }
        if (next> randomizedCollection.Count-1)
        {
            audioPlayer.PlayOneShot(audioFiles[2], 1.0f);
            nextImage.GetComponentInChildren<RawImage>().texture = null;

        }
        else
        {
            nextImage.GetComponentInChildren<RawImage>().texture = ImageSet[randomizedCollection[next].Position];

        }



        currImage.GetComponentInChildren<RawImage>().texture = ImageSet[randomizedCollection[curr].Position];

        indexMonitor.text = "Index pos: " + curr.ToString();
        labelMonitor.text = randomizedCollection[curr].Label;
    }
    

    private void CreateCollections(string[] imageLabels)
    {
        for (int i =0; i < imageLabels.Length; i++)
        {
            imagesCollection.Add(new ImageObject(i, imageLabels[i]));
            randomizedCollection.Add(new ImageObject(i, imageLabels[i]));

        }

    }


   public void HarcodedCollections()
    {
        randomizedCollection.Add(new ImageObject(0, taskTypeOne));
        randomizedCollection.Add(new ImageObject(15, "image_question"));

        randomizedCollection.Add(new ImageObject(1, taskTypeTwo));
        randomizedCollection.Add(new ImageObject(15, "image_question"));

        randomizedCollection.Add(new ImageObject(2, taskTypeThree));
        randomizedCollection.Add(new ImageObject(15, "image_question"));

        randomizedCollection.Add(new ImageObject(3, taskTypeOne));
        randomizedCollection.Add(new ImageObject(15, "image_question"));

        randomizedCollection.Add(new ImageObject(4, taskTypeTwo));
        randomizedCollection.Add(new ImageObject(15, "image_question"));

        randomizedCollection.Add(new ImageObject(5, taskTypeThree));
        randomizedCollection.Add(new ImageObject(15, "image_question"));

        randomizedCollection.Add(new ImageObject(6, taskTypeOne));
        randomizedCollection.Add(new ImageObject(15, "image_question"));

        randomizedCollection.Add(new ImageObject(7, taskTypeTwo));
        randomizedCollection.Add(new ImageObject(15, "image_question"));

        randomizedCollection.Add(new ImageObject(8, taskTypeThree));
        randomizedCollection.Add(new ImageObject(15, "image_question"));

        randomizedCollection.Add(new ImageObject(9, taskTypeOne));
        randomizedCollection.Add(new ImageObject(15, "image_question"));

        randomizedCollection.Add(new ImageObject(10, taskTypeTwo));
        randomizedCollection.Add(new ImageObject(15, "image_question"));

        randomizedCollection.Add(new ImageObject(11, taskTypeThree));
        randomizedCollection.Add(new ImageObject(15, "image_question"));

        randomizedCollection.Add(new ImageObject(12, taskTypeOne));
        randomizedCollection.Add(new ImageObject(15, "image_question"));

        randomizedCollection.Add(new ImageObject(13, taskTypeTwo));
        randomizedCollection.Add(new ImageObject(15, "image_question"));

        randomizedCollection.Add(new ImageObject(14, taskTypeThree));
        randomizedCollection.Add(new ImageObject(15, "image_question"));

      


    }


    public void Shuffle<T>(List<T> list)

      {

          for (int i = list.Count - 1; i > 0; i--)

          {

              int r = UnityEngine.Random.Range(0, i);

              T temp = list[i];

              list[i] = list[r];

              list[r] = temp;

          }

      }

    public static String GetTimestamp(DateTime value)
    {
        return value.ToString("MMddHHmmss");
    }
    public void GetData(string data)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(ThisWillBeExecutedOnTheMainThread(data));

    }

    public IEnumerator ThisWillBeExecutedOnTheMainThread(string data)
    {
        string[] splitArray = data.Split(char.Parse(";"));

        userLabel = splitArray[0];
        userImageSize= splitArray[1];
        Debug.Log(userLabel + "_" + randomizedCollection[currIndex].Label);

        if (randomizedCollection[currIndex].Label == userLabel)
        {
            audioPlayer.PlayOneShot(audioFiles[1],1.0f);

        }
        else
        {
            audioPlayer.PlayOneShot(audioFiles[0],1.0f);

        }

        yield return null;
    }

}
