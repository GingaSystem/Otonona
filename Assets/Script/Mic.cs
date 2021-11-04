using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;


public class Mic : MonoBehaviour
{
    private NoteNameDetector notename;
    private Text text;
    AudioSource aud; //マイクの音を読み取ったり内部再生する用
    public AudioSource aud2; //初音ミクの声を再生する
    public AudioClip[] clips;
    private String lastNotename;

    private int n = 0;

    void Start()
    {
        aud = GetComponent<AudioSource>();
        text = GetComponentInChildren<Text>();
        //Debug.Log(text);
        notename = new NoteNameDetector();
        // マイク名、ループするかどうか、AudioClipの秒数、サンプリングレート を指定する
        aud.clip = Microphone.Start(null, true, 1, 44100);
        aud.Play();
        //aud2.clip = clips[0];
        //aud2.Play();
        //aud.PlayOneShot(sound);

        text.text = "";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key");
            // スクリーンショットを保存
            CaptureScreenShot("ScreenShot" + n.ToString() + ".png");
            n++;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("Enter");
            GameObject sc = GameObject.Find("ShareController");
            sc.GetComponent<ShareController>().Share(); //ここなにやってるのだろ
        }
        if (Time.frameCount % 10 != 0) { return; }
        Debug.Log("UPDATE!");
        float[] spectrum = new float[1024];
        aud.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
        var maxIndex = 0;
        var maxValue = 0.0f;
        var freq = 0.0f;

        for (int i = 0; i < spectrum.Length; i++)
        {
            var val = spectrum[i];
            if (val > maxValue)
            {
                maxValue = val;
                maxIndex = i;
                // maxValue が最も大きい周波数成分の値で、
                // maxIndex がそのインデックス。欲しいのはこっち。
            }
        }


        if (maxValue < 0.035)
        {
            Debug.Log("無音: maxValue:" + maxValue);
            return;
        }

        //Debug.Log("Audio:" + AudioSettings.outputSampleRate);
        //Debug.Log("spectrum: " + spectrum.Length);
        freq = maxIndex * AudioSettings.outputSampleRate / 2 / spectrum.Length;
        //Debug.Log("freq:" + freq);
        try
        {
            String currentNoteName = notename.GetNoteName(freq);
            Debug.Log("notename" + currentNoteName);

            if (lastNotename != currentNoteName)
            {
                text.text = text.text + currentNoteName;
                lastNotename = currentNoteName;
                notename.soundPlay(freq, this);
                //freqに0が入ると例外が起こる(なぜ0が多いのか)
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }




    }
    // 画面全体のスクリーンショットを保存する
    void CaptureScreenShot(string filePath)
    {
        ScreenCapture.CaptureScreenshot(filePath);
        Debug.Log("☆CaptureScreenShot");
    }

}

public class NoteNameDetector
{
    Mic mic;
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
    public void soundPlay(float freq, Mic mic)
    {
        Debug.Log("soundPlay: " + freq);
        var noteNumber = calculateNoteNumberFromFrequency(freq);
        var note = noteNumber % 12; //0〜11の数字が入る
        mic.aud2.clip = mic.clips[note];
        mic.aud2.Stop();
        mic.aud2.PlayOneShot(mic.aud2.clip);
        Debug.Log("soundPlay: " + noteNames[note]);

    }

    // See https://en.wikipedia.org/wiki/MIDI_tuning_standard
    private int calculateNoteNumberFromFrequency(float freq)
    {
        return Mathf.RoundToInt(69 + 12 * Mathf.Log(freq / 440, 2));
    }
}