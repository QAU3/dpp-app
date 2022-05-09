using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhrasesObject 
{
    string phrases;
    string type;
    float fontSize;
    string fontType; 
    public PhrasesObject(string phrases, float size, string type, string fontType)
    {
        this.Phrases = phrases;
        this.FontSize = size;
        this.Type = type;
        this.FontType = fontType;
    }
    public string Phrases { get => phrases; set => phrases = value; }
    public string Type { get => type; set => type = value; }
    public float FontSize { get => fontSize; set => fontSize = value; }
    public string FontType { get => fontType; set => fontType = value; }
}
