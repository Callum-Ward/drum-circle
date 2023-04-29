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

public class UIGlowEffect
{
    protected int glowStage;
    protected float glowStrength;
    protected float glowRate;
    protected Color glowColorMin;
    protected Color glowColorMax;
    protected bool repeat;
    protected bool overriding;

    public UIGlowEffect()
    {
        this.glowStage = 0;
        this.glowRate = 0f;
        this.glowStrength = 0f;
        this.glowColorMin = new Color(0f, 0f, 0f, 0.4f);
        this.glowColorMax = new Color(0f, 0f, 0f, 0.4f);
        this.repeat = false;
        this.overriding = false;
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