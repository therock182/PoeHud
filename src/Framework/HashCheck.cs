using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PoeHUD.Framework
{
    class HashCheck
    {
        #region VARIABLES
        private const int OFFSET_CHECKSUM = 0x12;
        #endregion

        #region METHODS
        public static ushort GetCSum(string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException("Invalid fileName");
            return GetCSum(File.ReadAllBytes(fileName));
        }
        public static ushort GetCSum(byte[] fileData)
        {
            if (fileData.Length < OFFSET_CHECKSUM + 1)
                throw new ArgumentException("Invalid fileData");
            return BitConverter.ToUInt16(fileData, OFFSET_CHECKSUM);
        }
        #endregion
    }
}
