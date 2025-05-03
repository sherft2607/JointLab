# (c) Meta Platforms, Inc. and affiliates. Confidential and proprietary.
# -*- coding: utf-8 -*-

import argparse
from pathlib import Path
import numpy as np
import math

import pyvrs

import sys

from PIL import Image
from pixelmatch.contrib.PIL import pixelmatch
from matplotlib import pyplot as plt

class Stream:
    def __init__(self, streamId:str, startTime:int, filtered_reader:pyvrs.filter.FilteredVRSReader) -> None:
        self.streamId = streamId
        self.startTime = startTime
        self.filtered_reader = filtered_reader


DEVICE_INDEPENDENT_RGBIMAGE = "8003"
ANNOTATION_RECORDABLE_CLASS = "400" 

# Display the image of the frames passed in for the specified camera angles
def displayFrames(record:  Image, replay: Image) -> None:
    plt.figure(figsize=(20, 10))

    # Create a plot to present each image and
    # adjust the image according to the frames value
    plt.subplot(1, 2, 1)
    image = np.asarray(record)
    plt.imshow(image, vmin=0, vmax=255, cmap="gray")
    plt.title("Record")
    plt.axis("off")

    plt.subplot(1, 2, 2)
    image = np.asarray(replay)
    plt.imshow(image, vmin=0, vmax=255, cmap="gray")
    plt.title("Replay")
    plt.axis("off")

    plt.show()

def getStreams(reader:pyvrs.SyncVRSReader, sId: str) -> list[Stream]:
    ret = []

    streamIds = reader.stream_ids
    # print(streamIds)

    for streamId in streamIds:
        if not streamId.startswith(sId):
            continue

        streamIdx = streamId[len(sId)+1]

        # NOTE: This doesn't separate the streams by index, only by type.
        # TODO: Handle multiple image streams in the same VRS file by iterating over reader.stream_ids() to find the matches
        filtered_reader = reader.filtered_by_fields(
                            stream_ids = {streamId},
                            record_types = {'configuration'},
                        )
        startTime = filtered_reader[0].timestamp
        filtered_reader = reader.filtered_by_fields(
                            stream_ids = {streamId},
                            record_types = {'data'},
                        )
        ret.append( Stream(streamId, startTime, filtered_reader) )
        # print(streamId, streamIdx, startTime)

    return ret

def getSnapshots(stream:Stream) -> list[(int, int)]:
    ret = []
    for record in stream.filtered_reader:
        # print(record)
        meta = record.metadata_blocks[0]
        # print(record.timestamp, meta['iValue'])
        numFrames = int(meta['iValue'])
        ret.append({'numFrames': numFrames, 'timestamp': record.timestamp-stream.startTime})

    return ret

def getTimestamp(stream:Stream, idx: int) -> float:
    # print(idx, len(stream.filtered_reader))
    if idx >= len(stream.filtered_reader):
        raise IndexError("The sample index is out of range of the stream") 
    record = stream.filtered_reader[idx]
    return record.timestamp-stream.startTime

def getFrameIdx(stream:Stream, timestamp:float) -> int:
    nextIdx = 0
    selectedIdx = len(stream.filtered_reader)-1
    record = None
    mindiff = 1e11
    while nextIdx < len(stream.filtered_reader):
        record = stream.filtered_reader[nextIdx]
        diff = abs(timestamp - (record.timestamp-stream.startTime))
        if diff < mindiff:
            mindiff = diff
            selectedIdx = nextIdx
        nextIdx += 1
    # use the last record
    return selectedIdx

def getFrame(stream:Stream, frameIndex:int):
    if frameIndex < 0 or frameIndex >= len(stream.filtered_reader):
        raise IndexError("index out of rangeof the stream")

    record = stream.filtered_reader[frameIndex]
    # print("getFrame:", frameIndex, record)
    # check that the image exists
    if len(record.image_blocks) == 0:
        return {'image':None, 'timestamp': record.timestamp}
    pixels = np.array(record.image_blocks[0])
    # print(record, f'shape={pixels.shape}')
    image = Image.fromarray(np.uint8(pixels)).convert('RGBA')
    # print(image.format, image.size, image.mode)
    return {'image':image, 'timestamp': record.timestamp}

def formatTime(time:float):
    return "{:.3f}sec".format(time)

def compareImages(diffs_output_path:str, threshold:float, sampleFrame, sampleIdx:int, matchFrame, matchIdx:int, prefix:str) -> int:
    img_diff = None
    if(diffs_output_path):
        img_diff = Image.new("RGBA", matchFrame['image'].size)

    numPixelsDiff = pixelmatch(sampleFrame['image'], matchFrame['image'], output=img_diff, threshold=threshold, includeAA=True)

    if(diffs_output_path):
        sampleFrame['image'].save(f"{diffs_output_path}/{prefix}-sampleIdx-{sampleIdx}.png")
        matchFrame['image'].save(f"{diffs_output_path}/{prefix}-matchIdx-{matchIdx}.png")
        img_diff.save(f"{diffs_output_path}/{prefix}-{numPixelsDiff}-diffs-sampleIdx-{sampleIdx}-matchIdx-{matchIdx}.png")

    return numPixelsDiff

def findTheMatchingFrame(frame_stream:Stream, frame_name:str, target_frame_stream:Stream, target_frame_name:str, sampleIdx:int, max_test_frames:int, diffs_output_path:str, threshold:float) -> (int, int, dict):  
    minPixelsDiff = 1e11
    bestMatch = None
    numberOfPixels = 0;

    sampleTS = getTimestamp(frame_stream, sampleIdx)
    sampleFrame = getFrame(frame_stream, sampleIdx)
    matchBestIdx = getFrameIdx(target_frame_stream, sampleTS)
    # search around the frame of interest until we find the closest match

    # check if the sample frame is valid
    if sampleFrame['image'] is None:
        return (minPixelsDiff,numberOfPixels, bestMatch)

    # get the number of pixels in the sample frame
    numberOfPixels = sampleFrame['image'].size[0] * sampleFrame['image'].size[1]

    # find the matching frame in the target_frame_stream
    for matchIdx in range(matchBestIdx - 1, matchBestIdx + 2):
        # continue the loop if out of range of the target stream
        if  matchIdx < 0 or matchIdx >= len(target_frame_stream.filtered_reader):
            continue

        matchFrame = getFrame(target_frame_stream, matchIdx)
        
        # check if the match frame is valid
        if matchFrame['image'] is None:
            continue

        numPixelsDiff = compareImages(diffs_output_path, threshold, sampleFrame, sampleIdx, matchFrame, matchIdx, f"{target_frame_name}-{frame_name}")

        print("  compared ", frame_name, sampleIdx, " and ", target_frame_name, matchIdx, ": #px diff=", numPixelsDiff)
        if numPixelsDiff > minPixelsDiff:
            print("  Comparison pixel diff ", numPixelsDiff," current match minimum pixel diff" , minPixelsDiff, " stopping search as the diff will only get worse")
            break

        minPixelsDiff = numPixelsDiff
        bestMatch = {'sampleStream':frame_name, 'minPixelsDiff':numPixelsDiff, 'sampleFrame':sampleFrame, 'sampleIdx':sampleIdx, 'matchStream':target_frame_name, 'matchFrame':matchFrame, 'matchIdx':matchIdx}

        if(minPixelsDiff == 0):
            break
    return (minPixelsDiff,numberOfPixels, bestMatch)

def compareVRS(record:str, replay:str, sample_location:float, max_test_frames, best_match_pixels_diff_threshold:int, show_matches:bool, diffs_output_path:str, threshold:float) -> int:
    """
    Compare two VRS files and return 0 if match found.
    :param record: The path to the VRS file to compare against
    :param replay: The path to the VRS file to compare against
    :param sample_location: The location in the record stream to compare against. 0=beginning, 0.5=middle, 1=end
    :param max_test_frames: max number of frames to test per group in each direction
    :param best_match_pixels_diff_threshold: The maximum number of pixels that can be different between the record and replay frames to be considered a match
    :param show_matches: Whether or not to display the corresponding screenshots side-by-side once a match is identified
    :param diffs_output_path: The path to the directory to store the diffs in
    :param threshold: The threshold for the pixelmatch algorithm to determine if two pixels are different. 0.1 is a good default.
    :return: 0 if match found, -1 if match found but too much pixel diff, -2 if no match found
    """
    record_path = Path(record)
    record_reader = pyvrs.SyncVRSReader(record_path)

    replay_path = Path(replay)
    replay_reader = pyvrs.SyncVRSReader(replay_path)

    # NOTE: only use the first snapshot stream
    record_snapshot_stream = getStreams(record_reader, ANNOTATION_RECORDABLE_CLASS)[0]
    replay_snapshot_stream = getStreams(replay_reader, ANNOTATION_RECORDABLE_CLASS)[0]

    # print(f'snapshots for {record_path}')
    record_groups = getSnapshots(record_snapshot_stream)
    # print(f'snapshots for {replay_path}')
    replay_groups = getSnapshots(replay_snapshot_stream)

    if len(record_groups) != len(replay_groups):
        raise ValueError('the number of snapshots record group is different than the number of snapshots replay group')

    print("Comparing", record_path, "with", replay_path, ": num groups=", len(record_groups))

    # NOTE: only use the first frame stream for now
    record_frame_stream = getStreams(record_reader, DEVICE_INDEPENDENT_RGBIMAGE)[0]
    replay_frame_stream = getStreams(replay_reader, DEVICE_INDEPENDENT_RGBIMAGE)[0]

    groupOffset = 0
    for groupIdx in range(0, len(record_groups)):
        if record_groups[groupIdx]['numFrames'] != replay_groups[groupIdx]['numFrames']:
            raise ValueError('the number of snapshots in the record group should be the same as the number of snapshots in the replay groups')

        numFrames = record_groups[groupIdx]['numFrames']

        sample_location = sample_location
        max_test_frames = max_test_frames
        minPixelsDiff = 1e11
        bestMatch = None
        for i in range(0, max_test_frames):
            sampleIdx = groupOffset+int(numFrames*sample_location + i)

            print("Group", groupIdx, ":numFrames=", numFrames,"sampleIndex=", sampleIdx)

            # break if sampleIdx out of range of the record stream
            if  sampleIdx < 0 or sampleIdx >= len(record_frame_stream.filtered_reader):
                break

            # check the replay against the record
            (PixelsDiff, numberOfPixels, match) = findTheMatchingFrame(record_frame_stream, "record", replay_frame_stream, "replay", sampleIdx, max_test_frames, diffs_output_path, threshold)
            if minPixelsDiff > PixelsDiff:
                minPixelsDiff = PixelsDiff
                bestMatch = match
            if minPixelsDiff < numberOfPixels * best_match_pixels_diff_threshold:
                print("best match is found so stop searching")
                break

        # report the best match
        if bestMatch:
            print(f" Best sample is {bestMatch['sampleStream']}#{bestMatch['sampleIdx']} ts={formatTime(bestMatch['sampleFrame']['timestamp'])}, best match is {bestMatch['matchStream']}#{bestMatch['matchIdx']} ts={formatTime(bestMatch['matchFrame']['timestamp'])}, #px diff={bestMatch['minPixelsDiff']}")
            if(show_matches):
                displayFrames(bestMatch['sampleFrame']['image'], bestMatch['matchFrame']['image'])
            if(bestMatch['minPixelsDiff']> numberOfPixels * best_match_pixels_diff_threshold):
                print(" The best match's pixels difference exceeds the threshold of ", best_match_pixels_diff_threshold * 100, "%. So this is a mismatch")
                return -1
            else:
                print(" The best match's pixels difference is less than the threshold of ", best_match_pixels_diff_threshold * 100, "%. So this is a match")
        else:
            print(" no match found!")
            return -2

        groupOffset += numFrames

    return 0

def main():

    parser = argparse.ArgumentParser(description='use pixelmatch to diff vrs files')
    parser.add_argument('record', type=str, help="VRS reference recording file to compare against")
    parser.add_argument('replay', type=str, help="VRS replay to check")
    # parser.add_argument('record frames')
    parser.add_argument('--threshold', type=float, required=False, default=0.1, help="threshold over which the pixels are considered to be different")
    parser.add_argument('--sample_location', type=float, required=False, default=0.5, help="where in the group to test: 0=beginning, 0.5=middle, 1=end")
    parser.add_argument('--max_test_frames', type=int, required=False, default=3, help="max number of frames to test per group in each direction")
    parser.add_argument('--show_matches', type=bool, required=False, default=False, help="display the corresponding screenshots side-by-side once a match is identified")
    parser.add_argument('--diffs_output_path', type=str, required=False, default=None, help="output folder for image diffs")
    parser.add_argument('--best_match_pixels_diff_threshold', type=int, required=False, default=0.05, help="threshold on the pixels diff in percentage of the best match")

    args = parser.parse_args()

    res = compareVRS(args.record, args.replay, args.sample_location, args.max_test_frames, args.best_match_pixels_diff_threshold, args.show_matches, args.diffs_output_path, args.threshold)
    assert 0 == res, f"Vrs comparison failed res: {res}"
    return 0

if __name__ == "__main__":
    main()
