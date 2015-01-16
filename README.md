PoeHud
======

Reads data from Path of Exile client application and displays it on transparent overlay, while you play PoE.
No longer contains maphack/particleshack/zoomhack/fullbright.
it now only relies on reading memory. Thinking about implementing another way than RPM to do this.

Added a PE header scrambler (really basic at this point in time but hopefully I'll be able to dev it further to make this safer to use)


### Requirements
* .NET framerwork v.4.5 or newer (you already have it on Windows 8+)
* Windows Vista or newer (XP won't work)
* Path of Exile should be running in Windowed or Windowed Fullscreen mode (the pure Fullscreen mode does not let PoeHUD draw anything over the game window)
* Windows Aero transparency effects must be enabled. (If you get a black screen this is the issue)

### Item alert settings
The file config/crafting_bases.txt has the following syntax:
`Name,[Level],[Quality],[Rarity1,[Rarity2,[Rarity3]]]`

Examples of valid declarations:
```
Vaal Regalia,78
Corsair Sword,78,10
Gold Ring,75,,White,Rare
Ironscale Gauntlets,,10,White,Magic
Quicksilver Flask,1,5
Portal Scroll
Iron Ring
```
Also the mods used for mobs and items are listed in Content.ggpk\Data\Mods.dat.
