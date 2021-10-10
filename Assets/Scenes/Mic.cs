using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;


public class Mic : MonoBehaviour {
        private NoteNameDetector notename;
        private Text text;
        //public AudioClip sound;
        
    void Start() {
        AudioSource aud = GetComponent<AudioSource>();
        text = GetComponentInChildren<Text>();
        //Debug.Log(text);
        notename = new NoteNameDetector();
        // マイク名、ループするかどうか、AudioClipの秒数、サンプリングレート を指定する
        aud.clip = Microphone.Start(null, true, 1, 44100);
        aud.Play();
        //aud.PlayOneShot(sound);
        
        text.text = "読み取り中";   
    }
    void Update(){
      
       
      if(Time.frameCount%10 != 0){return;}
    float[] spectrum = new float[1024];
    AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
    var maxIndex = 0;
    var maxValue = 0.0f;
    var freq = 0.0f;
    
    for (int i = 0; i < spectrum.Length; i++)
    {
    var val = spectrum[i];
    if (val > maxValue){
    maxValue = val;
    maxIndex = i;
    // maxValue が最も大きい周波数成分の値で、
    // maxIndex がそのインデックス。欲しいのはこっち。
    }

    //Debug.Log("max: " + maxIndex);
    //Debug.Log("Audio:" + AudioSettings.outputSampleRate);
    //Debug.Log("spectrum: " + spectrum.Length);
    freq = maxIndex * AudioSettings.outputSampleRate / 2 / spectrum.Length;
    Debug.Log("freq:" + freq);
    try{
    Debug.Log("notename" + notename.GetNoteName(freq));
    text.text = notename.GetNoteName(freq);
    }catch(Exception e){
      Debug.Log(e);
    }
    }

  }    
    
}

public class NoteNameDetector
{
  private string[] noteNames = { "ド", "ド♯", "レ", "レ♯", "ミ", "ファ", "ファ♯", "ソ", "ソ♯", "ラ", "ラ♯", "シ" };

  public string GetNoteName(float freq)
  {
    //Debug.Log("GetNoteName: "+freq);
    // 周波数からMIDIノートナンバーを計算
    var noteNumber = calculateNoteNumberFromFrequency(freq); //0から100くらい？Midiに対応している 全てのオクターブを重ねるのもあり
    // 0:C - 11:B に収める
    var note = noteNumber % 12; //あまりではなくて商がオクターブに対応しているかも
    Debug.Log("note:" + note);
    // 0:C～11:Bに該当する音名を返す
    return noteNames[note];
  }

  // See https://en.wikipedia.org/wiki/MIDI_tuning_standard
  private int calculateNoteNumberFromFrequency(float freq)
  {
    return Mathf.RoundToInt(69 + 12 * Mathf.Log(freq / 440, 2));
  }
}