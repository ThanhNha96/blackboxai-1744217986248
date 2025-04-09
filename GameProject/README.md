# Dog Adventure Game Demo

A simple platformer game where you control a customizable dog character, collect items, and complete levels.

## Setup Instructions

1. Open Unity Hub
2. Add the GameProject folder as a new project
3. Open the project in Unity Editor
4. In the Project window, navigate to Scenes/MainMenu
5. Double click MainMenu scene to open it

## Required Unity Setup

1. Create the following folders in the Project window if they don't exist:
   - Prefabs
   - Scenes
   - Scripts
   - Materials
   - Sprites
   - Resources (for localization)

2. Setup Localization:
   - Add LocalizationManager to a persistent GameObject in the MainMenu scene
   - Add LocalizedText component to all UI Text elements that need translation
   - Set the appropriate localization key for each LocalizedText component

2. Required Prefabs (Create basic objects and save as prefabs):
   - Player prefab with these components:
     * Sprite Renderer
     * Rigidbody2D (2D physics)
     * Box Collider 2D
     * DogController script
   - Platform prefab:
     * Sprite Renderer
     * Box Collider 2D
   - Collectible prefab:
     * Sprite Renderer
     * Circle Collider 2D (Is Trigger enabled)
     * Collectible script
   - Level End prefab:
     * Sprite Renderer
     * Box Collider 2D (Is Trigger enabled)

3. Required Scenes:
   - MainMenu
   - GameScene
   - CustomizationScene
   - CollectionScene
   - SettingsScene

## Building the Game

1. Set up all scenes in Build Settings:
   - Open File > Build Settings
   - Add all scenes from the Scenes folder
   - Ensure MainMenu scene is first in the list

2. Configure Player Settings:
   - Company Name: YourCompanyName
   - Product Name: Dog Adventure
   - Default Icon: Add your game icon

3. Build the Game:
   - Choose your target platform (PC/Mac/Linux)
   - Click "Build"
   - Select output folder
   - Wait for build completion

## Controls

- Arrow Keys / A,D - Move left/right
- Space - Jump
- Escape - Pause game
- Mouse - Menu navigation

## Language Settings

The game supports multiple languages that can be changed in the Settings menu:
1. Open Settings
2. Find the Language dropdown
3. Select your preferred language
4. All text will update automatically

For Developers:
- Use LocalizedText component on UI Text elements
- Add new translations in LocalizationManager.cs
- Use localization keys for all in-game text
- Test all supported languages

## Features

- Customizable dog character
- Collectible items
- Level progression
- Save/Load system
- Settings configuration
- Dog collection system
- Multilingual support:
  * English
  * Vietnamese (Tiếng Việt)
  * Chinese (中文)
  * Korean (한국어)
  * Japanese (日本語)

## Demo Level

The demo includes one generated level with:
- Platforms to jump between
- Collectible items to gather
- End goal to complete the level
- Score tracking
- Level completion system

## Known Issues

- Some visual effects might be missing
- Sound effects not implemented
- Limited animation states

## Next Steps for Full Game

1. Add more levels
2. Implement sound effects and background music
3. Add more dog customization options
4. Create level selection menu
5. Add more gameplay mechanics
6. Implement achievements system
