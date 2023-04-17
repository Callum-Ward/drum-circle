import json
import mido
import numpy as np

files = [
    #('./drakkar.mp3', 'drakkar_data.json'),
    #('./break_130.wav', 'break_130_data.json'),
    #('./download.wav', 'download_data.json'),
    #('./BiBDrumsBass.mp3', 'BiBDrumsBass.json'),
    #('./PrettyWomanDrums.mp3', 'PrettyWomanDrums.json'),
    #('./Jormungandr.mp3', 'Jormungandr.json')
    #('./tutorial1.wav', 'tutorial1.json')
    ('./DD_L3_Player1_Stem.wav', 'DD_L3_P1_json.json'),
    ('./DD_L3_Player2_Stem.wav', 'DD_L3_P2_json.json'),
    ('./DD_L3_Player3_Stem.wav', 'DD_L3_P3_json.json')
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

def create_midi_map(path, output):
    mid = mido.MidiFile(path, clip=True)
    for m in mid.tracks[2][:20]:
        print(m)


def main():
    create_midi_map('./DD_L3_P1_midi.mid', None)

main()