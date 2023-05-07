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

public class UITargetEffect : UIGlowEffect
{
    private int swellStage;
    private float swellEffect;
    private float swellEffectInc;
    private float swellEffectDec;

    public UITargetEffect() : base()
    {
        this.swellStage = 0;
        this.swellEffect = 1f;
        this.swellEffectInc = 0f;
        this.swellEffectDec = 0f;
    }

    public void SetMode(string mode, float glowStrength = 0f)
    {
        switch(mode)
        {
            case "swell":
                this.swellStage = 1;
                this.swellEffectInc = 0.15f;
                this.swellEffectDec = 0.05f;
                break;
            case "winning":
                if(this.overriding)
                {
                    break;
                }
                if(glowStrength >= 15)
                {
                    glowStrength = 15;
                }
                this.glowColorMin = new Color(0.25f, 0.2f, 0.01f, 0.4f);
                this.glowColorMax = new Color(0.5f, 0.4f, 0.02f, 0.6f);
                this.glowStrength = glowStrength;
                this.glowIncRate = 0.05f;
                this.glowDecRate = 0.05f;
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
                this.glowIncRate = 0.05f;
                this.glowDecRate = 0.05f;
                this.repeat = true;
                this.overriding = true;
                this.glowStage = 1;
                break;
            case "none":
                this.glowColorMin = new Color(0f, 0f, 0f, 0.4f);
                this.glowStrength = 0f;
                this.glowIncRate = 0f;
                this.glowDecRate = 0f;
                this.repeat = false;
                this.overriding = false;
                break;
        }
    }

    public float getNextScaleEffect()
    {
        if(this.swellStage == 1)
        {
            if(this.swellEffect < 1.15f)
            {
                this.swellEffect += this.swellEffectInc;
            }
            else
            {
                this.swellStage = 2;
            }
        }
        else if(this.swellStage == 2)
        {
            if(this.swellEffect > 1f)
            {
                this.swellEffect -= this.swellEffectDec;
            }
            else
            {
                this.swellStage = 0;
            }
        }
        return this.swellEffect;
    }
}