# GlobalGamJam2023
My Global gam jam entry for 2023

## Engine
The game engine will be a custom game engine fully implemented during the 48 hour time window. 
I will use Silk.net as it auto creates bindings to openGL saving me time. 
I will also use OpenGL over the newer vulkan only because I know OpenGL better and can implement it faster.
### Engine Design
The engine will be based around a Entity Component System (ECS) model.
This will allow for a fully data driven approach.
### Entities
Entities are nothing but Ids. They must not directly contain any data or logic. 
Should only be used to reference components
### Components
Components are pure data, should not contain any logic. 
In order to optimize data layout and cpu caching they must be Blittable. 
To help drive this I will implement them as `record structs` and enforce the `unmanged` constraint on generics.
### Systems
Systems are where all the logic will be. in a system you can query entities and their components to run some logic.
Systems will run in asyc to each other and allow running logic in async batches against if needed.
They should also be able to be assigned a game phase in case they depend on another system output to be applied before starting.
By attaching a system to a phase instead of directly linking system as a dependency of another system it allows the two systems to be full decompiled
and allow for a simpler logic for when a system should run.
### Input
Silk.net has some basic cross platform input support and I will use that to save some time.
### Audio
Silk.net has bindings to cross platform audio that I will use to save time. 
### Art
I will implement importing image files via the image sharp library. 
### Assets
All Assets will be embedded resources and should be streamed in async.
### Physics
I wont need to implement anything beside basic collisions so I will write physics 100% from scratch.

## References & Useful links
Silk.net: https://github.com/dotnet/Silk.NET