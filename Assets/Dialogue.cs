using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Dialogue
{
    public string name;

    [TextArea(3, 30)]
    public string[] sentences;
    public AudioClip[] audioClips; // Array of audio clips

}
