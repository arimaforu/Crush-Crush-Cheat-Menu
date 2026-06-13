# Crush Crush Cheat Menu

Cheat menu for **Crush Crush** built as a MelonLoader mod.

## Install

1. Install [MelonLoader](https://github.com/LavaGang/MelonLoader/releases) into the Crush Crush game directory.
2. Run the game once to generate the config files, and then close it.
3. Download the latest release from the releases tab and place the `CrushCore.dll` file in the `Mods` folder.
4. Start the game and enjoy the cheats!

## Build

Install:

- [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet)
- [MelonLoader](https://github.com/LavaGang/MelonLoader/releases) if you haven't already

Open CrushCore.csproj and change the GameDir path to your Crush Crush installation:

```xml
<GameDir Condition="'$(GameDir)' == ''">..\YOUR\GAME\DIR\CrushCrush</GameDir>
```

Then open a terminal in the project folder and run:

```bash
dotnet restore
dotnet build
```

The compiled mod DLL will be created here:

```text
bin\Debug\net472\CrushCore.dll
```

## Hotkeys

- `F1` - Show or hide the cheat menu.
- `F6` - Skip the current phone timer.

## Features

- Modern dark cheat menu built with Unity IMGUI.
- Draggable window with saved position.
- Saves selected tabs and settings using MelonPreferences.
- English and Russian language support.
- Global notification system.
- Currency controls:
  - Add a custom amount of diamonds.
- Girls controls:
  - Level up the current girl.
  - Complete the current girl.
  - Set the current girl's hearts to a high value.
  - Unlock all girls.
  - Set all detected girls to the Lover relationship level.
  - Toggle automatic phone timer skipping.
- Jobs and hobbies controls:
  - Unlock and max all detected jobs.
  - Unlock and max all hobbies.
- Stats controls:
  - Change Unity `Time.timeScale`.
  - Change game reset boost.
  - Reset time scale back to `1`.
