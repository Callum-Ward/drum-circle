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

    public void TransferBeat(RhythmSpawner spawner, int playerIndex, int drumIndex, float velocity)
    {
        if(playerIndex == this.provider)
        {
            spawner.spawn(this.recipient + 1, 1 - drumIndex, 1);
        }
    }
}