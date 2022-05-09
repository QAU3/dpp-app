using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Test : MonoBehaviour
{

    public TMP_Text textComponent, textClone;
    public GameObject imageComponent, imageClone;
    public RawImage originalImage, cloneImage;

    public TMP_FontAsset [] fontAssets; 
    public GameObject PARENT; 
    public GameObject A, B;
    public float textOffset, imageOffset ;
    private RectTransform textComponenTransform;
    private float componentWidth;
   
    public bool isTextRunning, isImageRunning;
    public float speed;
    float savedSpeed; 
    // Start is called before the first frame update
    private void Start()
    {
        savedSpeed = speed;
    }
    private void InitializeAnim(string type)
    {
        
        switch (type)
        {
            case "string":
                PARENT.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, 100);
                componentWidth = PARENT.GetComponent<RectTransform>().rect.width / textOffset;
                RespawnPointsPosition(componentWidth);
                //Position elements
                textComponent.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                textClone.GetComponent<RectTransform>().localPosition = new Vector3(componentWidth, 0, 0);

                textClone.font = fontAssets[PlayerPrefs.GetInt("FontAssetIndex")];
                textComponent.font = fontAssets[PlayerPrefs.GetInt("FontAssetIndex")];

                break;
            case "phra":
                PARENT.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 300);
                componentWidth = PARENT.GetComponent<RectTransform>().rect.width / textOffset;
                RespawnPointsPosition(componentWidth);
                //Position elements
                textComponent.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                textClone.GetComponent<RectTransform>().localPosition = new Vector3(componentWidth, 0, 0);
                textClone.font = fontAssets[PlayerPrefs.GetInt("FontAssetIndex")];
                textComponent.font = fontAssets[PlayerPrefs.GetInt("FontAssetIndex")];
                break;
            case "image":

                PARENT.GetComponent<RectTransform>().sizeDelta=new Vector2(800,800);
                componentWidth = PARENT.GetComponent<RectTransform>().rect.width / imageOffset;
                RespawnPointsPosition(componentWidth);

                //Position elements
                imageComponent.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                imageClone.GetComponent<RectTransform>().localPosition = new Vector3(componentWidth, 0, 0);
                break;
        }

    }


    public void RespawnPointsPosition(float distance)
    {
        A.GetComponent<RectTransform>().localPosition = new Vector3(-distance, 0, 0);
        B.GetComponent<RectTransform>().localPosition = new Vector3(distance, 0, 0);
    }
    public void SetString(StringMessage message)
    {
        if (message.Type == "reading")
        {
                InitializeAnim("phra");
           
        } else
        {
            InitializeAnim("string");
            Debug.Log(message.Type);


        }
        //SET VISIBILITY
        textComponent.gameObject.SetActive(true);
        textClone.gameObject.SetActive(true);
        imageClone.SetActive(false);
        imageComponent.SetActive(false);



        //SET VIEW
        textComponent.text = message.Text;
        textComponent.fontSize = message.FontSize;
        textClone.text = message.Text;
        textClone.fontSize = message.FontSize;

        isTextRunning = true;
        isImageRunning = false; 
     }


    public void SetStringSize(StringMessage message)
    {
        if (isTextRunning)
        {
            textComponent.fontSize = message.FontSize;
            textClone.fontSize = message.FontSize;

        }
    }
    public IEnumerator SetImage(ImageMessage message)
    {
        InitializeAnim("image");
        //SET VISIBILITY
        textComponent.gameObject.SetActive(false);
        textClone.gameObject.SetActive(false);
        imageClone.SetActive(true);
        imageComponent.SetActive(true);

        //SET VIEW
        originalImage.texture = message.CurrentImage;
        cloneImage.texture = message.CurrentImage;

        isTextRunning = false;
        isImageRunning = true;


        if (message.Type == "linear")
        {
            yield return StartCoroutine(Play());
            yield return StartCoroutine(Pause());
            yield return StartCoroutine(Play());
            yield return StartCoroutine(Pause());
            yield return StartCoroutine(Play());
            yield return StartCoroutine(Pause());
            originalImage.texture = message.DefaulImage;
            cloneImage.texture = message.DefaulImage;
        }
        else
        {
            yield return StartCoroutine(Pause());
            yield return StartCoroutine(Play());
            yield return StartCoroutine(Pause());
            yield return StartCoroutine(Play());
            yield return StartCoroutine(Pause());
            yield return StartCoroutine(Play());
            yield return StartCoroutine(Pause());
             originalImage.texture = message.DefaulImage;
            cloneImage.texture = message.DefaulImage;
        }
       

    }
    public void SetSpeed(float v)
    {
        speed = v;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        AnimationHandler();

    }


    public void AnimationHandler()
    {
        if (isTextRunning)
        {
            textComponent.transform.Translate(Vector3.left * Time.fixedDeltaTime * 16 / 32);
            textClone.transform.Translate(Vector3.left * Time.fixedDeltaTime * 16 / 32);

            if (textComponent.transform.position.x <= A.transform.position.x)
            {
                textComponent.transform.position = B.transform.position;
            }

            if (textClone.transform.position.x <= A.transform.position.x)
            {

                textClone.transform.position = B.transform.position;
            }
        }

        if (isImageRunning)
        {
            imageComponent.transform.Translate(Vector3.left * Time.fixedDeltaTime * speed);
            imageClone.transform.Translate(Vector3.left * Time.fixedDeltaTime * speed);

            if (imageComponent.transform.position.x <= A.transform.position.x)
            {
                imageComponent.transform.position = B.transform.position;
            }

            if (imageClone.transform.position.x <= A.transform.position.x)
            {

                imageClone.transform.position = B.transform.position;
            }
        }




    }
    public IEnumerator Pause()
    {
        yield return new WaitForSecondsRealtime(2);
        //Time.timeScale = 0;
        speed= 0;
    }

    public IEnumerator Play()
    {
        yield return new WaitForSecondsRealtime(2);
        //Time.timeScale = 1;
        speed = 0.5f;

    }

}
