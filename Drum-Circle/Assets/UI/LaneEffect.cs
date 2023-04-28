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

public class LaneEffect 
{
    private int glowStage;
    private float glowStrength;
    private float glowRate;
    private Color glowColorMin;
    private Color glowColorMax;
    private bool shaking;
    private float shakeTime;
    private float shakeDuration;
    private bool repeat;
    private bool overriding;

    public LaneEffect()
    {
        this.glowStage = 0;
        this.glowRate = 0f;
        this.glowStrength = 0f;
        this.glowColorMin = new Color(0f, 0f, 0f, 0.4f);
        this.glowColorMax = new Color(0f, 0f, 0f, 0.4f);
        this.shaking = false;
        this.shakeTime = 0f;
        this.shakeDuration = 0.3f;
        this.repeat = false;
        this.overriding = false;
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
                this.glowColorMax = new Color(glowStrength * 0.1f, 0f, 0f, 0.4f + glowStrength * 0.075f);
                this.glowStrength = glowStrength;
                this.glowRate = 0.05f;
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
                this.glowColorMin = new Color(0f, 0.05f, 0f, 0.4f);
                this.glowColorMax = new Color(0f, glowStrength * 0.02f, 0f, 0.4f + glowStrength * 0.01f);
                this.glowStrength = glowStrength;
                this.glowRate = 0.02f;
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

    public Color getTransitionColor(Color currentGlow)
    {
        float redInc = (this.glowColorMin.r - currentGlow.r) * 0.5f;
        float greenInc = (this.glowColorMin.g - currentGlow.g) * 0.5f;
        float blueInc = (this.glowColorMin.b - currentGlow.b) * 0.5f;
        float alphaInc = (this.glowColorMin.a - currentGlow.a) * 0.5f;
        return new Color(currentGlow.r + redInc, currentGlow.g + greenInc, currentGlow.b + blueInc, currentGlow.a + alphaInc);
    }

    public Color getNextGlowValue(Color currentGlow)
    {
        float redInc = (this.glowColorMax.r - this.glowColorMin.r) * this.glowRate;
        float greenInc = (this.glowColorMax.g - this.glowColorMin.g) * this.glowRate;
        float blueInc = (this.glowColorMax.b - this.glowColorMin.b) * this.glowRate;
        float alphaInc = (this.glowColorMax.a - this.glowColorMin.a) * this.glowRate;

        if(this.glowStage == 1) 
        {
            if(currentGlow.a >= this.glowColorMax.a)
            {
                this.glowStage = 2;
            }
            return new Color(currentGlow.r + redInc, currentGlow.g + greenInc, currentGlow.b + blueInc, currentGlow.a + alphaInc);
        }
        else if(this.glowStage == 2) 
        {
            if(currentGlow.a <= this.glowColorMin.a)
            {
                if(this.repeat)
                {
                    this.glowStage = 1;
                }
                else{
                    this.glowStage = 0;
                    this.glowColorMax = new Color(0f, 0f, 0f, 0.4f);
                    return this.glowColorMax;
                }
            }
            return new Color(currentGlow.r - redInc, currentGlow.g - greenInc, currentGlow.b - blueInc, currentGlow.a - alphaInc);
        }
        this.glowColorMax = new Color(0f, 0f, 0f, 0.4f);
        return this.glowColorMax;
    }

}