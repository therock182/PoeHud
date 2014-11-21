using System.Collections.Generic;
using PoeHUD.Framework;

namespace PoeHUD.Controllers
{
	public class FileInMemory
	{
        public Memory M { get; private set; }
		public  int Address { get; private set; }

		public FileInMemory(Memory m, int address)
		{
			M = m;
			Address = address;
		}

	    private int NumberOfRecords
	    {
	        get { return M.ReadInt(this.Address + 0x44); }
	    }

	    protected IEnumerable<int> RecordAddresses()
		{
			int firstRec = M.ReadInt(this.Address + 0x30);
			int lastRec = M.ReadInt(this.Address + 0x34);
			int cnt = NumberOfRecords;

			int recLen = ( lastRec - firstRec ) / cnt;
			for (int i = 0; i < cnt; i++)
				yield return firstRec + i * recLen;
		}
	}
}