-- Create the Flixplorer database
CREATE DATABASE IF NOT EXISTS Flixplorer;

-- Use the Flixplorer database
USE Flixplorer;

-- Create the Users table
CREATE TABLE IF NOT EXISTS Users (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL,
    Password VARCHAR(255) NOT NULL,
    Salt VARCHAR(100) NOT NULL
);

-- Create the Movies table
CREATE TABLE IF NOT EXISTS Movies (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    NetflixID INT NOT NULL,
    ImagePath VARCHAR(5000) NOT NULL,
    HideFromList BOOLEAN NOT NULL DEFAULT FALSE
);

-- Create the Series table
CREATE TABLE IF NOT EXISTS Series (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    NetflixID INT NOT NULL,
    ImagePath VARCHAR(5000) NOT NULL,
    HideFromList BOOLEAN NOT NULL DEFAULT FALSE
);

-- Create the FavouriteMovies table
CREATE TABLE IF NOT EXISTS FavouriteMovies (
    UserID INT NOT NULL,
    MovieID INT NOT NULL,
    PRIMARY KEY (UserID, MovieID),
    FOREIGN KEY (UserID) REFERENCES Users(ID),
    FOREIGN KEY (MovieID) REFERENCES Movies(ID)
);

-- Create the FavouriteSeries table
CREATE TABLE IF NOT EXISTS FavouriteSeries (
    UserID INT NOT NULL,
    SeriesID INT NOT NULL,
    PRIMARY KEY (UserID, SeriesID),
    FOREIGN KEY (UserID) REFERENCES Users(ID),
    FOREIGN KEY (SeriesID) REFERENCES Series(ID)
);
