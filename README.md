# ArchiveManager
This project allows extracting files from various containers. Based on the open framework, it is easy to add support for any container.

## Project Status
(**PAUSED**)

Only 'extract' functions are implemented. None of the modifying functions or 'save' is working.

## Project Intention
This project is built to process middle-ware containers which are used in video games. It helps extracting and replacing textures, sounds or other files from games.

## Containers Supported
All containers are named based on the file identifier, which is the first 4 bytes of a file.

### UNI2 (Union 2)
A container with identifier 0x554e4932 (ASCII: UNI2).

This container support is developed to extract files for Xbox 360 game "俺の嫁 ～あなただけの花嫁～".

For further information: http://www.snailium.com/archives/287/

### MAAB
MAAB is a package of various images to conform an animated image. (Missing documentation)

### CWAB
CWAB is a package of various images to conform an animated image. First observed in Xbox 360 game "俺の嫁 ～あなただけの花嫁～". (Missing documentation)

### Pak
This refers to the Pak container observed in Xbox 360 game "Time Leap".

This container doesn't have an identifier. It starts with an unsigned integer to indicate number of files included in the package. And then the start offsets of the files. The file sizes are determined by calculating the difference of the start offsets of two files.

### AXCS
(Missing documentation)

### AFS
Originally by Sega. A container with identifier 0x441465300 (ASCII: AFS).

This container support is developed to extract files fro Xbox 360 game "Cross Channel".

For further information: http://www.snailium.com/2012-08-28-xbox-360-afs-cross-channel/

### CPK (CPack or CRI-Pack)
A container with identifier 0x43504b20 (ASCII: CPK), used by CRI middle-ware. (Missing documentation)

Note: the codes used to unpack CPack file is converted from some program from Internet. I cannot remember the original source since this is a project long time ago.

### LNK4
A container with identifier 0x4C4E4B34 (ASCII: LNK4).

This container support is developed to extract files fro Xbox 360 game "Code_18".

For further information: http://www.snailium.com/2012-08-28-xbox-360-lnk4-container-code-18/

