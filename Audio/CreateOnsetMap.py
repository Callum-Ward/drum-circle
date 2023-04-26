import json
import librosa
import numpy as np

files = [
    #('./drakkar.mp3', 'drakkar_data.json'),
    #('./break_130.wav', 'break_130_data.json'),
    #('./download.wav', 'download_data.json'),
    #('./BiBDrumsBass.mp3', 'BiBDrumsBass.json'),
    #('./PrettyWomanDrums.mp3', 'PrettyWomanDrums.json'),
    #('./Jormungandr.mp3', 'Jormungandr.json')
    #('./tutorial1.wav', 'tutorial1.json')
    ('./DDC Oriental - Drum 1 - Taiko.wav', 'DD_L2_P1_json.json'),
    ('./DDC Oriental - Drum 2 - Khol.wav', 'DD_L2_P2_json.json'),
    ('./DDC Oriental - Drum 3 - Conga.wav', 'DD_L2_P3_json.json')
]

def print_progress_bar (iteration, total, prefix = '', suffix = '', decimals = 1, length = 100, fill = '█', printEnd = "\r"):
    """
    Call in a loop to create terminal progress bar
    @params:
        iteration   - Required  : current iteration (Int)
        total       - Required  : total iterations (Int)
        prefix      - Optional  : prefix string (Str)
        suffix      - Optional  : suffix string (Str)
        decimals    - Optional  : positive number of decimals in percent complete (Int)
        length      - Optional  : character length of bar (Int)
        fill        - Optional  : bar fill character (Str)
        printEnd    - Optional  : end character (e.g. "\r", "\r\n") (Str)
    """
    percent = ("{0:." + str(decimals) + "f}").format(100 * (iteration / float(total)))
    filledLength = int(length * iteration // total)
    bar = fill * filledLength + '-' * (length - filledLength)
    print(f'\r{prefix} |{bar}| {percent}% {suffix}', end = printEnd)
    # Print New Line on Complete
    if iteration == total: 
        print()

def create_onset_map(path, output, read_onset_strengths=False):
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

    onset_strength_mean = 0.0
    onset_strength_max = 0.0
    onset_strength_count = 0

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
        if read_onset_strengths:
            for j in range(0, len(onset_env_times)):
                if int(round(onset_env_times[j], 2) * 100) == i and (timestamp["isBeat"] or timestamp["isOnset"]):
                    if onset_env[j] > onset_strength_max:
                        onset_strength_max = onset_env[j]
                    onset_strength_mean += onset_env[j]
                    onset_strength_count += 1
                    timestamp["strength"] = str(onset_env[j])
                    break
        data.append(timestamp)
        print_progress_bar(i, int(round(duration, 2) * 100), prefix='Progress', suffix='Complete', length=50)

    if read_onset_strengths:
        onset_strength_mean = onset_strength_mean / onset_strength_count
    
    for d in data:
        if read_onset_strengths and (d['isOnset'] or d['isBeat']):
            d['strength'] = str((float(d['strength']) - onset_strength_mean) / onset_strength_max)
                
    with open(output, "w") as f:
        json.dump(data, f)
    print('data written')

def print_onset_map_from_json(file):
    f = open(file)
    data = json.load(f)
    for d in data:
        if d['isOnset'] or d['isBeat']:
            print(d)
    
    f.close()


def main():
    #print_onset_map_from_json('./BiBDrumsBass-str.json')
    #create_onset_map('./BiBDrumsBass.mp3', 'BiBDrumsBass.json')
    for path, output in files:
        create_onset_map(path, output)

main()