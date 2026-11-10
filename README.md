# Sandrone Cat v3.2

A small, transparent Windows desktop companion with an oversized head, an eight-frame walking cycle, a six-frame wake-up stretch, and a click-to-hiss reaction.

## Run

Exit any older instance, then double-click `SandroneCat.exe`. Keep `sprites.png` and `stretch.png` beside the executable. No installation or administrator access is required.

Requires Windows with .NET Framework 4.x and WPF. The included executable was built with the Windows .NET Framework compiler.

## Behavior

- While you use the keyboard, mouse, or switch foreground windows, the cat sits and watches you.
- After 30 seconds without activity, it walks along the bottom of the primary screen's work area, above the taskbar.
- After walking for 30 seconds, it returns to the nearest bottom corner and sleeps.
- On waking, it stretches for 1.8 seconds before sitting down.
- Clicking a sleeping cat plays the stretch first, followed by a two-second hiss. There is no sound.
- Walking animation follows distance traveled, with a consistent scale and aligned paws.

## Controls

- Click: hiss.
- Hold the left mouse button and drag: move the cat.
- Right-click: sleep, hiss, choose a size, return to the bottom-right corner, or exit.
- Sizes: Mini (120), Default (160), and Large (200).

The app queries the system's last input timestamp and foreground window handle. It does not read typed text or page content. It does not add itself to startup.

## Build from source

Run the included PowerShell script from this folder:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\build.ps1
```

The script compiles `Cat.cs`, `Behavior.cs`, and `WalkCycle.cs` into `SandroneCat.exe` using the .NET Framework compiler included with Windows. Exit the running app before rebuilding.

## Files

- `Cat.cs`: transparent window, input activity, animations, and context menu.
- `Behavior.cs`: sitting, walking, returning, and sleeping states.
- `WalkCycle.cs`: movement-based walk animation timing.
- `sprites.png`: sleeping, hissing, sitting, blinking, and eight walking poses.
- `stretch.png`: six wake-up stretch frames.
- `walking-preview.gif`: preview of the walking poses.
- `PROMPTS.md`: English notes about the generated artwork.

## Artwork

Unofficial fan-made Sandrone cat companion inspired by Genshin Impact. Character art was generated and refined with the built-in imagegen tool using the user's reference. The dark garment is a skirt on the cat's torso and back; the white ornament sits in front of the ears. This project is not affiliated with HoYoverse.
