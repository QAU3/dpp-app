using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringCollection 
{
    int fontSize;
    string sentece;
    string type;
    string fontType;
    public StringCollection(string sentence, int fontSize, string type, string fontType)
    {
        this.FontSize = fontSize;
        this.Sentece = sentence;
        this.Type = type;
        this.FontType = fontType;
    }

    public int FontSize { get => fontSize; set => fontSize = value; }
    public string Sentece { get => sentece; set => sentece = value; }
    public string Type { get => type; set => type = value; }
    public string FontType { get => fontType; set => fontType = value; }
}
