using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Exam_CMD_Karvatyuk
{
    class CMD
    {
        public DirectoryInfo DirInfo { get; private set; }
        public DriveInfo DrInfo { get; private set; }
        public List<string> HistoryOfCommands { get; private set; }
        public string[] Commands { get; set; }

        private void MkDir(string dirName) // Создание директорий.
        {
            if (dirName == "")
            { throw new MyException(" The syntax of the command is incorrect."); }
            DirInfo.CreateSubdirectory(dirName);
        }

        private void RmDir(string dirName, string atribute) // Yдаление директорий.
        {
            bool rec = false;
            if (atribute == "/s") { rec = true; }

            DirectoryInfo delDirInfo = new DirectoryInfo(dirName);
            if (delDirInfo.Exists)
            {
                delDirInfo.Delete(rec);
            }
        }

        private void Del(string fileName, string atribute) // Yдаление файлов и директорий.
        {
            if (fileName == "")
            { throw new MyException(" The syntax of the command is incorrect."); }

            FileInfo delFileInfo = new FileInfo(fileName);
            if (delFileInfo.Exists)
            {
                delFileInfo.Delete();
            }

            bool rec = false;
            DirectoryInfo delDirInfo = new DirectoryInfo(fileName);
            if (delDirInfo.Exists)
            {
                if (atribute == "/s") { rec = true; }
                delDirInfo.Delete(rec);
            }
        }

        private void Copy(string fileName, string copyTo) // Копирование файлов и папок.
        {
            if (fileName == "" || copyTo == "")
            { throw new MyException(" The syntax of the command is incorrect."); }
            if (File.Exists(fileName))
            {
                File.Copy(fileName, copyTo);
            }

            if (Directory.Exists(fileName))
            {
                DirectoryInfo src = new DirectoryInfo(fileName);
                DirectoryInfo dest = new DirectoryInfo(copyTo);
                CopyAll(src, dest);
            }
        }

        private void CopyAll(DirectoryInfo source, DirectoryInfo destination) // Копирование подпапок.
        {
            Directory.CreateDirectory(destination.FullName);
            foreach (FileInfo fIn in source.GetFiles())
            {
                fIn.CopyTo(Path.Combine(destination.FullName, fIn.Name), true);
            }
            foreach (DirectoryInfo dIn in source.GetDirectories())
            {
                DirectoryInfo dirDest = destination.CreateSubdirectory(dIn.Name);
                CopyAll(dIn, dirDest);
            }
        }

        private void CD(string path) // изменяет текущий каталог
        {
            DirectoryInfo tmp = new DirectoryInfo(path == "" ? "." : path);
            if (tmp.Exists)
            {
                DirInfo = tmp;
                Directory.SetCurrentDirectory(DirInfo.FullName);
            }
            else
            {
                throw new MyException(" The system cannot find the path specified.");
            }

        }

        private void Find(string text) // Поиск файлов и папок.
        {
            foreach (var file in DirInfo.GetDirectories("*" + text + "*.*", SearchOption.AllDirectories))
            {
                Console.WriteLine($"<DIR>  {file.FullName}");
            }
            foreach (var file in DirInfo.GetFiles("*" + text + "*.*", SearchOption.AllDirectories))
            {
                Console.WriteLine($"<FILE> {file.FullName}");
            }
            Console.WriteLine();
        }

        private void Rename(string arg1, string arg2) // Групповое переименование файлов.
        {
            FileInfo[] files = DirInfo.GetFiles();
            if (arg1 == "*")
            {
                int c = 0;
                for (int i = 0; i < files.Length; i++)
                {
                    File.Move(files[i].Name, $"{arg2}({c++}){files[i].Extension}");
                }
            }
            else
            {
                if (File.Exists(arg1))
                {
                    File.Move(arg1, $"{arg2}{new FileInfo(arg1).Extension}");
                }
            }
        }

        private void Move(string source, string destination) // Перемещение файлов и папок.
        {
            if (File.Exists(source))
            {
                File.Move(source, destination);
            }
            if (Directory.Exists(source))
                Directory.Move(source, destination);
        }

        private void Cat(string fileName)
        {
            if (!File.Exists(fileName))
            {
                using (File.CreateText(fileName)) { }
            }
            else
            {
                FileInfo fileInfo = new FileInfo(fileName);

                using (StreamReader sr = File.OpenText(fileName))
                {
                    string str = "";
                    while ((str = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(str);
                    }
                }
            }
        }

        private void Dir() // Отображение всех файлов и папок в текущей директории.
        {
            var dirs = DirInfo.GetDirectories();
            var files = DirInfo.GetFiles();
            Console.WriteLine($"\t..");
            foreach (var dir in dirs)
            {
                Console.WriteLine($"{dir.LastWriteTime:dd.MM.yyyy HH:mm} {"<DIR>",15} \\{dir}\\");
            }
            foreach (var file in files)
            {
                Console.WriteLine($"{file.LastWriteTime:dd.MM.yyyy HH:mm} {file.Length,15:N0} \\{file}");
            }
            Console.WriteLine($"\t\t\t{files.Length} File(s)\t{files.Sum(x => x.Length),15:N0} bytes");
            DrInfo = new DriveInfo(DirInfo.Root.FullName);
            Console.WriteLine($"\t\t\t{dirs.Length} Dirs(s)\t{DrInfo.AvailableFreeSpace,15:N0} bytes free");
            Console.WriteLine();
        }

        private void Attrib(string fileName) // Просмотр атрибутов указанного файла.
        {
            if (File.Exists(fileName))
            {
                FileInfo fi = new FileInfo(fileName);
                Console.WriteLine($" {fi.Attributes} \t {fi.FullName}");
            }
        }

        private void Help(string command) // Bвод команды help с ключом, чтобы узнать детали о конкретной команде.
        {
            if (command == "")
            {
                Console.WriteLine($"\n help \t - Provides Help information for Console commands.\n" +
                    $" cls \t - Clears the screen.\n" +
                    $" dir \t - Displays a list of files and subdirectories in a directory.\n" +
                    $" find \t - Searching files.\n" +
                    $" cd \t - Displays the name of or changes the current directory.\n" +
                    $" copy \t - Copies one or more files to another location.\n" +
                    $" move \t - Moves files from one directory to another directory.\n" +
                    $" cat \t - Creates files and print on the standard output.\n" +
                    $" rename\t - Renames a file or files.\n" +
                    $" mkdir \t - Creates a directory.\n" +
                    $" del \t - Deletes one or more files.\n" +
                    $" rmdir \t - Removes a directory.\n" +
                    $" attrib\t - Displays file attributes.\n" +
                    $" history - History of entered commands.\n" +
                    $" exit \t - Quits the Console program (command interpreter).");
                Console.WriteLine();
            }
            else
            {
                switch (command)
                {
                    case "help": Console.WriteLine($" help \t - Provides Help information for Console commands. [help command]\n"); break;
                    case "cls": Console.WriteLine($" cls \t - Clears the screen.\n"); break;
                    case "dir": Console.WriteLine($" dir \t - Displays a list of files and subdirectories in a directory.\n"); break;
                    case "find": Console.WriteLine($" find \t - Searching files. [find name]\n"); break;
                    case "cd": Console.WriteLine($" cd \t - Displays the name of or changes the current directory. [cd path]\n"); break;
                    case "copy": Console.WriteLine($" copy \t - Copies one or more files to another location. [copy source destination]\n"); break;
                    case "move": Console.WriteLine($" move \t - Moves files from one directory to another directory. [move source destination]\n"); break;
                    case "cat": Console.WriteLine($" cat \t - Creates files and print on the standard output. [cat filename]\n"); break;
                    case "rename": Console.WriteLine($" rename\t - Renames a file or files(use \'*\' for all in directory). [rename * resFileName] [rename name newName]\n"); break;
                    case "mkdir": Console.WriteLine($" mkdir \t - Creates a directory. [mkdir dirName]\n"); break;
                    case "del": Console.WriteLine($" del \t - Deletes one or more files. [del fileName or dirName]\n"); break;
                    case "rmdir": Console.WriteLine($" rmdir \t - Removes a directory. [rmdir dirName]\n"); break;
                    case "attrib": Console.WriteLine($" attrib\t - Displays file attributes. [attrib fileName]\n"); break;
                    case "history": Console.WriteLine($" history - History of entered commands.\n"); break;
                    case "exit": Console.WriteLine($" exit \t - Quits the Console program (command interpreter)."); break;
                }
            }
        }

        private void PrintHistory() // просмотр истории введенных команд.
        {
            foreach (string item in HistoryOfCommands)
            {
                Console.WriteLine($"\t {item}");
            }
            Console.WriteLine();
        }

        private string[] GetCommands(string input) // Обработка команд с учетом кавычек.
        {
            bool newWord = true;
            int c = 0;
            string[] tmp = new string[c];
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == ' ')
                {
                    newWord = true;
                }
                else
                {
                    if (input[i] == '"')
                    {
                        Array.Resize(ref tmp, tmp.Length + 1);
                        i++;
                        do
                        {
                            tmp[tmp.Length - 1] += input[i++];
                        } while (input[i] != '"' && i < input.Length);
                        newWord = true;
                    }
                    else
                    {
                        if (newWord)
                        {
                            Array.Resize(ref tmp, tmp.Length + 1);
                            newWord = false;
                        }

                        tmp[tmp.Length - 1] += input[i];
                    }
                }
            }
            return tmp;
        }

        public void Menu() // Menu
        {
            DirInfo = new DirectoryInfo(".");
            HistoryOfCommands = new List<string>();
            Commands = new string[0];
            string act = "";

            Console.WriteLine($"Enter \"help\" for information Console commands.");

            do
            {
                Console.Write($"User@PC:{DirInfo.Name}\\> ");
                act = Console.ReadLine();
                Commands = GetCommands(act);

                HistoryOfCommands.Add(act);

                switch (Commands[0])
                {
                    case "help":
                        try
                        {
                            Help(Commands.Length > 1 ? Commands[1] : "");
                        }
                        catch (Exception ex)
                        { Console.WriteLine(ex.Message); }
                        break;
                    case "cls":
                        Console.Clear();
                        break;
                    case "dir":
                        Dir();
                        break;
                    case "find":
                        try
                        {
                            Find(Commands.Length > 1 ? Commands[1] : "");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    case "cd":
                        try
                        {
                            CD(Commands.Length > 1 ? Commands[1] : ".");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    case "copy":
                        try
                        {
                            Copy(Commands.Length > 1 ? Commands[1] : "", Commands.Length > 2 ? Commands[2] : "");
                        }
                        catch (Exception ex)
                        { Console.WriteLine(ex.Message); }
                        break;
                    case "cp":
                        try
                        {
                            Copy(Commands.Length > 1 ? Commands[1] : "", Commands.Length > 2 ? Commands[2] : "");
                        }
                        catch (Exception ex)
                        { Console.WriteLine(ex.Message); }
                        break;
                    case "move":
                        try
                        {
                            Move(Commands.Length > 1 ? Commands[1] : "", Commands.Length > 2 ? Commands[2] : "");
                        }
                        catch (Exception ex)
                        { Console.WriteLine(ex.Message); }
                        break;
                    case "rename":
                        try
                        {
                            Rename(Commands.Length > 1 ? Commands[1] : "", Commands.Length > 2 ? Commands[2] : "");
                        }
                        catch (Exception ex)
                        { Console.WriteLine(ex.Message); }
                        break;
                    case "rmdir":
                        try
                        {
                            RmDir(Commands.Length > 1 ? Commands[1] : "", Commands.Length > 2 ? Commands[2] : "");
                        }
                        catch (Exception ex)
                        { Console.WriteLine(ex.Message); }
                        break;
                    case "del":
                        try
                        {
                            Del(this.Commands.Length > 1 ? this.Commands[1] : "", this.Commands.Length > 2 ? this.Commands[2] : "");
                        }
                        catch (Exception ex)
                        { Console.WriteLine(ex.Message); }
                        break;
                    case "cat":
                        try
                        {
                            Cat(this.Commands.Length > 1 ? this.Commands[1] : "");
                        }
                        catch (Exception ex)
                        { Console.WriteLine(ex.Message); }
                        break;
                    case "mkdir":
                        try
                        {
                            MkDir(this.Commands.Length > 1 ? this.Commands[1] : "");
                        }
                        catch (Exception ex)
                        { Console.WriteLine(ex.Message); }
                        break;
                    case "attrib":
                        try
                        {
                            Attrib(this.Commands.Length > 1 ? this.Commands[1] : "");
                        }
                        catch (Exception ex)
                        { Console.WriteLine(ex.Message); }
                        break;
                    case "exit":
                        break;
                    case "history":
                        PrintHistory();
                        break;
                    case "":
                        break;
                    default:
                        Console.WriteLine($" \"{this.Commands[0]}\" is not recognized as an internal or external command.");
                        break;
                }
                /*foreach (var com in Commands)
                {
                    Console.Write($" {com}");
                }
                Console.WriteLine();*/
            } while (Commands[0] != "exit");
        }
    }
}
