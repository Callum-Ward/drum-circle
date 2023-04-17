using System;
using System.Collections;
using UnityEngine;

public class FreestyleHandler  {

    private BeatTransfer beatTransfer;
    private int playerCount;

    private float[] soloSchedule = {10f};
    private float[] soloDurations = {200f};
    private float[] transferSchedule = {10000f};
    private float[] transferDurations = {200f};
    private int scheduleIndex = 0;
    private int activeSoloist = -1;

    public FreestyleHandler(int playerCount)
    {
        this.playerCount = playerCount;
        this.beatTransfer = null;
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
            int oneShot = audioManager.PlayRandomOneShot(velocity);
            this.beatTransfer.transferBeat(spawner, playerIndex, drumIndex, oneShot, velocity);
        } 
        else if(this.activeSolo())
        {
            if(playerIndex != this.activeSoloist)
            {
                return;
            }
            int timeAtNearestNote = audioAnalyser.timeAtNearestNote(playerIndex, drumIndex, audioManager.activeSources[0].time);
            if(timeAtNearestNote != -1)
            {
                //spawner.spawn(playerIndex, 1 - drumIndex, 1, 0, "rising");
                audioManager.PlayDrumTrackAtTime(playerIndex, (int)Math.Floor((double)(timeAtNearestNote / 1000)));
            }
        }
    }

    public void handleFreestyle(RhythmSpawner spawner, AudioManager audioManager, float time)
    {
        if(time >= transferSchedule[scheduleIndex] && this.beatTransfer == null)
        {
            this.beatTransfer = new BeatTransfer(0, 1);
            spawner.setFreestyleMode("transfer", this.beatTransfer, -1);

            audioManager.FadeOutDrumTrack(0);
            audioManager.FadeOutDrumTrack(1);
            audioManager.FadeOutDrumTrack(2);
            
        }
        else if(time >= transferSchedule[scheduleIndex] + transferDurations[scheduleIndex] && this.beatTransfer != null)
        {
            spawner.setFreestyleMode("none", this.beatTransfer, -1);

            audioManager.FadeInDrumTrack(this.beatTransfer.getProvider(), "slow");
            audioManager.FadeInDrumTrack(this.beatTransfer.getRecipient(), "slow");

            this.beatTransfer = null;
            this.scheduleIndex += 1;
        }
        else if(time >= soloSchedule[scheduleIndex] && !this.activeSolo())
        {
            this.activeSoloist = scheduleIndex;
            spawner.setFreestyleMode("solo", this.beatTransfer, this.activeSoloist);

            audioManager.FadeOutDrumTrack(0);
            audioManager.FadeOutDrumTrack(1);
            audioManager.FadeOutDrumTrack(2);
            
        }
        else if(time >= soloSchedule[scheduleIndex] + soloDurations[scheduleIndex] && this.activeSolo())
        {
            spawner.setFreestyleMode("none", this.beatTransfer, -1);
            audioManager.FadeInDrumTrack(0, "slow");
            audioManager.FadeInDrumTrack(1, "slow");
            audioManager.FadeInDrumTrack(2, "slow");

            this.activeSoloist = -1;
            this.scheduleIndex += 1;
        }
    }
}