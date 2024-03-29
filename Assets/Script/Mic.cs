using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Mic : MonoBehaviour
{
    private NoteNameDetector notename;
    private Text text;
    AudioSource aud; //マイクの音を読み取ったり内部再生する用
    public AudioSource aud2; //初音ミクの声を再生する
    public AudioClip[] clips; //3オクターブ分のクリップを入れておく
    public AudioClip[] clips_interval;
    public static double minSound = 0.01;
    private String lastNoteName;
    private int n = 0;
    public int lastNoteNumber;
    private int otonosa;
    public static bool toggleAbsOn = true;
    public static bool toggleRelativeOn;




    void Start()
    {
        aud = GetComponent<AudioSource>();
        text = GetComponentInChildren<Text>();
        notename = new NoteNameDetector();
        // マイク名、ループするかどうか、AudioClipの秒数、サンプリングレート を指定する
        aud.clip = Microphone.Start(null, true, 1, 44100);
        aud.Play();
        text.text = "";


        // Test
        testCalcDegree();       
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
        if (Time.frameCount % 5 != 0) { return; } //実行スピード調整
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
                // maxIndex がそのインデックス。
            }
        }

       
        Debug.Log("HomeMinSound: "+ minSound);
        if (maxValue < minSound)
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
            String currentNoteName = notename.GetNoteName(freq,maxValue);
            int currentNoteNumber = notename.calculateNoteNumberFromFrequency(freq)-48; //0から35の数字ならば3オクターブ内
            Debug.Log("notename" + currentNoteName);

            //String appendText = "";
            if ((lastNoteName != currentNoteName)&&(lastNoteNumber != currentNoteNumber) && toggleAbsOn) //絶対音感
            {   text.text += currentNoteName;
                //appendText = currentNoteName;
                lastNoteNumber = currentNoteNumber;
                notename.soundPlay(freq, this, maxValue);
                
                
            }
            
            if((lastNoteNumber != currentNoteNumber) && toggleRelativeOn){ //相対音感
                //appendText = $"{currentNoteName} ({lastNoteNumber} -> {currentNoteNumber}), ";
                //appendText = currentNoteName;
                text.text += currentNoteName;
                otonosa = calcDegree(currentNoteNumber, lastNoteNumber); //otonosaは0〜35にする(3オクターブ対応) 
                Debug.Log($"☆ {lastNoteNumber} -> {currentNoteNumber} (Δ{otonosa} deg.)");
                lastNoteNumber = currentNoteNumber;
                
                if(otonosa>=36){otonosa = 2;}
                notename.soundPlay2(freq, this, maxValue, otonosa);
                
               

            }
            /* if(appendText.Length != 0) {
                text.text += appendText;
            }*/
           
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    int calcDegree(int to, int from){
        // { "ド", "ド♯", "レ", "レ♯", "ミ", "ファ", "ファ♯", "ソ", "ソ♯", "ラ", "ラ♯", "シ" }; // -> 12
        int[] distanceList = {1, 1, 2, 2, 3, 4, 4, 5, 5, 6, 6, 7};
        int toNote = to % 12;
        int fromNote = from % 12;

        int distance = Math.Abs(distanceList[toNote] - distanceList[fromNote]) + 1;
        int octaves = (int)Math.Abs(to - from) / 12;

        return octaves * 7 + distance;
    }

    void testCalcDegree() {
        //Debug.Assert(false);
        //{ "ド", "ド♯", "レ", "レ♯", "ミ", "ファ", "ファ♯", "ソ", "ソ♯", "ラ", "ラ♯", "シ" };
        Debug.Assert(calcDegree(0, 0) == 1);

        Debug.Assert(calcDegree(0, 1) == 1);
        Debug.Assert(calcDegree(1, 0) == 1);
        Debug.Assert(calcDegree(0, 2) == 2);
        Debug.Assert(calcDegree(2, 0) == 2);
        Debug.Assert(calcDegree(0, 3) == 2);
        Debug.Assert(calcDegree(3, 0) == 2);

        Debug.Assert(calcDegree(1, 2) == 2);
        Debug.Assert(calcDegree(2, 1) == 2);
        Debug.Assert(calcDegree(1, 3) == 2);
        Debug.Assert(calcDegree(3, 1) == 2);

        Debug.Assert(calcDegree(2, 3) == 1);
        Debug.Assert(calcDegree(3, 2) == 1);
        Debug.Assert(calcDegree(3, 4) == 2);
        Debug.Assert(calcDegree(4, 3) == 2);

        Debug.Assert(calcDegree(0, 12) == 8);
        Debug.Assert(calcDegree(12, 0) == 8);
        Debug.Assert(calcDegree(0, 13) == 8);
        Debug.Assert(calcDegree(13, 0) == 8);
        Debug.Assert(calcDegree(0, 14) == 9);
        Debug.Assert(calcDegree(14, 0) == 9);

        Debug.Assert(calcDegree(2, 26) == 15);
        Debug.Assert(calcDegree(26, 2) == 15);
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
    public void ButtonClicked() {
        SceneManager.LoadScene("setUp");
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
    public void soundPlay2(float freq, Mic mic, float maxValue, int otonosa){

            mic.aud2.clip = mic.clips_interval[otonosa-1];
            mic.aud2.Stop();
            mic.aud2.PlayOneShot(mic.aud2.clip);
            Debug.Log("soundPlay2: relative pitch " + otonosa);

    }

    // See https://en.wikipedia.org/wiki/MIDI_tuning_standard
    public int calculateNoteNumberFromFrequency(float freq)
    {
        return Mathf.RoundToInt(69 + 12 * Mathf.Log(freq / 440, 2));
    }

    
}