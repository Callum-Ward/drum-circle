using System;
using System.Collections;
using UnityEngine;

public class FreestyleHandler {

    private BeatTransfer beatTransfer;
    private int playerCount;

    private float[] schedule = {20f, 40f, 60f};
    private float[] durations = {10f, 10f, 10f};
    private float scheduleIndex = 0;

    public FreestyleHandler(int playerCount)
    {
        this.playerCount = playerCount;
        this.beatTransfer = null;
    }

    public int nextPlayerIndex()
    {
        return this.beatTransfer.nextPlayer(this.playerCount);
    }

    public void handleDrumHitFreestyle(RhythmSpawner spawner, int playerIndex, int drumIndex, float velocity, float delay)
    {
        if(this.beatTransfer != null)
        {
            this.beatTransfer.TransferWithDelay(spawner, playerIndex, drumIndex, velocity, delay);
        }
    }

    public void handleFreestyle(float time)
    {
        if(time >= schedule[scheduleIndex] && this.beatTransfer == null)
        {
            this.beatTransfer = new BeatTransfer(0, 1);
        }
        else if(time >= schedule[scheduleIndex] + durations[scheduleIndex] && this.beatTransfer != null)
        {
            this.beatTransfer = null;
            this.scheduleIndex += 1;
        }
    }
}