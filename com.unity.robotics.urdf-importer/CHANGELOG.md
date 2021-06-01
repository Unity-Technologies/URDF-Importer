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