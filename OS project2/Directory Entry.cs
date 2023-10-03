using System;
using System.Collections.Generic;
using System.Text;

namespace The_Gang
{
    class Directory_Entry
    {
        public char[] FileOrDirectoryName = new char[11];
        public byte fileAttribute;
        public byte[] fileEmpty = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public int FileSize;
        public int FileFirstCluster;

        public Directory_Entry(string name, byte attribute, int FirstCluster, int FSize)
        {

            fileAttribute = attribute;

            if (fileAttribute == 0x0)
            {

                string[] FileName = name.Split('.');
                AssignFileName(FileName[0].ToCharArray(), FileName[1].ToCharArray());
            }
            else
            {
                AssignDirectoryName(name.ToCharArray());
            }

            FileFirstCluster = FirstCluster;
            FileSize = FSize;
        }

        public void AssignFileName(char[] name, char[] extension)
        {
            if (name.Length <= 7 && extension.Length == 3)
            {
                int j = 0;
                for (int i = 0; i < name.Length; i++)
                {
                    j++;
                    FileOrDirectoryName[i] = name[i];
                }
                FileOrDirectoryName[j] = '.';
                for (int i = 0; i < extension.Length; i++)
                {
                    j++;
                    FileOrDirectoryName[j] = extension[i];
                }
                for (int i = ++j; i < FileOrDirectoryName.Length; i++)
                {
                    FileOrDirectoryName[i] = ' ';
                }
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    FileOrDirectoryName[i] = name[i];
                }
                FileOrDirectoryName[7] = '.';
                for (int i = 0, j = 8; i < extension.Length; j++, i++)
                {
                    FileOrDirectoryName[j] = extension[i];
                }
            }
        }

        public void AssignDirectoryName(char[] name)
        {
            if (name.Length <= 11)
            {
                int j = 0;
                for (int i = 0; i < name.Length; i++)
                {
                    j++;
                    FileOrDirectoryName[i] = name[i];
                }
                for (int i = ++j; i < FileOrDirectoryName.Length; i++)
                {
                    FileOrDirectoryName[i] = ' ';
                }
            }
            else
            {
                int j = 0;
                for (int i = 0; i < 11; i++)
                {
                    j++;
                    FileOrDirectoryName[i] = name[i];
                }
            }
        }
        public static byte[] Directory_EntryToBytes(Directory_Entry d)
        {
            byte[] bytes = new byte[32];
            for (int i = 0; i < d.FileOrDirectoryName.Length; i++)
            {
                bytes[i] = (byte)d.FileOrDirectoryName[i];
            }
            bytes[11] = d.fileAttribute;
            int j = 12;
            for (int i = 0; i < d.fileEmpty.Length; i++)
            {
                bytes[j] = d.fileEmpty[i];
                j++;
            }
            byte[] fc = BitConverter.GetBytes(d.FileFirstCluster);
            for (int i = 0; i < fc.Length; i++)
            {
                bytes[j] = fc[i];
                j++;
            }
            byte[] sz = BitConverter.GetBytes(d.FileSize);
            for (int i = 0; i < sz.Length; i++)
            {
                bytes[j] = sz[i];
                j++;
            }
            return bytes;

        }
        public byte[] GetBytes(Directory_Entry e)
        {

            byte[] b = new byte[32];

            for (int i = 0; i < 11; i++)
            {
                b[i] = (byte)FileOrDirectoryName[i];
            }

            b[11] = fileAttribute;

            for (int i = 12, j = 0; i < 24 && j < 12; i++, j++)
            {
                b[i] = fileEmpty[j];
            }

            for (int i = 24; i < 28; i++)
            {
                b[i] = (byte)FileFirstCluster;
            }

            for (int i = 28; i < 32; i++)
            {
                b[i] = (byte)FileSize;
            }

            return b;
        }

        public Directory_Entry GetDirectoryEntry(byte[] b)
        {

            for (int i = 0; i < 11; i++)
            {
                FileOrDirectoryName[i] = (char)b[i];
            }

            fileAttribute = b[11];

            for (int i = 12, j = 0; i < 24 && j < 12; i++, j++)
            {
                fileEmpty[j] = b[i];
            }

            for (int i = 24; i < 28; i++)
            {
                FileFirstCluster = b[i];
            }

            for (int i = 28; i < 32; i++)
            {
                FileSize = b[i];
            }

            Directory_Entry d1 = new Directory_Entry(new string(FileOrDirectoryName), fileAttribute, FileFirstCluster, FileSize);
            return d1;
        }


    }
}
