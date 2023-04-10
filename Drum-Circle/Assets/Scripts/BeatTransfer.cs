using System;
using System.Collections;
using UnityEngine;

public class BeatTransfer {
    private const float delay = 1.0f;
    private int recipient;
    private int provider;

    public BeatTransfer(int provider, int recipient)
    {
        this.provider = provider;
        this.recipient = recipient;
    }

    public int getRecipient()
    {
        return this.recipient;
    }

    public int getProvider()
    {
        return this.provider;
    }

    public int nextPlayer(int playerCount)
    {
        return playerCount - (this.provider + this.recipient);
    }

    public void transferBeat(RhythmSpawner spawner, int playerIndex, int drumIndex, float velocity)
    {
        if(playerIndex == this.provider)
        {
            spawner.spawnWithDelay(this.recipient + 1, 1 - drumIndex, 1, 1.0f);
        }
    }
}