# OSKLock
The **OSK Mover Tray App** is a lightweight Windows utility designed to control the position of the on-screen keyboard (OSK), locking it in place and preventing users from exiting.
<!-- Optional: Add an image showing the tray icon -->

## Features
- Automatically positions the OSK above the taskbar to prevent overlap.
- Runs on startup, managed via Task Scheduler.
- System tray icon for quick access and application control.
- Customizable position and size of the OSK window.
- Configurable to run as an administrator for elevated access.

## Installation
### Prerequisites
- .NET Framework 4.7.1 or later
- Windows OS with on-screen keyboard functionality

### Steps to Install
1. **Clone the repository**:
```
git clone https://github.com/The-Watering-Can-Dev-Team/OSKLock.git
```
2. **Build the project** using Visual Studio or any compatible IDE that supports .NET Framework 4.7.1.
3. **Run the Installer**: The installer will automatically set up the app and create a scheduled task to run the app on user login.
4. **Verify the Tray Icon**: After installation, the app will start running, and you should see the OSK Mover Tray Icon in your system tray.

## Usage
Once the application is running, it will:
- Automatically adjust the OSK window to stay above the taskbar whenever the OSK is launched.
- Display an icon in the system tray for quick access.

Right-click the tray icon to:
- Exit the application.

The app runs quietly in the background and ensures that the on-screen keyboard behaves as expected without overlapping the taskbar.

## Configuration
If you need to adjust the behavior or position of the OSK, modify the following values in the code:
- **Position**: Change the X and Y coordinates in the SetWindowPos method.
- **Size**: Adjust the width and height of the OSK window as needed.

### Key Components
- `NotifyIcon`: Provides the system tray icon and its functionalities.
- `SetWindowPos`: Manages the OSK window's position and prevents it from covering the taskbar.
- `Task Scheduler`: Automatically launches the app on user login with the necessary privileges.

## Contributing
Contributions are welcome! Feel free to fork this repository, make changes, and submit pull requests. If you find any issues or have feature suggestions, please create an issue on GitHub.

## License
This project is licensed under the MIT License. See the LICENSE file for more details.
