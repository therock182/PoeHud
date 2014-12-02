namespace PoeHUD.Models
{
    public struct Pattern
    {
        public byte[] Bytes;
        public string Mask;

        public Pattern(byte[] pattern, string mask)
        {
            Bytes = pattern;
            Mask = mask;
        }
    }
}