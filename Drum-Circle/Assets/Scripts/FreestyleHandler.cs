using System;
using System.Collections;
using UnityEngine;

public class FreestyleHandler  {

    private BeatTransfer beatTransfer;
    private int playerCount;

    private float[] schedule = {2000f};
    private float[] durations = {20f};
    private int scheduleIndex = 0;

    public FreestyleHandler(int playerCount)
    {
        this.playerCount = playerCount;
        this.beatTransfer = null;
    }

    public bool active()
    {
        return this.beatTransfer != null;
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

    public void handleDrumHitFreestyle(RhythmSpawner spawner, AudioManager audioManager, int playerIndex, int drumIndex, float velocity, float delay)
    {
        if(this.beatTransfer != null)
        {
            if(playerIndex != this.beatTransfer.getProvider())
            {
                return;
            }
            int oneShot = audioManager.PlayRandomOneShot(velocity);
            this.beatTransfer.transferBeat(spawner, playerIndex, drumIndex, oneShot, velocity);
        }
    }

    public void handleFreestyle(RhythmSpawner spawner, AudioManager audioManager, float time)
    {
        if(time >= schedule[scheduleIndex] && this.beatTransfer == null)
        {
            this.beatTransfer = new BeatTransfer(0, 1);
            spawner.setFreestyleMode(this.beatTransfer, true);

            audioManager.FadeOutDrumTrack(0);
            audioManager.FadeOutDrumTrack(1);
            
        }
        else if(time >= schedule[scheduleIndex] + durations[scheduleIndex] && this.beatTransfer != null)
        {
            spawner.setFreestyleMode(null, false);

            audioManager.FadeInDrumTrack(this.beatTransfer.getProvider(), "slow");
            audioManager.FadeInDrumTrack(this.beatTransfer.getRecipient(), "slow");

            this.beatTransfer = null;
            this.scheduleIndex += 1;
        }
    }
}