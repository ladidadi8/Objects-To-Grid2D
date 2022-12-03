# Objects-To-Grid2D

Script that generates 2D grid and positions objects based on grid parameters in Unity edit mode.

Since it was made for personal project, some parameters might need to be changed within the script in order to achieve desired results.

Objects you wish to position should be assigned inside inspector, since script only positions existing objects, and doesn't instantiate new objects. This way script can be disabled or removed without affecting object positions, or data that they might have.

Using CTRL-Z to undo actions taken by script might lead to error, but re-adding script or using hard reset option within script can fix issues.

Almost entire script is commented on how it works so making changes should be easy. 

Example of generated positions
![Generated Points](https://user-images.githubusercontent.com/57329828/205448303-1729abf9-9ec8-48c8-8309-51bc87f21444.png)


Example of randomize positions
![Randomize points](https://user-images.githubusercontent.com/57329828/205449354-aeeda452-c4bf-4b41-ae56-3172eb7164e0.png)

