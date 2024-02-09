# Off time

## About

Off time is a simple Unity puzzle and quiz game I developed as a thesis project for my 2022 software developer and tester certification. 
This is my first "real" project, so it's very simple and kinda rough around the edges. The code is in English, but the in game user interfaces are all Hungarian. 
Translating it would be a waste of effort, as I doubt anyone is interested in playing this game.

## Features

- Multiple users
- Game states can be saved and loaded
- Leaderboard

## How to set up

You can manually select the project folder in unity after cloning it, it is not recommended to update the project version.


## How to play

I provided an installer on the releases tab

**Don't run random executables from the internet**

Create a windows virtual machine, and run the installer there, or start the game from the unity editor at the main menu scene. (After reviewing the code) 
If you take the installer route, the game might ask you for firewall access, deny it. (Unity analytics needs it)

The game will ask you to create a user profile. (This was a project requirement) 

Your goal is to route power through various nodes and logic gates, and power up the end node.

Most nodes can be rotated by getting close enough to them and clicking on them.
Some nodes have have blockers attached to them, that prevent them from working.
You can unlock them by powering said blockers, and clicking on them. (These have a maximum range as well) After unlocking, the blockers don't need to be powered.