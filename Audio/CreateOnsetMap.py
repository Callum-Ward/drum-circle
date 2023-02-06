import json
import librosa
import numpy as np

def main():
    hop_length = 512
    x, sr = librosa.load('./drakkar.mp3')

    onset_frames = librosa.onset.onset_detect(x, sr=sr, wait=1, pre_avg=1, post_avg=1, pre_max=1, post_max=1)
    onset_times = librosa.frames_to_time(onset_frames)

    onset_env = librosa.onset.onset_strength(x, sr=sr, aggregate=np.median)
    times = librosa.times_like(onset_env, sr=sr, hop_length=hop_length)

    tempo, beats = librosa.beat.beat_track(x, sr=sr)
    beat_times = librosa.frames_to_time(beats)

    duration = librosa.get_duration(y=x, sr=sr)

    print(len(onset_env))
    print(len(onset_times))

    data = []
    for i in range(0, int(round(duration, 2) * 100)):
        timestamp = {"isOnset": False, "isBeat": False, "strength": 0.0}
        for t in onset_times:
            if int(round(t, 2) * 100) == i:
                timestamp["isOnset"] = True
                break
        for t in beat_times:
            if int(round(t, 2) * 100) == i:
                timestamp["isBeat"] = True
                break
        data.append(timestamp)
                
    with open("Dataset.json", "w") as f:
        json.dump(data, f)

main()