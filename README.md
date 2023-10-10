# KeyCorrect
Intercept keyboard with correct characters<br><br>
![image](https://github.com/MP3Martin/KeyCorrect/assets/60501493/c56b4e5c-1a1a-4bf0-83c3-a55cf0ae70f6)


## Info
- My first .NET project
- Made for personal usage (because handling different keyboard layouts / special characters would be too complex for this "simple project")
- **Only supports the most simple characters** (maybe try switching to english keyboard layout if it is typing weird characters)
- ⚠️ Use at own risk ⚠️, you chose to use this software

## First use
1. Download the latest EXE from [releases](https://github.com/MP3Martin/KeyCorrect/releases/latest) or build it from source using .NET 6
2. Right click on the EXE and choose **"Run as administrator"** (Only required for the first time to install the keyboard driver)
3. Read the program output. If it says *"Done! Restart your computer."*, then do just that **(important)**
4. After restarting, you can just run the downloaded EXE (simple double click, no admin permission required) and use it

## Example usage
- You copy *"text"* to your clipboard
- You launch this program
- You open notepad
- You press the PageUp key
- You type *"abcd"* on your keyboard, but the actual text that appeared in notepad was *"text"*

## Uninstall
- Remove the EXE you downloaded (KeyCorrect-v\*.\*.\*.exe)
- Download and run this script: **[here](https://github.com/MP3Martin/KeyCorrect/blob/main/scripts/uninstall-interception-driver-run-as-admin.exe)**
- Restart your computer

## Issues
- Please report any issue in [issues](https://github.com/MP3Martin/KeyCorrect/issues) or at https://discord.mp3martin.xyz/
- But remember that stuff like different keyboard layouts are not supported. You can still create a pull request, but don't create an issue because *enter* shows up like and *underscore*

## Pull requests
- Anything is welcome (feature, code cleanup, speed optimisation, fixing a little misspell)
