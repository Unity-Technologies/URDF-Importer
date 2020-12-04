# URDF Importer 

URDF Importer allows you to import a robot defined in [URDF](http://wiki.ros.org/urdf/XML) format in a Unity scene. URDF defines the geometry, visual meshes, kinematic and dynamic attributes of a Robot. Importer parses a URDF file and imports it into Unity using PhyX 4.0 articulation bodies.

# Using the Package

## Adding the URDF package

1. Create a new Project in your Unity Hin and open the Unity Editor.

2. Open the Package Manager from Unity Menu. Click `Window -> Package Manager`. A new package manager window should appear.

3. Click on the `+` sign on the top left corner of the package manager window and click on `Add Package from Git URL`. A text box should appear below the `+` sign.

4. Paste the link to the URDF github repository in the text box and press `Enter`. This should add the URDF Importer package in your project.

5. Click `Import URDF`.

## Importing the robot using URDF file

1. Copy the URDF and the associated files in the assets folder in the Project window. Make sure the [location](https://github.com/Unity-Technologies/Unity-Robotics-Hub/blob/main/tutorials/urdf_importer/urdf_appendix.md#file-hierarchy) of the mesh files is correct.

2 Right Click on the URDF file and click `Import Robot from URDF`.

3. A window will appear with the Import settings for the Robot. First setting mentions the orietation of the mesh files. The second setting is used to select the algorithm to be used in Collisson mesh Decomposition. For more informaiton click [here](https://github.com/Unity-Technologies/Unity-Robotics-Hub/blob/main/tutorials/urdf_importer/urdf_appendix.md#convex-mesh-collider)


# Tutorials

Instructions for using URDF importer can be found [here](https://github.com/Unity-Technologies/Unity-Robotics-Hub/blob/master/tutorials/urdf_importer/urdf_tutorial.md).
