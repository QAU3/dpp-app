using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringDecoder : MonoBehaviour
{
    public ImageControl imageControl;
    public TaskControl taskControl;
    public PhasesControl phrasesControl;
    public AudioSource audioPlayer;
    public AudioClip[] audioFiles;
  public void GetData (string data)
    {
        string[] splitData = data.Split(char.Parse(";"));
        Debug.Log(data);
        switch (splitData[0])
        {
            case "char":
                UnityMainThreadDispatcher.Instance().Enqueue(CharTaskListener(splitData[1]));

                break;
            case "image":
                UnityMainThreadDispatcher.Instance().Enqueue(ImageTaskListener(splitData[1]));
                break;
            case "phra":
                UnityMainThreadDispatcher.Instance().Enqueue(PhraseTaskListener(splitData[1]));
                break;
        }
    }
    public IEnumerator PhraseTaskListener(string data)
    {
        phrasesControl.value = data;
        yield return null;
    }
    public IEnumerator CharTaskListener(string data)
    {
        taskControl.value  = data;
        yield return null;
    }

    public IEnumerator ImageTaskListener(string userLabel)
    {
        imageControl.Q_A = userLabel;
        yield return null;
    }

}
