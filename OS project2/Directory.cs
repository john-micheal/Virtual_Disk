using System;
using System.Collections.Generic;
using System.Text;

namespace The_Gang
{
    class Directory : Directory_Entry
    {
        public List<Directory_Entry> DirectoryTable;
        public Directory parent = null;
        public Directory(string namefile, byte attributefile, int firstClustumfile, int FileSizeD, Directory p)
            : base
            (namefile,
             attributefile,
             firstClustumfile, FileSizeD)
        {
            DirectoryTable = new List<Directory_Entry>();
            if (p != null)
            {
                parent = p;
            }
        }

        public Directory_Entry GetDirectoryEntry()
        {
            Directory_Entry me = new Directory_Entry(new string(FileOrDirectoryName), fileAttribute, FileFirstCluster, FileSize);
            return me;
        }

        public void WriteDirectory()
        {
            byte[] dirsorfilesBYTES = new byte[DirectoryTable.Count * 32];
            for (int i = 0; i < DirectoryTable.Count; i++)
            {
                byte[] b = GetBytes(DirectoryTable[i]);
                for (int j = i * 32, k = 0; k < b.Length; k++, j++)
                    dirsorfilesBYTES[j] = b[k];
            }
            List<byte[]> bytesls = splitBytes(dirsorfilesBYTES);
            int clusterFATIndex;
            if (FileFirstCluster != 0)
            {
                clusterFATIndex = FileFirstCluster;
            }
            else
            {
                clusterFATIndex = FatTable.Getavaliableblock();
                FileFirstCluster = clusterFATIndex;
            }
            int lastCluster = -1;
            for (int i = 0; i < bytesls.Count; i++)
            {
                if (clusterFATIndex != -1)
                {
                    Virsual_Disk.WriteBlock(bytesls[i], clusterFATIndex, 0, bytesls[i].Length);
                    FatTable.SetNext(clusterFATIndex, -1);
                    if (lastCluster != -1)
                        FatTable.SetNext(lastCluster, clusterFATIndex);
                    lastCluster = clusterFATIndex;
                    clusterFATIndex = FatTable.Getavaliableblock();
                }
            }
            if (parent != null)
            {
                parent.UpdateContentOfDirectory(GetDirectoryEntry());
                parent.WriteDirectory();
            }
            FatTable.WriteFatTable();
        }


        public void ReadDirectory()
        {
            if (FileFirstCluster != 0)
            {
                int fatIndex = FileFirstCluster;

                int next = FatTable.GetNext(fatIndex);
                List<byte> lsOfBytes = new List<byte>();
                List<Directory_Entry> dt = new List<Directory_Entry>();

                do
                {
                    lsOfBytes.AddRange(Virsual_Disk.ReadBlock(fatIndex));
                    fatIndex = next;
                    if (fatIndex != -1)
                    {
                        next = FatTable.GetNext(fatIndex);
                    }
                } while (next != -1);

                for (int i = 0; i < lsOfBytes.Count; i++)
                {
                    byte[] b = new byte[32];
                    for (int k = i * 32, m = 0; m < b.Length && k < lsOfBytes.Count; m++, k++)
                    {
                        b[m] = lsOfBytes[k];
                    }
                    if (b[0] == 0)
                        break;
                    dt.Add(GetDirectoryEntry(b));
                }

            }



        }

        public int SearchDirectory(string Name)
        {
            if (Name.Length < 11)
            {
                Name += "\0";
                for (int i = Name.Length + 1; i < 12; i++)
                    Name += " ";
            }
            else
            {
                Name = Name.Substring(0, 11);
            }
            for (int i = 0; i < DirectoryTable.Count; i++)
            {

                string n = new string(DirectoryTable[i].FileOrDirectoryName);
                if (n.Equals(Name))
                    return i;

            }

            return -1;
        }

        public int Searchfile(string Name)
        {
            if (Name.Length < 11)
            {

                for (int i = Name.Length + 1; i < 12; i++)
                    Name += " ";
            }
            else
            {
                Name = Name.Substring(0, 11);
            }
            for (int i = 0; i < DirectoryTable.Count; i++)
            {

                string n = new string(DirectoryTable[i].FileOrDirectoryName);

                if (n.Equals(Name))
                    return i;

            }

            return -1;
        }
        public void UpdateContentOfDirectory(Directory_Entry d)
        {
            int index = SearchDirectory(new string(d.FileOrDirectoryName));
            if (index != -1)
            {
                DirectoryTable.RemoveAt(index);
                DirectoryTable.Insert(index, d);
            }
        }

        public void DeleteDirectory()
        {
            if (FileFirstCluster != 0)
            {
                int index = FileFirstCluster;
                int next = -1;
                do
                {
                    FatTable.SetNext(index, 0);
                    next = index;

                    if (index != -1)
                        index = FatTable.GetNext(index);

                } while (next != -1);
            }
            if (parent != null)
            {
                parent.ReadDirectory();
                int Index = parent.SearchDirectory(new string(FileOrDirectoryName));
                if (Index != -1)
                {
                    parent.DirectoryTable.RemoveAt(Index);
                    parent.WriteDirectory();
                }
            }
        }
        public static List<byte[]> splitBytes(byte[] bytes)
        {
            List<byte[]> ls = new List<byte[]>();
            int number_of_arrays = bytes.Length / 1024;
            int rem = bytes.Length % 1024;
            for (int i = 0; i < number_of_arrays; i++)
            {
                byte[] b = new byte[1024];
                for (int j = i * 1024, k = 0; k < 1024; j++, k++)
                {
                    b[k] = bytes[j];
                }
                ls.Add(b);
            }
            if (rem > 0)
            {
                byte[] b1 = new byte[1024];
                for (int i = number_of_arrays * 1024, k = 0; k < rem; i++, k++)
                {
                    b1[k] = bytes[i];
                }
                ls.Add(b1);
            }
            return ls;
        }


    }
}
