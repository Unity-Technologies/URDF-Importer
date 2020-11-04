# URDF Importer 

URDF Importer allows you to import a robot defined in [URDF](http://wiki.ros.org/urdf/XML) format in a Unity scene. URDF defines the geometry, visual meshes, kinematic and dynamic attributes of a Robot. Importer parses a URDF file and imports it into Unity using PhyX 4.0 articulation bodies.

# Integrate URDF Importer into Unity Project

- Clone or download the URDF Importer Repo
- In your Unity project create a directory named `Plugins` in `Assets`
- Copy the `URDFLibrary` and `UnityEditorScripts` directories into the newly created `Assets/Plugins` directory
- Confirm the integration succeded by selecting `Assets` from the menu bar and look for the `Import Robot from URDF` option

# Importing a URDF into a Unity scene

- Find the `urdf` file of the robot you want to import in the `Assets` folder and select it
- From the menu click `Assets` -> `Import Robot from URDF` or in file explorer right click on the selected file and click `Import Robot from URDF`

# Tutorials

Instructions for using URDF importer can be found [here](https://github.com/Unity-Technologies/Unity-Robotics-Hub/blob/master/tutorials/urdf_importer/urdf_tutorial.md).
