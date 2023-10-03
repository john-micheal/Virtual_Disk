using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace The_Gang
{
    class FatTable
    {
        static int[] FatTable1 = new int[1024];
        static byte[] arrOfByte = new byte[4 * 1024];
        public FatTable()
        {
            FatTable1 = new int[1024];
        }

        public static void initialize()
        {
            for (int i = 0; i < FatTable1.Length; i++)
            {
                if (i < 5)
                    FatTable1[i] = -1;
                else
                    FatTable1[i] = 0;
            }
        }
        public static void WriteFatTable()
        {
            Virsual_Disk.Disk = new FileStream(@"G:\j\OS project\OS project2\bin\Debug\netcoreapp3.1\john.txt", FileMode.Open, FileAccess.Write);
            // Function that make the cursor stop after the first block
            Virsual_Disk.Disk.Seek(1024, SeekOrigin.Begin);

            // Function that convert the int to byte and store tnem in arrOfByte 
            Buffer.BlockCopy(FatTable1, 0, arrOfByte, 0, arrOfByte.Length);

            // store the arrOfByte in myFile
            Virsual_Disk.Disk.Write(arrOfByte, 0, arrOfByte.Length);
            Virsual_Disk.Disk.Close();
        }
        public static int[] ReadFatTable()
        {
            Virsual_Disk.Disk = new FileStream(@"G:\j\OS project\OS project2\bin\Debug\netcoreapp3.1\john.txt", FileMode.Open, FileAccess.Read);
            Virsual_Disk.Disk.Seek(1024, SeekOrigin.Begin);
            // read fat table and store the values in arrOfByte
            Virsual_Disk.Disk.Read(arrOfByte, 0, arrOfByte.Length);
            // convert from byte to int and store them in FatTable1
            Buffer.BlockCopy(arrOfByte, 0, FatTable1, 0, 4096);
            Virsual_Disk.Disk.Close();
            return FatTable1;
        }
        public void PrintFatTable1ForTest()
        {

            ReadFatTable();

            for (int i = 0; i < FatTable1.Length; i++)
            {
                Console.WriteLine(i + 1 + "\t-->\t" + FatTable1[i]);
            }

        }
        public static int Getavaliableblock()
        {
            int freeIndex = -1;
            for (int i = 0; i < 1024; i++)
            {
                if (FatTable1[i] == 0)
                {
                    freeIndex = i;
                    break;
                }

            }
            return freeIndex;
        }
        public static int GetAvilaibleBlocks()
        {
            int count = 0;
            for (int i = 0; i < FatTable1.Length; i++)
            {
                if (FatTable1[i] == 0)
                {
                    count++;
                }
            }
            return count;
        }
        public static int GetNext(int index)
        {
            return FatTable1[index];
        }
        public static void SetNext(int index, int value)
        {
            FatTable1[index] = value;
        }
        public static int GetFreeSpace()
        {
            int FreeSpace = GetAvilaibleBlocks() * 1024;
            return FreeSpace;
        }
    }
}
