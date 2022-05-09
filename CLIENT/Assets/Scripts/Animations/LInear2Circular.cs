using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LInear2Circular : MonoBehaviour
{
    public Texture2D defaultImage;
    public RawImage imageDisplay; 
    public GameObject viewPort, A,B,C,D,E;
    //public TMP_Text textDisplay;
    GameObject pivot; 
    float angularSpeed, linearSpeed, savedSpeed;

    string animType;

    //public RawImage imageDisplay;
    bool linear;
    int animation_repetition;
    float x, y, angle,radious;
    // Start is called before the first frame update
    void Start()
    {
        //InitAnimation( "l");
    }
    public void InitAnimation(ImageMessage message)
    {
        animType = message.Type;
        animation_repetition = 0;
        viewPort.GetComponent<RectTransform>().localPosition = new Vector3(350, 0, 0);
        imageDisplay.texture = message.CurrentImage;
        linear = true;
        angularSpeed = message.Speed;
        savedSpeed = angularSpeed;
        linearSpeed = 0.5f;
        radious = message.Radious;
        defaultImage = message.DefaulImage;
        SetRespawnPositions(animType);

    }

    private void SetRespawnPositions(string type)
    {
        if (type=="linear-circular")
        {
            A.GetComponent<RectTransform>().localPosition = new Vector3(150, 0, 0);
            B.GetComponent<RectTransform>().localPosition = new Vector3(93, 0, 0);
            C.GetComponent<RectTransform>().localPosition = new Vector3(-107, 0, 0);
            D.GetComponent<RectTransform>().localPosition = new Vector3(-164, 0, 0);
            E.GetComponent<RectTransform>().localPosition = new Vector3(-364, 0, 0);

        }
        else
        {
            A.GetComponent<RectTransform>().localPosition = new Vector3(293, 0, 0);
            B.GetComponent<RectTransform>().localPosition = new Vector3(93, 0, 0);
            C.GetComponent<RectTransform>().localPosition = new Vector3(36, 0, 0);
            D.GetComponent<RectTransform>().localPosition = new Vector3(-164, 0, 0);
            E.GetComponent<RectTransform>().localPosition = new Vector3(-221, 0, 0);
            pivot = A;
        }


    }


    public void StopAnimation()
    {
        imageDisplay.texture = defaultImage;
        linearSpeed = 0;
        angularSpeed = 0;
    }
  
   
    // Update is called once per frame
    void FixedUpdate()
    {
        switch (animType)
        {
            case "linear-circular":
                RunNormalAnimation();
                break;
            case "circular-linear":
                RunInvertedAnimation();

                break;

        }



    }

    private void RunInvertedAnimation()
    {
        if (!linear)
        {
            angularSpeed = savedSpeed;
            angle = 0;
            viewPort.transform.Translate(Vector3.left * Time.fixedDeltaTime * linearSpeed);




            switch (animation_repetition)
            {
                case 1:
                    if (viewPort.transform.position.x <= B.transform.position.x) { pivot = C; linear = true; }
                    break;
                case 2:
                    if (viewPort.transform.position.x <= D.transform.position.x) { pivot = E; linear = true; }

                    break;
                case 3:
                    if (viewPort.transform.position.x <= E.transform.position.x)
                    {
                        //pivot = F;
                        //linear = false;
                        StopAnimation();
                    }

                    break;

            }


        }
        else
        {
            x = pivot.transform.position.x + Mathf.Cos(angle) * radious;
            y = pivot.transform.position.y + Mathf.Sin(angle) * radious;
            viewPort.transform.position = new Vector2(x, y);
            angle += (Mathf.Deg2Rad * angularSpeed) * Time.fixedDeltaTime;
         
                    
                    if (angle >= 6)
                    {
                        angularSpeed = 0;
                        linear = false ;
                        if (animation_repetition < 3) { animation_repetition++; }
                        if (animation_repetition == 3) { StopAnimation(); }
                        
                    }
          
            
            Debug.Log(string.Format("x: {0}, y: {1}, angle: {2}, rad:{3}, speed: {4}", x, y, angle, radious, angularSpeed));

        }

        Debug.Log(animation_repetition);
    }

    private void RunNormalAnimation()
    {
        if (linear)
        {
            viewPort.transform.Translate(Vector3.left * Time.fixedDeltaTime * linearSpeed);
            angularSpeed = savedSpeed;
            angle = 0;
            switch (animation_repetition)
            {
                case 0:
                    if (viewPort.transform.position.x <= A.transform.position.x) { pivot = B; linear = false; }
                    break;
                case 1:
                    if (viewPort.transform.position.x <= C.transform.position.x) { pivot = D; linear = false; }

                    break;
                case 2:
                    if (viewPort.transform.position.x <= E.transform.position.x)
                    {
                        //pivot = F;
                        //linear = false;

                        StopAnimation();
                    }

                    break;

            }


        }
        else
        {

            x = pivot.transform.position.x + Mathf.Cos(angle) * radious ;
            y = pivot.transform.position.y + Mathf.Sin(angle) * radious;
            viewPort.transform.position = new Vector2(x, y);
            angle += (Mathf.Deg2Rad * angularSpeed) * Time.fixedDeltaTime;
            if (angle >= 6)
            {
                angularSpeed = 0;
                linear = true;
                if (animation_repetition < 3) { animation_repetition++; } 

            }
            Debug.Log(string.Format("x: {0}, y: {1}, angle: {2}, rad:{3}, speed: {4}", x, y, angle, radious, angularSpeed));

        }

        Debug.Log(animation_repetition);
    }
}
