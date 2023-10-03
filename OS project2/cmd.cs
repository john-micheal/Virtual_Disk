using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace The_Gang
{
    class Cmd
    {
        public static void Cls()
        {
            Console.Clear();
        }

        public static void Help()
        {
            Console.WriteLine("cls\t:\t Clear the screen.");
            Console.WriteLine("quit\t:\t Quits the Cmd.exe program.");
            Console.WriteLine("help\t:\t Provides help information for windows command.");
            Console.WriteLine("md\t:\t Creates a directory.");
            Console.WriteLine("cd\t:\t Display the name of or change the current directory.");
            Console.WriteLine("rd\t:\tRemove a directory.");
        }

        public static void Quit()
        {
            FatTable.WriteFatTable();
            Virsual_Disk.Disk.Close();
            Environment.Exit(0);

        }

        public static void Md(string Name)
        {
            if (Program.currentDirectory.SearchDirectory(Name) == -1)
            {

                Directory_Entry newdirectory = new Directory_Entry(Name, 0x10, 0, 0);
                Program.currentDirectory.DirectoryTable.Add(newdirectory);
                Program.currentDirectory.WriteDirectory();
                if (Program.currentDirectory.parent != null)
                {
                    Program.currentDirectory.parent.UpdateContentOfDirectory(Program.currentDirectory.parent);
                    Program.currentDirectory.parent.WriteDirectory();
                }
                Console.WriteLine(Name + "  directory is been created");
            }
            else
            {
                Console.WriteLine("A directory " + Name + " already exists.");
            }
        }

        public static void Rd(string Name)
        {
            int index = Program.currentDirectory.SearchDirectory(Name);
            if (index != -1)
            {
                int firstCluster = Program.currentDirectory.DirectoryTable[index].FileFirstCluster;
                int fileSize = Program.currentDirectory.DirectoryTable[index].FileSize;
                Directory d1 = new Directory(Name, 0x10, firstCluster, fileSize, Program.currentDirectory);
                d1.DeleteDirectory();
                Program.currentPath = new string(Program.currentDirectory.FileOrDirectoryName).Trim();
                Console.WriteLine(Name + "\tdirectory is been removed.");
            }
            else
            {
                Console.WriteLine("The system cannot find " + Name + " directory.");
            }
        }

        public static string Import(string Path)
        { 
            string Name = "";
            if (File.Exists(Path))
            {
                char[] separators = new char[2];
                separators[0] = '\\';
                separators[1] = ':';
                string[] nam = Path.Split(separators);
                Name = nam[nam.Length - 1];

                string Content = " ";

                if (Program.currentDirectory.Searchfile(Name) == -1)
                {
                    Content += File.ReadAllText(Path);
                    int size = Content.Length;
                    int firstCluster = 0;
                    if (size > 0)
                    {
                        firstCluster = FatTable.Getavaliableblock();
                    }

                    File_Entry file = new File_Entry(Name, 0x0, firstCluster, size, Program.currentDirectory, Content);
                    file.writeFileContent();
                    Directory_Entry fil = new Directory_Entry(Name, 0x0, firstCluster, size);
                    Program.currentDirectory.DirectoryTable.Add(fil);


                    if (Program.currentDirectory.parent != null)
                    {
                        Program.currentDirectory.parent.UpdateContentOfDirectory(Program.currentDirectory.parent);
                        Program.currentDirectory.parent.WriteDirectory();
                    }

                    Program.currentDirectory.WriteDirectory();
                }
                else if (Program.currentDirectory.Searchfile(Name) != -1)
                {
                    Console.WriteLine("file name is been doubled");
                }
            }
            else if (!File.Exists(Path))
            {
                Console.WriteLine("Path isn't correct");
            }
            return Name;
        }
        public static void copy(string source, string dest)
        {
            string name = Import(source);
            export(name, dest);
        }
        public static void export(string source, string dest)
        {
            int name_start = source.LastIndexOf(".");
            string filename = source.Substring(name_start + 1);
            if (filename == "txt")
            {
                int index = Program.currentDirectory.Searchfile(source);
                if (index != -1)
                {
                    if (System.IO.Directory.Exists(dest))
                    {
                        int cluster = Program.currentDirectory.DirectoryTable[index].FileFirstCluster;
                        int size = Program.currentDirectory.DirectoryTable[index].FileSize;
                        string content = null;
                        File_Entry file = new File_Entry(source, 0x0, cluster, size, Program.currentDirectory, content);
                        file.readFileContent();
                        StreamWriter st = new StreamWriter(dest + "\\" + source);
                        st.Write(file.content);
                        st.Flush();
                        st.Close();
                    }
                    else if (index == -1)
                    {
                        Console.WriteLine("The system can not find this file");
                    }
                }
                else if (filename != "txt")
                {
                    Console.WriteLine("This file is not exist");
                }
            }
            else
            {
                Console.WriteLine("enter file name");
            }
        }

        public static void rename(string oldName, string newName)
        {
            int oldIndex2 = Program.currentDirectory.SearchDirectory(oldName);
            int oldIndex = Program.currentDirectory.Searchfile(oldName);
            if (oldIndex != -1)
            {
                int newIndex = Program.currentDirectory.Searchfile(new string(newName));

                if (newIndex == -1)
                {
                    Directory_Entry d1 = new Directory_Entry(newName, Program.currentDirectory.DirectoryTable[oldIndex].fileAttribute, Program.currentDirectory.DirectoryTable[oldIndex].FileFirstCluster, Program.currentDirectory.DirectoryTable[oldIndex].FileSize);

                    Program.currentDirectory.DirectoryTable.RemoveAt(oldIndex);
                    Program.currentDirectory.DirectoryTable.Insert(oldIndex, d1);
                    Program.currentDirectory.WriteDirectory();

                }
                else
                {
                    Console.WriteLine("new name is exist write another name");
                }
            }
            else if (oldIndex2 != -1)
            {
                int newIndex2 = Program.currentDirectory.SearchDirectory(new string(newName));

                if (newIndex2 == -1)
                {
                    Directory_Entry d1 = new Directory_Entry(newName, Program.currentDirectory.DirectoryTable[oldIndex2].fileAttribute, Program.currentDirectory.DirectoryTable[oldIndex2].FileFirstCluster, Program.currentDirectory.DirectoryTable[oldIndex2].FileSize);
                    Program.currentDirectory.DirectoryTable.RemoveAt(oldIndex2);
                    Program.currentDirectory.DirectoryTable.Insert(oldIndex2, d1);
                    Program.currentDirectory.WriteDirectory();

                }
                else
                {
                    Console.WriteLine("new name is exist write another name");
                }
            }
            else
            {
                Console.WriteLine("no file or folder with this name");
            }
        }

        public static void del(string fileName)
        {
            int index = Program.currentDirectory.Searchfile(fileName);
            if (index != -1)
            {
                if (Program.currentDirectory.DirectoryTable[index].fileAttribute == 0x0)
                {
                    int cluster = Program.currentDirectory.DirectoryTable[index].FileFirstCluster;
                    int size = Program.currentDirectory.DirectoryTable[index].FileSize;
                    File_Entry f = new File_Entry(fileName, 0x0, cluster, size, Program.currentDirectory, null);
                    Program.currentDirectory.DirectoryTable.RemoveAt(index);
                }
                else
                {
                    Console.WriteLine("no file with this name");
                }
            }
        }
        public static void Type(string Name)
        {
            int index = Program.currentDirectory.Searchfile(Name);
            if (index != -1)
            {
                if (Program.currentDirectory.DirectoryTable[index].fileAttribute == 0x0)
                {
                    int FirstCluster = Program.currentDirectory.DirectoryTable[index].FileFirstCluster;
                    int FileSize = Program.currentDirectory.DirectoryTable[index].FileSize;
                    string Content = string.Empty;
                    File_Entry file = new File_Entry(Name, 0x0, FirstCluster, FileSize, Program.currentDirectory, Content);
                    file.readFileContent();
                    Console.WriteLine(file.content);
                }
            }
            else if (Program.currentDirectory.SearchDirectory(Name) != -1)
            { Console.WriteLine("it's folder not file"); }
            else if (index == -1 && index == -1)
            { Console.WriteLine("file Not Been created"); }
        }
        public static void Dir()
        {
            string name = " ";
            int numOfFiles = 0, numOfOFolders = 0, sizeOfFiles = 0;
            for (int i = 0; i < Program.currentDirectory.DirectoryTable.Count; i++)
            {
                if (Program.currentDirectory.DirectoryTable[i].fileAttribute == 0x10)
                {
                    numOfOFolders += 1;
                    for (int j = 0; j < Program.currentDirectory.DirectoryTable[i].FileOrDirectoryName.Length; j++)
                    {
                        name += Program.currentDirectory.DirectoryTable[i].FileOrDirectoryName[j];
                    }

                    Console.WriteLine("<DIR>\t" + name);
                    name = " ";
                }
                else
                {
                    numOfFiles += 1;
                    sizeOfFiles += Program.currentDirectory.DirectoryTable[i].FileSize;
                    for (int j = 0; j < Program.currentDirectory.DirectoryTable[i].FileOrDirectoryName.Length; j++)
                    {
                        name += Program.currentDirectory.DirectoryTable[i].FileOrDirectoryName[j];
                    }
                    Console.WriteLine("     \t" + name);
                    name = " ";
                }
            }
            int FreeSpace = FatTable.GetAvilaibleBlocks() * 1024; 
            Console.Write("number of files:\t" + numOfFiles + "\tsize of files:\t" + sizeOfFiles + "\nnum of folders:\t" + numOfOFolders + "\t" + FreeSpace + "Bytes\n");
        }

        public static void Cd(string Name)
        {
            int index = Program.currentDirectory.SearchDirectory(Name);
            if (Name == "..")
            {

                Program.currentPath = new string(Program.currentDirectory.parent.FileOrDirectoryName);
                Program.currentDirectory = Program.currentDirectory.parent;
                Program.currentDirectory.ReadDirectory();
            }
            else if (index != -1)
            {
                int firstCluster = Program.currentDirectory.DirectoryTable[index].FileFirstCluster;
                int fileSize = Program.currentDirectory.DirectoryTable[index].FileSize;
                Directory d1 = new Directory(Name, 0x10, firstCluster, fileSize, Program.currentDirectory);
                Program.currentPath = new string(Program.currentDirectory.FileOrDirectoryName).Trim() + "\\" + new string(d1.FileOrDirectoryName).Trim();
                Program.currentDirectory = d1;
                Program.currentDirectory.ReadDirectory();
                Console.WriteLine("The path now is " + Name);
            }
         
            else
            {
                Console.WriteLine("The system cannot find the path specified.");
            }
        }
    }
}