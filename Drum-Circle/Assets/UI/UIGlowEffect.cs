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
    protected float glowIncRate;
    protected float glowDecRate;
    protected Color glowColorMin;
    protected Color glowColorMax;
    protected bool repeat;
    protected bool overriding;

    public UIGlowEffect()
    {
        this.glowStage = 0;
        this.glowIncRate = 0f;
        this.glowDecRate = 0f;
        this.glowStrength = 0f;
        this.glowColorMin = new Color(0f, 0f, 0f, 0.4f);
        this.glowColorMax = new Color(0f, 0f, 0f, 0.4f);
        this.repeat = false;
        this.overriding = false;
    }

    //Gets color between the base color of glow effect and the general base color to increase smoothness of transition
    public Color getTransitionColor(Color currentGlow)
    {
        float redInc = (this.glowColorMin.r - currentGlow.r) * 0.5f;
        float greenInc = (this.glowColorMin.g - currentGlow.g) * 0.5f;
        float blueInc = (this.glowColorMin.b - currentGlow.b) * 0.5f;
        float alphaInc = (this.glowColorMin.a - currentGlow.a) * 0.5f;
        return new Color(currentGlow.r + redInc, currentGlow.g + greenInc, currentGlow.b + blueInc, currentGlow.a + alphaInc);
    }


    //Gets the next color at this stage of the glow effect to apply to the required element
    public Color getNextGlowValue(Color currentGlow)
    {
        if(this.glowStage == 0)
        {
            this.glowColorMax = new Color(0f, 0f, 0f, 0.4f);
            return this.glowColorMax;
        }
        
        float glowRate = this.glowStage == 1 ? this.glowIncRate : this.glowDecRate;
        float redInc = (this.glowColorMax.r - this.glowColorMin.r) * glowRate;
        float greenInc = (this.glowColorMax.g - this.glowColorMin.g) * glowRate;
        float blueInc = (this.glowColorMax.b - this.glowColorMin.b) * glowRate;
        float alphaInc = (this.glowColorMax.a - this.glowColorMin.a) * glowRate;

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