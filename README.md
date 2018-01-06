# Spaceworks #

Spaceworks is a work-in-progress collection of C# classes for the Unity3D Game Engine which allows the programmer to create Planetary Game Terrain.

![Close-up Image](Screenshots/planet.png)
![Far-away Image](Screenshots/planet2.png)

## Project Structure ##
Materials
	- Preconfigured test materials
Primitives
	- C# classes that are ustilized by each of the systems within Spaceworks
Scenes
	- Test scenes for each system
Shaders
	- Premade shaders
Systems
	- C# scripts relating to various parts of Spaceworks

## Systems ##
The core system to Spaceworks is the Planet Render which creates, manages, and renderes planetary terrain. This is usually done through the Planet Service script. There are many auxilary systems which are used alongside the planet renderer. These include a modular building system, a random planet name generator, a floating origin system for large sized scene management, and some basic camera scripts.
