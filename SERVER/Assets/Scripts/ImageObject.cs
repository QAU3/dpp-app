using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageObject
{
    int position;
    string label;
    string selected_size;
    string type;

    public  ImageObject(int position, string label)
    {
        this.Position = position;
        this.Label = label;
    }
    public ImageObject(int position, string label,string size)
    {
        this.Position = position;
        this.Label = label;
        this.Selected_size = size;
    }
    public ImageObject(int position, string label, string size,string type)
    {
        this.Position = position;
        this.Label = label;
        this.Selected_size = size;
        this.Type = type;
    }
    public int Position { get => position; set => position = value; }
    public string Label { get => label; set => label = value; }
    public string Selected_size { get => selected_size; set => selected_size = value; }
    public string Type { get => type; set => type = value; }
}
