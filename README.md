# WoW Name Changer

C# script to automatically migrate addons settings after a character's name change in World of Warcraft.

## Basic usage

The script will ask the user for the account name, realm name and the old and new character's names in order to locate the right folders.

The script should be ran right BEFORE performing the name change inside WoW.

The script also gives the possibility to the user to create a backup of the WTF folder before the name change process (**highly recommended!**) and also to generate a file containing all the lines across all the addons that will be changed by the process (**also highly recommended!**).

## Disclaimer

The addon is not 100% accurate and **MIGHT BREAK THINGS UP**, for example if the old character's name is a substring of another character's name or the account or realm name (or any other string that might be in the addons settings in general).

For this reason, the use of the backup and the inspection of the lines that are about to be changed is **highly recommended**.
