using System;
using System.Collections;
using UnityEngine;

public class FreestyleHandler  {

    private BeatTransfer beatTransfer;
    private int playerCount;

    private float[] soloSchedule;
    private float[] soloDurations;
    private float[] transferSchedule;
    private float[] transferDurations;
    public int scene = 1;
    private int scheduleIndex = 0;
    private int activeSoloist = -1;
    private bool noticeActive = false;
    private float noticeDuration = 2f;

    public FreestyleHandler(int playerCount)
    {
        this.playerCount = playerCount;
        this.beatTransfer = null;
    }

    public void setScene(int sceneNo)
    {
        if (sceneNo > 0 && sceneNo < 4)
        {
            scene = sceneNo;
            switch(scene)
            {
                case 1: //forest
                    soloSchedule = new float[]{10000f, 10000f};
                    soloDurations = new float[]{10000f, 10000f};
                    break;
                case 2: //mountains
                    soloSchedule = new float[]{10000f, 10000f};
                    soloDurations = new float[]{10000f, 10000f};
                    break;
                case 3: //beach
                    soloSchedule = new float[]{153f, 180f, 206f, 10000f};
                    soloDurations = new float[]{13f, 18f, 14f, 10000f};
                    break;
            }
        }
    }

    public bool active()
    {
        return this.beatTransfer != null;
    }

    public bool activeSolo()
    {
        return this.activeSoloist >= 0;
    }

    public int nextPlayerIndex()
    {
        return this.beatTransfer.nextPlayer(this.playerCount);
    }

    public bool checkMiss(int queueNo)
    {
        if(this.beatTransfer != null)
        {
            return beatTransfer.getProvider() != queueNo / 2;
        }
        return false;
    }

    public void handleDrumHitFreestyle(
        RhythmSpawner spawner,
        AudioManager audioManager,
        AudioAnalyser audioAnalyser,
        int playerIndex,
        int drumIndex,
        float velocity,
        float delay
    ){
        if(this.beatTransfer != null)
        {
            if(playerIndex != this.beatTransfer.getProvider())
            {
                return;
            }
            int oneShotIndex = audioManager.PlaySoloOneShot(playerIndex, drumIndex, velocity);
            this.beatTransfer.transferBeat(spawner, playerIndex, drumIndex, oneShotIndex, velocity);
        } 
        else if(this.activeSoloist == playerIndex)
        {
            audioManager.PlayDrumOneShot(playerIndex * 2 + drumIndex, velocity);
            spawner.spawn(playerIndex + 1, 1 - drumIndex, velocity > 0.5f ? 2 : 1, 0, "rising");
        }
    }

    public void handleFreestyle(RhythmSpawner spawner, BeatUI beatUI, AudioManager audioManager, float time)
    {
        if(time >= soloSchedule[scheduleIndex] && !this.activeSolo())
        {
            this.activeSoloist = scheduleIndex;
            spawner.setFreestyleMode("solo", this.beatTransfer, this.activeSoloist);
            beatUI.toggleFreestyle(this.activeSoloist, true);

            this.noticeActive = true;

            audioManager.FadeOutDrumTrack(0);
            audioManager.FadeOutDrumTrack(1);
            audioManager.FadeOutDrumTrack(2);

            audioManager.ReduceToBackgroundLayer();
            
        }
        else if(time >= soloSchedule[scheduleIndex] + noticeDuration && this.activeSolo() && this.noticeActive)
        {
            //beatUI.FreestyleNoticeStop();
            this.noticeActive = false;
        }
        else if(time >= soloSchedule[scheduleIndex] + soloDurations[scheduleIndex] && this.activeSolo())
        {
            spawner.setFreestyleMode("none", this.beatTransfer, -1);
            beatUI.toggleFreestyle(this.activeSoloist, false);

            audioManager.FadeInDrumTrack(0, "slow");
            audioManager.FadeInDrumTrack(1, "slow");
            audioManager.FadeInDrumTrack(2, "slow");

            audioManager.FadeInFromBackgroundLayer();

            this.activeSoloist = -1;
            this.scheduleIndex += 1;
        }
    }
}