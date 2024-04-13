# KeyCorrect
Intercept keyboard with correct characters<br><br>
![image](https://github.com/MP3Martin/KeyCorrect/assets/60501493/c56b4e5c-1a1a-4bf0-83c3-a55cf0ae70f6)


## Info
- My first .NET project
- Made for personal usage (because handling different keyboard layouts / special characters would be too complex for this "simple project")
- **Only supports the most simple characters on most keyboard layouts** (maybe try switching to english keyboard layout if it is typing weird characters)
- **Czech QWERTZ keyboard layout** has special support for some characters like "ů", "ž", "-", ...
- Only for Windows x64
- Build on top of [this example](https://github.com/0x2E757/InputInterceptor#example-application) thanks to [@oblitum](https://github.com/oblitum) and [@0x2E757](https://github.com/0x2E757)
- **⚠️ Use at own risk ⚠️**, you choose to use this software
- The driver this software uses might **prevent games** like Forza Horizon 5 **from launching**! (because of an anti-cheat that detects the keyboard driver)
*(This software is not affiliated with or endorsed by the developers or publishers of Forza games.)*

## First use
1. Download the latest installer from [releases](https://github.com/MP3Martin/KeyCorrect/releases/latest) or build it from source using .NET 6
2. Double click the installer EXE
3. Select "Install"
4. Press "Yes" when it asks for admin permission
5. Read the program output. If it says *"Done! Restart your computer."*, then do just that **(important)**
6. After restarting, you can just run the program (from desktop or start menu) and use it (program name is "KeyCorrect")

## Example usage
- You copy *"text"* to your clipboard
- You launch this program
- You open notepad
- You press the PageUp key
- You type *"abcd"* on your keyboard, but the actual text that appeared in notepad was *"text"*
- You press the PageUp key again to toggle the interception back to off

## No pageup key?
- How to press PageUp on a laptop (if you don't have a standalone PageUp key): Turn off NumLock and press Numpad9

## Uninstall
- Press `WIN` + `R`
- Type `appwiz.cpl`
- Find "KeyCorrect" and double click it
- When a window opens, select "Remove" and then "OK"
- Download and run this script to remove the keyboard driver: **[here](https://github.com/MP3Martin/KeyCorrect/blob/main/scripts/uninstall-interception-driver-run-as-admin.exe)**
- Restart your computer

## Issues
- Please report any issue in [issues](https://github.com/MP3Martin/KeyCorrect/issues) or at https://discord.mp3martin.xyz/
- But remember that stuff like different keyboard layouts are not supported. You can still create a pull request, but don't create an issue because *enter* shows up like an *underscore*

## Pull requests
- Anything is welcome (feature, code cleanup, speed optimisation, fixing a little misspell)
