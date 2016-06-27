# ArchiveManager
This project allows extracting files from various containers. Based on the open framework, it is easy to add support for any container.

## Project Status
**PAUSED**

Only 'extract' functions are implemented. Both 'import' and 'save' are not working.

## Project Intention
This project is built to process middle-ware containers which are used in video games. It helps extracting and replacing textures, sounds or other files from games.

## Containers Supported
All containers are named based on the file identifier, which is the first 4 bytes of a file.

### AFS
Originally by Sega. A container with identifier 0x441465300 (ASCII: AFS).

This container support is developed to extract files fro Xbox 360 game "Cross Channel".

For further information: http://www.snailium.com/2012-08-28-xbox-360-afs-cross-channel/

### AXCS
Missing documentation.

### CRI
A container used by CRI middle-ware. Missing documentation.

### LNK4
A container with identifier 0x4C4E4B34 (ASCII: LNK4).

This container support is developed to extract files fro Xbox 360 game "Code_18".

For further information: http://www.snailium.com/2012-08-28-xbox-360-lnk4-container-code-18/

### MAAB
Missing documentation.

### CWAB
First observed in Xbox 360 game "俺の嫁 ～あなただけの花嫁～". Missing documentation.

### UNI2 (Union 2)
A container with identifier 0x554e4932 (ASCII: UNI2).

This container support is developed to extract files for Xbox 360 game "俺の嫁 ～あなただけの花嫁～".

For further information: http://www.snailium.com/archives/287/
