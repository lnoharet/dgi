using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_handler : MonoBehaviour
{
    public NS_fluid fluid;
    //public GameObject Sliders;
    public Slider ViscSlider;
    public Slider DiffSlider;
    public Slider ForceSlider;
    public Slider SourceSlider;
    public Toggle PresetToggle;
    public Toggle BorderToggle;
    public Button ClearButton;

    // Start is called before the first frame update
    void Start() {
        ViscSlider.onValueChanged.AddListener(delegate {update_slider_value(1);});
        DiffSlider.onValueChanged.AddListener(delegate {update_slider_value(2);});
        ForceSlider.onValueChanged.AddListener(delegate {update_slider_value(3);});
        SourceSlider.onValueChanged.AddListener(delegate {update_slider_value(4);});
        BorderToggle.onValueChanged.AddListener(delegate {update_toggle_value(1);});
        PresetToggle.onValueChanged.AddListener(delegate {update_toggle_value(2);});
        ClearButton.onClick.AddListener(delegate {update_button_value(1);});
    }

    // Is called when a slider value is changed. 
    // Param idx refers to which slider is being changed. 
    void update_slider_value(int idx){
        switch(idx){
            case 1:
                fluid.viscosity = ViscSlider.value;
                break;
            case 2:
                fluid.densdiff = DiffSlider.value;
                break;
            case 3:
                fluid.force = ForceSlider.value;
                break;
            case 4:
                fluid.source = SourceSlider.value;
                break;
            default:
                break;

        }
    }

    // Is called when a checkbox is checked or unchecked.
    // Param idx refers to which toggle is being changed.
    void update_toggle_value(int idx){
        switch(idx){
            case 1: // Checkbox for borders
                fluid.borders = BorderToggle.isOn;  
                fluid.clear_boarders();              
                break;
            case 2: // Checkbox for Preset Scene
                fluid.static_source = PresetToggle.isOn;
                fluid.clear_scene();
                if (fluid.static_source){
                    fluid.set_obstacles();
                    fluid.borders = false;
                    BorderToggle.isOn = false;
                    BorderToggle.interactable = false;
                }
                else{
                    BorderToggle.interactable = true;

                }
                break;
            default:
                break;

        }
    }
    // Is called when a button is called.
    // Used for the Clear scene button.
    void update_button_value(int idx){
        fluid.clear_scene();                
        
    }
}
