using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Gang
{
    class Virsual_Disk
    {
        public static FileStream Disk;
        public static void CREATE_Disk(string path)
        {
            Disk = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }

        public static int GetFreeSpace()
        {
            return 1024 * 1024 - (int)Disk.Length;
        }

        public static void Initalize(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    CREATE_Disk(path);
                    byte[] b = new byte[1024];
                    for (int i = 0; i < b.Length; i++)
                        b[i] = 0;
                    WriteBlock(b, 0);
                    FatTable.initialize();
                    Directory root = new Directory("K:", 0x10, 5, 0, null);

                    root.WriteDirectory();
                    FatTable.SetNext(5, -1);
                    Program.currentDirectory = root;
                    FatTable.WriteFatTable();
                }
                else
                {
                    CREATE_Disk(path);
                    FatTable.ReadFatTable();
                    Directory root = new Directory("K:", 0x10, 5, 0, null);
                    root.ReadDirectory();
                    Program.currentDirectory = root;
                }
            }
            catch (IOException ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }

        public static void WriteBlock(byte[] data, int Index, int offset = 0, int count = 1024)
        {
            Disk = new FileStream(@"G:\j\OS project\OS project2\bin\Debug\netcoreapp3.1\john.txt", FileMode.Open, FileAccess.Write);
            Disk.Seek(Index * 1024, SeekOrigin.Begin);
            Disk.Write(data, offset, count);
            Disk.Flush();
            Disk.Close();
        }

        public static byte[] ReadBlock(int clusterIndex)
        {
            Disk = new FileStream(@"G:\j\OS project\OS project2\bin\Debug\netcoreapp3.1\john.txt", FileMode.Open, FileAccess.Read);
            Disk.Seek(clusterIndex * 1024, SeekOrigin.Begin);
            byte[] bytes = new byte[1024];
            Disk.Read(bytes, 0, 1024);
            Disk.Close();
            return bytes;
        }
    }
}
