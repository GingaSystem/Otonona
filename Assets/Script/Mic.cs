using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
//todo
//同じ音の連続でも無音が挟まった時にはもう一度歌う
//音の追加
//変化が大きかった部分に注目
public class Mic : MonoBehaviour
{
    private NoteNameDetector notename;
    private Text text;
    AudioSource aud; //マイクの音を読み取ったり内部再生する用
    public AudioSource aud2; //初音ミクの声を再生する
    public AudioClip[] clips; //3オクターブ分のクリップを入れておく
    public Slider slider;

    public double minSound;

    private String lastNotename;

    private int n = 0;



    void Start()
    {
        aud = GetComponent<AudioSource>();
        text = GetComponentInChildren<Text>();
        slider = GetComponentInChildren<Slider>();
        //Debug.Log(slider.value);
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
            Debug.Log("UpArrow");
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

        minSound = slider.value;
        Debug.Log("Slider: "+ slider.value);
        Boolean isMuon = false;
        if (maxValue < minSound)
        {
            isMuon = true;
            Debug.Log("無音: maxValue:" + maxValue);
            return;
        }

        //Debug.Log("Audio:" + AudioSettings.outputSampleRate);
        //Debug.Log("spectrum: " + spectrum.Length);
        freq = maxIndex * AudioSettings.outputSampleRate / 2 / spectrum.Length;
        //Debug.Log("freq:" + freq);
        try
        {
            String currentNoteName = notename.GetNoteName(freq,maxValue);
            Debug.Log("notename" + currentNoteName);

            if (lastNotename != currentNoteName) //lastNoteの音量がminSoundより小さかった時にも動かしたい
            {
                text.text = text.text + currentNoteName;
                lastNotename = currentNoteName;
                notename.soundPlay(freq, this, maxValue);
                //freqに0が入ると例外が起こる
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
    public void Clear()
    {
        text.text = "";
    }

}

public class NoteNameDetector
{
    Mic mic;
    private string[] noteNames = { "ド", "ド♯", "レ", "レ♯", "ミ", "ファ", "ファ♯", "ソ", "ソ♯", "ラ", "ラ♯", "シ" };

    public string GetNoteName(float freq, float maxValue)
    {
        //Debug.Log("GetNoteName: "+freq);
        // 周波数からMIDIノートナンバーを計算
        var noteNumber = calculateNoteNumberFromFrequency(freq);
        //0から100くらい？Midiに対応している 全てのオクターブを重ねるのもあり
        // 0:C - 11:B に収める
        var note = noteNumber % 12; //商がオクターブに対応している
        Debug.Log("3オクターブ外 note:" + note);
        Debug.Log("判定済みMaxValue:"+ maxValue);
        // 0:C～11:Bに該当する音名を返す
        return noteNames[note];

    }
    //noteNumberの値によって条件分岐をして指定の3オクターブに入っている時と入っていない時でわけたい
    //真ん中のド(C3)はnoteNumberが60 C2=48 C4=72 B4=83
    //入っている時→実際の音の高さで歌う。入っていない時→12で割ったあまり
    //12で割った時：C2=4...0,C3=5...0,C4...6...0
    //C2を0番目の要素としたいから48をひけばおっけー？
    public void soundPlay(float freq, Mic mic, float maxValue)
    {
        var noteNumber = calculateNoteNumberFromFrequency(freq);
        if (noteNumber <= 83 && noteNumber >= 48)
        {
            Debug.Log("3オクターブ内 soundPlay freq: " + freq);
            var note = noteNumber - 48; //NoteNumberに48〜83の数字が入る。noteは0〜35(配列の要素36個)
            Debug.Log("3オクターブ内 soundPlay noteNumber: " + note);
            mic.aud2.clip = mic.clips[note];
            mic.aud2.Stop();
            mic.aud2.PlayOneShot(mic.aud2.clip);
            Debug.Log("soundPlay: " + noteNames[note]);
            Debug.Log("判定済みMaxValue:"+ maxValue);
        }
        else
        {
            Debug.Log("3オクターブ外 soundPlay freq: " + freq);
            var note = noteNumber % 12; //0〜11の数字が入る
            Debug.Log("3オクターブ外 soundPlay noteNumber: " + note);
            mic.aud2.clip = mic.clips[note];
            mic.aud2.Stop();
            mic.aud2.PlayOneShot(mic.aud2.clip);
            Debug.Log("soundPlay: " + noteNames[note]);
        }

    }

    // See https://en.wikipedia.org/wiki/MIDI_tuning_standard
    private int calculateNoteNumberFromFrequency(float freq)
    {
        return Mathf.RoundToInt(69 + 12 * Mathf.Log(freq / 440, 2));
    }
}