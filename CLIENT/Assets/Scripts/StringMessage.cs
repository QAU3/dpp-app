using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StringMessage
{ 

float fontSize;
string text;
string type;
    string fontType;
    public float FontSize { get => fontSize; set => fontSize = value; }
    public string Text { get => text; set => text = value; }
    public string Type { get => type; set => type = value; }
    public string FontType { get => fontType; set => fontType = value; }

    

    public  StringMessage(string text, float fontSize)
    {
        this.Text= text;
        this.FontSize = fontSize;

    }
    public StringMessage(string text, float fontSize, string type)
    {
        this.Text = text;
        this.FontSize = fontSize;
        this.Type = type;

    }
    public StringMessage(string text, float fontSize, string type, string fontType)
    {
        this.Text = text;
        this.FontSize = fontSize;
        this.Type = type;
        this.FontType = fontType;

    }
}

