# UniGame.ViewSystem

MVVM View System for Unity3D

## Overview

- support base principles of MVVM concepts.
- support ui skins out of the box
- based on Unity Addressables Resources
- handle Addressables Resource lifetime


## UI System Settings

### Create Settings


![](https://i.gyazo.com/15833fe0019b9570d68cab6ba20d3df6.png)

### Settings Rebuild

Ui System supports auto settings rebuild . AssetPostprocessor will be triggered if any asset changed at project directory registered in Ui System Settings 

You can manualy trigger rebuild:

- For All Settings

![](https://i.gyazo.com/df803c28a8a9feb702cda99734cb9288.png)

- For target settings asset from inspector context menu

![](https://i.gyazo.com/7df8670d31e77df4c8f69bc2e7da9d92.png)



## Examples

### Item List View

![](https://github.com/UniGameTeam/UniGame.UISystem/blob/master/Readme/Assets/ui_list_demo.gif)
