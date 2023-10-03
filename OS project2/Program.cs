using System;
using System.IO;
using System.Text;

namespace The_Gang
{
    class Program
    {
        public static Directory currentDirectory;
        public static string currentPath;

        static void Main(string[] args)
        {
            Console.WriteLine("\t \t \t  \t \t  \t     WeLcOmE.          ");
            Virsual_Disk.Initalize("virtualDisk");
            currentPath = new string(currentDirectory.FileOrDirectoryName);
            while (true)
            {
                Console.Write(currentPath.Trim());
                string Enter = Console.ReadLine();
                if (!Enter.Contains(" "))
                {
                    if (Enter.ToLower() == "help")
                    {
                        Cmd.Help();
                    }
                    else if (Enter.ToLower() == "quit")
                    {
                        FatTable.WriteFatTable();
                        Virsual_Disk.Disk.Close();
                        Cmd.Quit();
                    }
                    else if (Enter.ToLower() == "cls")
                    {
                        Cmd.Cls();
                    }
                    else if (Enter.ToLower() == "md")
                    {
                        Console.WriteLine("The syntax of the command is incorrect.");
                    }
                    else if (Enter.ToLower() == "rd")
                    {
                        Console.WriteLine("The syntax of the command is incorrect.");
                    }
                    else if (Enter.ToLower() == "dir")
                    {
                        Cmd.Dir();
                    }
                    else if (Enter.ToLower() == "cd")
                    {
                        Console.WriteLine("K:\n");
                    }
                    else
                    {
                        Console.WriteLine("No command with this syntax.");
                    }
                }
                else if (Enter.Contains(" "))
                {
                    string[] EnterSplit = Enter.Split(" ");
                    if (EnterSplit[0].ToLower() == "md")
                    {
                        Cmd.Md(EnterSplit[1]);
                    }
                    else if (EnterSplit[0].ToLower() == "rd")
                    {
                        Cmd.Rd(EnterSplit[1]);
                    }
                    else if (EnterSplit[0].ToLower() == "cd")
                    {
                        Cmd.Cd(EnterSplit[1]);
                    }
                    else if (EnterSplit[0].ToLower() == "dir")
                    {
                        Cmd.Dir();
                    }

                    else if (EnterSplit[0].ToLower() == "import")
                    {
                        Cmd.Import(EnterSplit[1]);
                    }
                    else if (EnterSplit[0].ToLower() == "type")
                    {
                        Cmd.Type(EnterSplit[1]);
                    }
                    else if (EnterSplit[0].ToLower() == "export")
                    {
                        Cmd.export(EnterSplit[1], EnterSplit[2]);
                    }

                    else if (EnterSplit[0].ToLower() == "rename")
                    {
                        Cmd.rename(EnterSplit[1], EnterSplit[2]);
                    }
                    else if (EnterSplit[0].ToLower() == "del")
                    {
                        Cmd.del(EnterSplit[1]);
                    }
                    else if (EnterSplit[0].ToLower() == "cp")
                    {
                        Cmd.copy(EnterSplit[1], EnterSplit[2]);
                    }
                    else if (EnterSplit[0].ToLower() == "help")
                    {
                        if (EnterSplit.Length > 2)
                        {
                            Console.WriteLine("Error: " + EnterSplit[0] + " command syntax is \n help \n or \n help [command] \n function:Provides Help information for commands.");
                        }
                        else if (EnterSplit.Length == 2)
                        {
                            switch (EnterSplit[1])
                            {
                                case "cd":
                                    Console.WriteLine("Change the current default directory to the directory given in the argument.");
                                    Console.WriteLine("If the argument is not present, report the current directory.");
                                    Console.WriteLine("If the directory does not exist an appropriate error should be reported.");
                                    Console.WriteLine(EnterSplit[1] + " command syntax is \n cd \n or \n cd [directory]");
                                    Console.WriteLine("[directory] can be directory name or fullpath of a directory");
                                    break;
                                case "cls":
                                    Console.WriteLine("Clear the screen.");
                                    Console.WriteLine(EnterSplit[1] + " command syntax is \n cls");
                                    break;
                                case "dir":
                                    Console.WriteLine("List the contents of directory given in the argument.");
                                    Console.WriteLine("If the argument is not present, list the content of the current directory.");
                                    Console.WriteLine("If the directory does not exist an appropriate error should be reported.");
                                    Console.WriteLine(EnterSplit[1] + " command syntax is \n dir \n or \n dir [directory]");
                                    Console.WriteLine("[directory] can be directory name or fullpath of a directory");
                                    break;
                                case "quit":
                                    Console.WriteLine("Quit the shell.");
                                    Console.WriteLine(EnterSplit[1] + " command syntax is \n quit");
                                    break;
                                case "copy":
                                    Console.WriteLine("Copies one or more files to another location.");
                                    Console.WriteLine(EnterSplit[1] + " command syntax is \n copy [source]+ [destination]");
                                    Console.WriteLine("+ after [source] represent that you can pass more than file Name (or fullpath of file) or more than directory Name (or fullpath of directory)");
                                    Console.WriteLine("[source] can be file Name (or fullpath of file) or directory Name (or fullpath of directory)");
                                    Console.WriteLine("[destination] can be directory name or fullpath of a directory");
                                    break;
                                case "del":
                                    Console.WriteLine("Deletes one or more files.");
                                    Console.WriteLine("NOTE: it confirms the user choice to delete the file before deleting");
                                    Console.WriteLine(EnterSplit[1] + " command syntax is \n del [file]+");
                                    Console.WriteLine("+ after [file] represent that you can pass more than file Name (or fullpath of file)");
                                    Console.WriteLine("[file] can be file Name (or fullpath of file)");
                                    break;
                                case "help":
                                    Console.WriteLine("Provides Help information for commands.");
                                    Console.WriteLine(EnterSplit[1] + " command syntax is \n help \n or \n For more information on a specific command, type help [command]");
                                    Console.WriteLine("command - displays help information on that command.");
                                    break;
                                case "md":
                                    Console.WriteLine("Creates a directory.");
                                    Console.WriteLine(EnterSplit[1] + " command syntax is \n md [directory]");
                                    Console.WriteLine("[directory] can be a new directory name or fullpath of a new directory");
                                    break;
                                case "rd":
                                    Console.WriteLine("Removes a directory.");
                                    Console.WriteLine("NOTE: it confirms the user choice to delete the directory before deleting");
                                    Console.WriteLine(EnterSplit[1] + " command syntax is \n rd [directory]");
                                    Console.WriteLine("[directory] can be a directory name or fullpath of a directory");
                                    break;
                                case "rename":
                                    Console.WriteLine("Renames a file.");
                                    Console.WriteLine(EnterSplit[1] + " command syntax is \n rd [fileName] [new fileName]");
                                    Console.WriteLine("[fileName] can be a file name or fullpath of a filename ");
                                    Console.WriteLine("[new fileName] can be a new file name not fullpath ");
                                    break;
                                case "type":
                                    Console.WriteLine("Displays the contents of a text file.");
                                    Console.WriteLine(EnterSplit[1] + " command syntax is \n type [file]");
                                    Console.WriteLine("NOTE: it displays the filename before its content for every file");
                                    Console.WriteLine("[file] can be file Name (or fullpath of file) of text file");
                                    break;
                                case "import":
                                    Console.WriteLine("– import text file(s) from your computer ");
                                    Console.WriteLine(EnterSplit[1] + " command syntax is \n import [destination] [file]+");
                                    Console.WriteLine("+ after [file] represent that you can pass more than file Name (or fullpath of file) of text file");
                                    Console.WriteLine("[file] can be file Name (or fullpath of file) of text file");
                                    Console.WriteLine("[destination] can be directory name or fullpath of a directory in your implemented file system");
                                    break;
                                case "export":
                                    Console.WriteLine("– export text file(s) to your computer ");
                                    Console.WriteLine(EnterSplit[1] + " command syntax is \n export [destination] [file]+");
                                    Console.WriteLine("+ after [file] represent that you can pass more than file Name (or fullpath of file) of text file");
                                    Console.WriteLine("[file] can be file Name (or fullpath of file) of text file in your implemented file system");
                                    Console.WriteLine("[destination] can be directory name or fullpath of a directory in your computer");
                                    break;
                            }
                        }

                        else
                        {
                            Console.WriteLine("you can Press just  help or help then one command no thing else please.");
                        }
                    }

                }
            }




        }
    }
}
