using System;
using System.Collections.Generic;
using System.IO;

namespace WoWNameChanger {
    class Program {
        private static string wowDefaultPath = @"C:\Program Files\World of Warcraft\";
        private static string wowDefaultPathX86 = @"C:\Program Files (x86)\World of Warcraft\";
        private static string wtfFolder = @"WTF\";
        private static string accountFolder = @"Account\";
        private static string savedVariablesFolder = @"SavedVariables\";

        private static bool YesNoQuestion(string question, bool defaultAnswer) {
            string y = defaultAnswer ? "Y" : "y";
            string n = !defaultAnswer ? "N" : "n";
            string fullQuestion = question + " [" + y + "|" + n + "] ";

            ConsoleKey pressedKey;
            do {
                Console.Write(fullQuestion);
                pressedKey = Console.ReadKey(true).Key;
                Console.WriteLine();
            } while (pressedKey != ConsoleKey.Enter && pressedKey != ConsoleKey.Y && pressedKey != ConsoleKey.N);

            bool answer;
            if (pressedKey == ConsoleKey.Enter) {
                answer = defaultAnswer;
            } else {
                answer = pressedKey == ConsoleKey.Y ? true : false;
            }
            
            return answer;
        }

        private static string FindWoWFolder() {
            string wowPath = null;
            bool found = false;
            while (!found) {
                Console.Write("Specify your WoW folder path: ");
                wowPath = Console.ReadLine();
                if (!ExistWTF(wowPath)) {
                    Console.WriteLine("Invalid WoW folder, WTF not found. Please try again.");
                } else {
                    found = true;
                }
            }

            return wowPath;
        }

        private static bool ExistWTF(string wowPath) {
            return Directory.Exists(wowPath) && Directory.Exists(Path.Combine(wowPath, wtfFolder));
        }

        private static string GetWoWPath() {
            string wowPathFound = null;
            if (ExistWTF(wowDefaultPath)) {
                wowPathFound = wowDefaultPath;
            } else if (ExistWTF(wowDefaultPathX86)) {
                wowPathFound = wowDefaultPathX86;
            }

            string wowPath = null;
            if (wowPathFound == null) {
                Console.WriteLine("No WoW folder found.");
                wowPath = FindWoWFolder();
            } else {
                bool useFoundFolder = YesNoQuestion("WoW folder " + wowPathFound + " found. Use this folder?", true);
                if (useFoundFolder) {
                    wowPath = wowPathFound;
                } else {
                    Console.WriteLine("Specify another folder.");
                    wowPath = FindWoWFolder();
                }
            }

            Console.WriteLine("WoW folder found: " + wowPath);
            return wowPath;
        }

        private static string GetAccountPath(string wtfPath) {
            string account = null;
            bool found = false;
            while (!found) {
                Console.Write("Insert your account name: ");
                account = Console.ReadLine();
                if (account != "" && Directory.Exists(Path.Combine(wtfPath, accountFolder, account))){
                    found = true;
                } else {
                    Console.WriteLine("Account folder not found. Please try again.");
                }
            }

            string accountPath = Path.Combine(wtfPath, accountFolder, account);
            Console.WriteLine("Account folder found: " + accountPath);
            return accountPath;
        }

        private static string GetRealmPath(string accountPath) {
            string realm = null;
            bool found = false;
            while (!found) {
                Console.Write("Insert your realm name: ");
                realm = Console.ReadLine();
                if (realm != "" && Directory.Exists(Path.Combine(accountPath, realm))){
                    found = true;
                } else {
                    Console.WriteLine("Realm folder not found. Please try again.");
                }
            }

            string realmPath = Path.Combine(accountPath, realm);
            Console.WriteLine("Realm folder found: " + realmPath);
            return realmPath;
        }

        private static string GetOldCharacterName(string realmPath) {
            string oldCharacter = null;
            bool found = false;
            while (!found) {
                Console.Write("Insert your OLD character name: ");
                oldCharacter = Console.ReadLine();
                if (oldCharacter != "" && Directory.Exists(Path.Combine(realmPath, oldCharacter))){
                    found = true;
                } else {
                    Console.WriteLine("Character folder not found. Please try again.");
                }
            }

            string oldCharacterPath = Path.Combine(realmPath, oldCharacter);
            Console.WriteLine("OLD character folder found: " + oldCharacterPath);
            return oldCharacter;
        }

        private static void ChangeNameInFile(string oldCharacter, string newCharacter, string filePath) {
            string fileContent = File.ReadAllText(filePath);
            fileContent = fileContent.Replace(oldCharacter, newCharacter);
            File.WriteAllText(filePath, fileContent);
        }

        private static void ChangeNameForFilesInFolder(string oldCharacter, string newCharacter, string folderPath) {
            string[] files = Directory.GetFiles(folderPath);
            foreach(string file in files) {
                ChangeNameInFile(oldCharacter, newCharacter, file);
            }
        }

        private static void CopyDirectory(string sourcePath, string destinationPath) {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories)) {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories)) {
                string newNewPath = newPath.Replace(sourcePath, destinationPath);
                File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
            }
        }

        private static void PrintAllMatchingLinesInFolder(string oldCharacter, string folder, string outputFile, StreamWriter sw, List<string> uniqueLines) {
            string[] files = Directory.GetFiles(folder);
            foreach (string file in files) {
                string[] lines = File.ReadAllLines(file);
                foreach (string line in lines) {
                    if (line.Contains(oldCharacter)) {
                        if (!uniqueLines.Contains(line)) {
                            uniqueLines.Add(line);
                            sw.WriteLine(line);
                        }
                    }
                }
            }
        }

        private static void PrintAllMatchingLines(string oldCharacter, string accountSVPath, string oldCharacterSVPath, string outputFile) {
            List<string> uniqueLines = new List<string>();
            using (StreamWriter sw = new StreamWriter(File.Open(outputFile, System.IO.FileMode.Create))) {
                PrintAllMatchingLinesInFolder(oldCharacter, accountSVPath, outputFile, sw, uniqueLines);
                PrintAllMatchingLinesInFolder(oldCharacter, oldCharacterSVPath, outputFile, sw, uniqueLines);
            }
        }

        static void Main(string[] args) {
            string wowPath = GetWoWPath();
            string wtfPath = Path.Combine(wowPath, wtfFolder);
            string accountPath = GetAccountPath(wtfPath);
            string accountSVPath = Path.Combine(accountPath, savedVariablesFolder);
            string realmPath = GetRealmPath(accountPath);
            string oldCharacter = GetOldCharacterName(realmPath);
            string oldCharacterPath = Path.Combine(realmPath, oldCharacter);
            string oldCharacterSVPath = Path.Combine(oldCharacterPath, savedVariablesFolder);

            string newCharacter = "";
            while (newCharacter.Equals("")) {
                Console.Write("Insert your NEW character name: ");
                newCharacter = Console.ReadLine();
            }
            string newCharacterPath = Path.Combine(realmPath, newCharacter);

            if (YesNoQuestion("Do you want to backup the WTF folder before proceding?", true)) {
                string backupFolderDefaultName = "WTF-WoWNameChanger-backup";
                string backupFolderPath = null;
                int i = 0;
                do {
                    string backupFolder = i == 0 ? backupFolderDefaultName + @"\" : backupFolderDefaultName + i + @"\";
                    backupFolderPath = Path.Combine(wowPath, backupFolder);
                    i++;
                } while (Directory.Exists(backupFolderPath));
                CopyDirectory(wtfPath, backupFolderPath);
            }

            if (YesNoQuestion("Do you want to list all the lines that will be changed?", true)) {
                string outputFile = Path.Combine(wowPath, "WoWNameChanger-" + oldCharacter + "-UniqueLines.txt");
                PrintAllMatchingLines(oldCharacter, accountSVPath, oldCharacterSVPath, outputFile);
                Console.WriteLine("Unique lines that will be changed listed in file " + outputFile);
            }

            if (YesNoQuestion("Do you want to proceed with the name change?", true)) {
                ChangeNameForFilesInFolder(oldCharacter, newCharacter, accountSVPath);
                ChangeNameForFilesInFolder(oldCharacter, newCharacter, oldCharacterSVPath);
                Directory.Move(oldCharacterPath, newCharacterPath);
                Console.WriteLine("Name change completed!");
            }

            // Exit the terminal
            Console.Write("\nPress any key to exit...");
            Console.ReadKey(true);
        }
    }
}
