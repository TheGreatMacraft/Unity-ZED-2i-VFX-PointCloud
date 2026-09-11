# Unity ZED 2i VFX PointCloud
Simple project to show the very basic implementation of PointCloud with Unity's VFX using the ZED camera SDK.

<img width="707" height="474" alt="image" src="https://github.com/user-attachments/assets/9575c2ed-8e0a-4eda-81b5-385999867fd7" />

# How it works?

Step 1: By changing the ZEDPointCloudManager script, to expose private properties:
- Texture2D XYZTexture
- Texture2D colorTexture
we now get access to the color and position of every individual point.

Step 2: We now need to find a way to correctly paint and position each individual particle in the VFX graph. By using blocks that set Position and Color from an Attribute Map with a **Random Constant per Particle**, we can do just that. Since the seed is the same for both blocks, the Random Constant will always correctly target the very same particle in color and position map alike:

<img width="588" height="340" alt="image" src="https://github.com/user-attachments/assets/60a09650-c066-421d-8180-11f87a984366" />

Step 3: We now just need to feed both textures into the exposed property field (via a script) and set the particle rate (particles per second) and particle size. This step is very straightforward.

For additional questions or findings you can always reach out to me on discord: macraft
