public class Pairs
{
    int fontSize;
    string sentence;

    public Pairs()
    {
    }

    public Pairs(int fontSize, string sentence)
    {
        this.fontSize = fontSize;
        this.sentence = sentence;
    }

    public int GetFontSize()
    {
        return this.fontSize; 
    }
    public string GetSentence()
    {
        return this.sentence;
    }

    public void SetFontSize(int fontSize) 
    {
        this.fontSize = fontSize;
    }
    public void SetSentece(string sentence)
    {
        this.sentence = sentence;
    }
}