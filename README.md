# Flixplorer

**Flixplorer** is a WPF application that allows users to discover and explore Netflix content. The app provides features such as browsing the latest TV shows and movies, searching for specific titles, and managing favorite content through user accounts. With just a click, users can view more detailed information about any title and watch their selected content directly on Netflix.

## Features
- **User Accounts**: Create and manage accounts to keep track of favorite Netflix titles.
- **Content Discovery**: Fetch and display the latest TV shows and movies from Netflix using a third-party API.
- **Favorites Management**: Add titles to your personal favorites list for easy access later.
- **Search Functionality**: Search for specific titles via the integrated search field.
- **Watch on Netflix**: Access the Netflix site directly through the app by clicking on the 'Watch on Netflix' button.

## Installation

**Clone the repository:** `git clone https://github.com/lauri-sa/Flixplorer.git`

**Open the solution:** Use Visual Studio to open the `.sln` file in the project directory.

### Database Setup

The repository contains the necessary SQL script to create the required database. Before running the application, you'll need to:

- Install MySQL if it is not already installed.

- Create the database using the provided SQL script located in the `SqlScripts` folder. The database must be set up using MySQL as the application relies on MySQL for data storage.

- Add an `App.config` file to the project and configure the connection string to point to your MySQL database.

Copy this into your `App.config` file:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <configSections>
        <sectionGroup name="userSettings" type="System.Configuration.UserSettingsGroup, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" >
            <section name="MovieDatabase.Properties.Settings" type="System.Configuration.ClientSettingsSection, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" allowExeDefinition="MachineToLocalUser" requirePermission="false" />
        </sectionGroup>
    </configSections>

    <appSettings>
        <add key="ApiKey1" value="AddYourApiKeyForVersion1"/>
        <add key="ApiKey2" value="AddYourApiKeyForVersion2"/>
    </appSettings>

    <connectionStrings>
        <add name="ConnectionString" connectionString="Server=YourServerAddress;Port=YourPort;Database=flixplorer;UserID=YourUsername;Pwd=YourPassword;" />
    </connectionStrings>
</configuration>

```
Replace the placeholders in the `ConnectionStrings` section with your actual database information but keep the name as `ConnectionString`.

### API Configuration

The application relies on the uNoGS API to retrieve the latest Netflix content. You will need to:

1. **Get API keys:**

    - Create an account on [RapidAPI](https://rapidapi.com/) if you don't already have one.
    
    - Subscribe to the [uNoGS API](https://rapidapi.com/unogs/api/unogs/).
  
    - Generate two API keys: one for Version 1 and one for Version 2 of the API.

3. **Add the API keys** to the previously created `App.config` file under the `appSettings` section as shown below:

```xml
    <appSettings>
        <!-- Add your API keys here -->
        <add key="ApiKey1" value="AddYourApiKeyForVersion1"/>
        <add key="ApiKey2" value="AddYourApiKeyForVersion2"/>
    </appSettings>
```

Replace `AddYourApiKeyForVersion1` and `AddYourApiKeyForVersion2` placeholders with your actual API keys from RapidAPI.

## Running the Application

**Build the project:** Build the project using Visual Studio.

**Run the application:** Start the application directly from Visual Studio.

## Screenshots

![image](https://github.com/user-attachments/assets/85442453-2723-4a1d-8233-84d6172912c7)

![image](https://github.com/user-attachments/assets/2742e297-4da6-495e-9e06-1300b9b2ac4b)

![image](https://github.com/user-attachments/assets/579f3552-cb62-48e8-a21e-004806b6c764)

![image](https://github.com/user-attachments/assets/a7b87664-be88-4f31-aeee-01251eb2bebc)
