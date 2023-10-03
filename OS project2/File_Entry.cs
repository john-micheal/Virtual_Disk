using System;
using System.Collections.Generic;
using System.Text;

namespace The_Gang
{
    class File_Entry : Directory_Entry
    {
        public string content;
        public Directory parent;
        public File_Entry(string name, byte dir_attr, int dir_firstCluster, int fileSize, Directory pa, string Content) : base(name, dir_attr, dir_firstCluster, fileSize)
        {
            content = Content;
            if (pa != null)
                parent = pa;
        }
        List<byte> lsOfBytes = new List<byte>();
        public void readFileContent()
        {
            if (FileFirstCluster != 0)
            {
                string Content = string.Empty;
                int cluster = FileFirstCluster;
                int next = FatTable.GetNext(cluster);
                List<byte> ls = new List<byte>();
                do
                {
                    ls.AddRange(Virsual_Disk.ReadBlock(cluster));
                    cluster = next;
                    if (cluster != -1)
                        next = FatTable.GetNext(cluster);
                }
                while (next != -1);
                content += BytesToString(ls.ToArray());
            }
        }
        public void writeFileContent()
        {
            byte[] contentBYTES = StringToBytes(content);
            List<byte[]> bytesls = splitBytes(contentBYTES);
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
        }
        public void DeleteFileContent()
        {
            if (FileFirstCluster != 0)
            {
                int index = FileFirstCluster;
                int Next = -1;
                do
                {
                    FatTable.SetNext(index, 0);
                    Next = index;

                    if (index != -1)
                        index = FatTable.GetNext(index);

                } while (Next != -1);
            }
            if (parent != null)
            {
                parent.ReadDirectory();
                int Index = parent.SearchDirectory(FileOrDirectoryName.ToString());
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

        public static byte[] StringToBytes(string s)
        {
            byte[] bytes = new byte[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                bytes[i] = (byte)s[i];
            }
            return bytes;
        }
        public static string BytesToString(byte[] bytes)
        {
            string s = string.Empty;
            for (int i = 0; i < bytes.Length; i++)
            {
                if ((char)bytes[i] != '0')
                    s += (char)bytes[i];
                else
                    break;
            }
            return s;
        }


    }
}