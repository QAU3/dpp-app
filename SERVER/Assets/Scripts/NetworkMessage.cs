using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

[Serializable]
public class NetworkMessage
{
    string mediaType;
    string media;
    string animationType;
    float fontSize;
    string fontType;
    string set;
    string userid;
    public NetworkMessage(string mediaType, string media, string animationType, float fontSize, string fontType, string set, string userid)
    {
        this.MediaType = mediaType;
        this.Media = media;
        this.AnimationType = animationType;
        this.FontSize = fontSize;
        this.fontType = fontType;
        this.Set = set;
        this.Userid = userid;

    }

    public string MediaType { get => mediaType; set => mediaType = value; }
    public string Media { get => media; set => media = value; }
    public string AnimationType { get => animationType; set => animationType = value; }
    public float FontSize { get => fontSize; set => fontSize = value; }
    public string FontType { get => fontType; set => fontType = value; }
    public string Set { get => set; set => set = value; }
    public string Userid { get => userid; set => userid = value; }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("userid: ").Append(this.Userid).Append(", ");
        sb.Append("mediaType: ").Append(this.MediaType).Append(", ");
        sb.Append("media: ").Append(this.Media).Append(", ");
        sb.Append("animationType: ").Append(this.AnimationType).Append(", ");
        sb.Append("fontSize: ").Append(this.FontSize).Append(", ");
        sb.Append("fontType: ").Append(this.FontSize).Append(", ");
        sb.Append("set: ").Append(this.Set);

        return sb.ToString();
    }
}
