import json
import librosa
import numpy as np

files = [
    #('./drakkar.mp3', 'drakkar_data.json'),
    #('./break_130.wav', 'break_130_data.json'),
    #('./download.wav', 'download_data.json'),
    #('./BiBDrumsBass.mp3', 'BiBDrumsBass.json')
]

def create_onset_map(path, output):
    hop_length = 512
    x, sr = librosa.load(path)

    D = np.abs(librosa.stft(x))
    onset_env_times = librosa.times_like(D)

    onset_env = librosa.onset.onset_strength(x, sr=sr, aggregate=np.median)
    onset_frames = librosa.onset.onset_detect(x, sr=sr, onset_envelope=onset_env, wait=1, pre_avg=1, post_avg=1, pre_max=1, post_max=1)
    onset_times = librosa.frames_to_time(onset_frames)

    tempo, beats = librosa.beat.beat_track(x, sr=sr)
    beat_times = librosa.frames_to_time(beats)

    duration = librosa.get_duration(y=x, sr=sr)

    data = []
    for i in range(0, int(round(duration, 2) * 100)):
        timestamp = {"isOnset": False, "isBeat": False, "strength": 0.0}
        for t in beat_times:
            if int(round(t, 2) * 100) == i:
                timestamp["isBeat"] = True
                break
        for t in onset_times:
            if int(round(t, 2) * 100) == i and not timestamp["isBeat"]:
                timestamp["isOnset"] = True
                break
        for j in range(0, len(onset_env_times)):
            if int(round(onset_env_times[j], 2) * 100) == i and (timestamp["isBeat"] or timestamp["isOnset"]):
                timestamp["strength"] = str(onset_env[j])
                break
        data.append(timestamp)
                
    with open(output, "w") as f:
        json.dump(data, f)

def main():
    create_onset_map('./download.wav', 'd.json')
    for path, output in files:
        create_onset_map(path, output)

main()