using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageMessage 
{
    private Texture2D currentImage, defaulImage;
    private string type;
    private  float speed;
    private float radious; 
    public ImageMessage(Texture2D imageOne, Texture2D imageTwo, string type)
    {
        this.CurrentImage = imageOne;
        this.DefaulImage = imageTwo;
        this.Type = type;

    }
    public ImageMessage(Texture2D imageOne, Texture2D imageTwo, string type, float speed,float radious)
    {
        this.CurrentImage = imageOne;
        this.DefaulImage = imageTwo;
        this.Type = type;
        this.Speed = speed;
        this.Radious = radious;
    }


    public Texture2D CurrentImage { get => currentImage; set => currentImage = value; }
    public Texture2D DefaulImage { get => defaulImage; set => defaulImage = value; }
    public string Type { get => type; set => type = value; }
    public float Speed { get => speed; set => speed = value; }
    public float Radious { get => radious; set => radious = value; }
}
