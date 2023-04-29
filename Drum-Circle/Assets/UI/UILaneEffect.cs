using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.UIElements;
using UnityEngine.UIElements.StyleSheets;
using UnityEngine.UIElements.Experimental;
using System.Linq;

public class UILaneEffect : UIGlowEffect
{
    private bool shaking;
    private float shakeTime;
    private float shakeDuration;

    public UILaneEffect() : base()
    {
        this.shaking = false;
        this.shakeTime = 0f;
        this.shakeDuration = 0.3f;
    }

    public void SetMode(string mode, float glowStrength)
    {
        switch(mode)
        {
            case "miss":
                if(this.overriding)
                {
                    break;
                }
                this.glowColorMin = new Color(glowStrength * 0.01f, 0f, 0f, 0.4f + glowStrength * 0.025f);
                this.glowColorMax = new Color(glowStrength * 0.05f, 0f, 0f, 0.4f + glowStrength * 0.05f);
                this.glowStrength = glowStrength;
                this.glowRate = 0.1f;
                this.shaking = true;
                this.repeat = false;
                this.overriding = false;
                if(this.glowStage == 0)
                {
                    this.glowStage = 1;
                }
                break;
            case "combo":
                if(this.overriding)
                {
                    break;
                }
                if(glowStrength >= 15)
                {
                    glowStrength = 15;
                }
                this.glowColorMin = new Color(0f, 0.12f, 0f, 0.5f);
                this.glowColorMax = new Color(0f, glowStrength * 0.04f, 0f, 0.4f + glowStrength * 0.02f);
                this.glowStrength = glowStrength;
                this.glowRate = 0.05f;
                this.shaking = false;
                this.repeat = true;
                this.overriding = false;
                if(this.glowStage == 0)
                {
                    this.glowStage = 1;
                }
                break;
            case "freestyle":
                this.glowColorMin = new Color(0f, 0.3f, 0f, 0.4f);
                this.glowColorMax = new Color(0f, 0.5f, 0f, 0.6f);
                this.glowStrength = glowStrength;
                this.glowRate = 0.05f;
                this.shaking = false;
                this.repeat = true;
                this.overriding = true;
                this.glowStage = 1;
                break;
            case "none":
                this.glowColorMin = new Color(0f, 0f, 0f, 0.4f);
                this.glowStrength = 0f;
                this.glowRate = 0f;
                this.shaking = false;
                this.repeat = false;
                this.overriding = false;
                break;
        }
    }
    
    public float getNextShakeValue()
    {
        if(this.shaking && this.shakeTime >= this.shakeDuration)
        {
            this.shakeTime = 0f;
            this.shaking = false;
            return 0f;
        }
        else if(this.shaking)
        {
            this.shakeTime += Time.deltaTime;
            return (UnityEngine.Random.Range(-1f, 1f) * this.glowStrength);
        }
        return 0f;
    }
}