# Unfair Flips AutoFlip

A mod for [Unfair Flips](https://store.steampowered.com/app/3925760/) that flips the coin and buys upgrades for you.

It runs **inside the game**, not on your screen. It never moves your mouse, so you can tab out, watch a video or play something else while it flips.

- Flips the coin every time it's ready
- Buys upgrades as soon as you can afford them, in this order:
  1. Coin Value
  2. Heads Chance
  3. Combo
  4. Flip Time
- Keeps going while the game is in the background
- Stops on its own when you hit the ending

## Install (Windows)

1. **Download** `UnfairFlipsAutoFlip-v1.0.0.zip` from the [Releases page](../../releases/latest). (It's under **Assets**, near the bottom of the release.)
2. **Open the game folder.** In Steam, right-click **Unfair Flips** in your Library -> **Manage** -> **Browse local files**. A folder opens with `Unfair Flips.exe` in it.
3. **Unzip into that folder.** Open the zip you downloaded, select everything inside it (`BepInEx`, `winhttp.dll`, `doorstop_config.ini`) and drag it into the game folder. If Windows asks, choose **Replace**.

   When you're done, the game folder should look like this:

   ```
   Unfair Flips/
     BepInEx/
     MonoBleedingEdge/
     Unfair Flips_Data/
     doorstop_config.ini
     Unfair Flips.exe
     UnityCrashHandler64.exe
     UnityPlayer.dll
     winhttp.dll
   ```

   `winhttp.dll` must sit **next to** `Unfair Flips.exe`, not inside another folder. If you see a folder called `UnfairFlipsAutoFlip-v1.0.0` in there, open it and move its contents up one level.

4. **Play the game from Steam** like normal. The coin starts flipping by itself once you're in.

That's it. There's nothing else to run.

## Controls

Click the game window first, then:

| Key | What it does |
|---|---|
| **F8** | Turn auto flip on or off |
| **F9** | Turn auto buy on or off |

Both start **on**.

## Settings

After you've started the game once with the mod, a settings file appears at:

```
Unfair Flips/BepInEx/config/tyfur.unfairflips.autoflip.cfg
```

Open it with Notepad, change a value, save, and restart the game.

| Setting | Default | What it does |
|---|---|---|
| `AutoFlip` | `true` | Flip automatically |
| `AutoBuy` | `true` | Buy upgrades automatically |
| `Priority` | `FlipBaseWorth,HeadsChance,FlipMultiplier,FlipTime` | Buy order. `FlipBaseWorth` = Coin Value, `FlipMultiplier` = Combo |
| `SaveForTop` | `false` | `false`: buy the highest-priority upgrade you can afford right now. `true`: save up for the first upgrade in the list that isn't maxed, and buy nothing else until you have it |
| `ToggleFlip` / `ToggleBuy` | `F8` / `F9` | Hotkeys |

## Uninstall

Delete these from the game folder:

- `BepInEx` (folder)
- `winhttp.dll`
- `doorstop_config.ini`

Your save isn't touched either way.

## Problems?

**Nothing happens when I start the game.**
`winhttp.dll` is almost always in the wrong place. It has to be in the same folder as `Unfair Flips.exe`. After launching once, there should be a file at `BepInEx/LogOutput.log`. If that file isn't there, the mod didn't load.

**Windows Defender or my antivirus flagged `winhttp.dll`.**
That file is [BepInEx](https://github.com/BepInEx/BepInEx), the most widely used mod loader for Unity games. It loads mods by sitting next to the game's exe, which some antivirus programs find suspicious. It's a false positive. The full source code for BepInEx and for this mod is public.

**The keys don't work.**
The hotkeys only work while the game window is focused. Click on the game, then press F8 or F9.

**Steam Deck / Linux (Proton).**
Install the same way, then in Steam right-click the game -> **Properties** -> **Launch Options** and paste:

```
WINEDLLOVERRIDES="winhttp=n,b" %command%
```

**Mac** isn't supported.

## How it works

The mod uses [BepInEx 5](https://github.com/BepInEx/BepInEx) to load inside the game. Every frame, it checks the same conditions the game uses for the Space key. If the coin is ready, it calls the game's own flip code. Once the shop is open, it buys the next upgrade on your list by pressing that shop button through the game's own code. No memory editing, no fake mouse clicks.

## Building from source

You need the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) and the game with BepInEx installed (the build reads the game's DLLs).

```
dotnet build -c Release
```

If the game isn't in the default Steam folder:

```
dotnet build -c Release -p:GamePath="D:\SteamLibrary\steamapps\common\Unfair Flips"
```

The plugin ends up in `bin/Release/netstandard2.1/UnfairFlipsAutoFlip.dll`. Copy it to `Unfair Flips/BepInEx/plugins/`.

## Credits

- Unfair Flips is made by Heather Flowers. This mod isn't affiliated with or endorsed by the developer. Buy the game on Steam to support them.
- The release zip includes [BepInEx](https://github.com/BepInEx/BepInEx), licensed under LGPL-2.1 (see `BepInEx/LICENSE-BepInEx.txt`).
