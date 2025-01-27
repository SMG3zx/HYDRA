# HYDRA - Handler for Your Data Removal Administration

## Overview

HYDRA is a PowerShell utility for managing and cleaning up old user profiles across a Windows domain. It provides an interactive interface to scan computers, identify inactive profiles based on configurable criteria, and safely remove them to free up disk space.

## Features

- Parallel processing of multiple computers for efficient cleanup
- Configurable age threshold for profile deletion
- Skip list to protect critical profiles
- Detailed logging of all operations
- Interactive settings management

## Usage

### Main Menu

When launched, HYDRA presents the following menu:

```
HYDRA Version : 2.0
Author : SMG3zx
Last Updated : 1/27/25
=============================================
HYDRA Main Menu
1. Clean Profiles
2. Settings
3. Exit
Please select an option :
```
### Clean Profiles

This option allows you to initiate the profile cleanup process. HYDRA will:

- Scan the specified list of computers
- Identify and delete profiles older than the configured age threshold
- Log all actions taken during the cleanup process

### Settings

This option allows you to configure HYDRA's settings. You can:

- Change the client list path
- Change the log location
- Change the skip list
- Change the age threshold
- Change the throttle limit

### Exit

This option allows you to safely exit the HYDRA utility.
