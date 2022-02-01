# Changelog

All notable changes to this repository will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/) and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## Unreleased

### Upgrade Notes

### Known Issues

### Added

### Changed

### Deprecated

### Removed

### Fixed


## [0.5.2-preview] - 2022-02-01

Added Sonarqube scanner

### Fixed
Fixed inability to read relative file paths 

Correct Axis change issues in URDF Importer


## [0.5.1-preview] - 2021-10-04
Fixed bug with multiple references to the same mesh during import

Add the [Close Stale Issues](https://github.com/marketplace/actions/close-stale-issues) action

### Upgrade Notes

### Known Issues

### Added

Start supporting file:// type URI.

### Changed

### Deprecated

### Removed

### Fixed

## [0.5.0-preview] - 2021-07-15

### Upgrade Notes
Update third party notices

Upgrade namespace from RosSharp.Urdf to Unity.Robotics.UrdfImporter

### Known Issues

### Added
Adding badges to main README

### Changed
STL files will not be automatically processed to create .prefab files when copied into the Assets directory or when assets are reimported. Instead the processing happens during the URDF import and required .prefab files will be created if they don't exist already or if the "Overwrite Existing Prefabs" option is checked in the URDF Import settings dialog.

Renamed "URDF" to "Urdf" in class names, function names and source filenames.

Renamed RuntimeURDFImporter to RuntimeUrdfImporterExample for clarification

### Deprecated

### Removed

### Fixed
Bug where-in URDF Importer would throw an error when installed via Package Manager because it can't save prefabs to its own directories

Compile error "Plugin 'assimp.dll' is used from several locations" when creating a Universal Windows Platform build (#122) 

Fix no material issue in Export robot to URDF (#127)

Fix the inconsistent casing of meta files (#128)

## [0.4.0-preview] - 2021-05-27

Note: The logs below only presents the changes from 0.3.0-preview

### Upgrade Notes
Refactor the codebase and support for runtime URDF importing

### Known Issues

### Added
Save the generated cylinder meshes to a new folder, `Assets/URDF/GeneratedMeshes`, as primitive cylinders will have no associated .stl filename/path

Add a link to the Robotics forum, and add a config.yml to add a link in the Github Issues page

Add unit tests and test coverage reporting

### Changed

### Deprecated

### Removed

### Fixed
Correct collider rotation so that the collider meshes matches visual meshes for both Y-axis and Z-axis

Replace "../" with the expected "package://" and throw more warnings if things still don't match up
