using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class setUpScene : MonoBehaviour
{
    public Slider slider;
    public Toggle ToggleAbs;
    public Toggle ToggleRelative;
    
    
    void Start()
    {
        slider.value = (float)Mic.minSound;
        ToggleAbs.isOn = Mic.toggleAbsOn;
        ToggleRelative.isOn = Mic.toggleRelativeOn;
    }

    // Update is called once per frame
    void Update()
    {
        Mic.minSound = slider.value;
        //Debug.Log(slider.value);
    }
    public void ButtonClicked(){
        SceneManager.LoadScene("Home");
    }
    public void ToggleAbsOn(){
        Mic.toggleAbsOn = ToggleAbs.isOn;
    }
    public void ToggleRelativeOn(){
        Mic.toggleRelativeOn = ToggleRelative.isOn;
    }
}
