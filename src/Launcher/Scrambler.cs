using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Scrambler
{
    class Scrambler
    {
        #region VARIABLES
        private const int OFFSET_CHECKSUM = 0x12;
        #endregion

        #region METHODS
        public static ushort GetCSum(string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException("Invalid exe name");
            return GetCSum(File.ReadAllBytes(fileName));
        }
        public static ushort GetCSum(byte[] fileData)
        {
            if (fileData.Length < OFFSET_CHECKSUM + 1)
                throw new ArgumentException("Invalid data");
            return BitConverter.ToUInt16(fileData, OFFSET_CHECKSUM);
        }
        public static void ScrambleCsum(string sourceFile, string destFile, ushort checkSum)
        {
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("Invalid exe name");
            ScrambleCsum(File.ReadAllBytes(sourceFile), destFile, checkSum);
        }
        public static void ScrambleCsum(byte[] data, string destFile, ushort checkSum)
        {
            byte[] checkSumData = BitConverter.GetBytes(checkSum);
            checkSumData.CopyTo(data, OFFSET_CHECKSUM);
            File.WriteAllBytes(destFile, data);
        }
        #endregion
    }
}
