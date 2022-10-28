namespace Protocol
{
    public class Constants
    {
        public static readonly int FixedCommandDataSize = 2;
        public static readonly int FixedStatusSize = 3;
        public static readonly int FixedDataSize = 8;

        public static readonly int FixedFileNameSize = 4;
        public static readonly int FixedFileSize = 8;
        public static readonly int MaxPackageSize = 32768; // 32kb

        public static int WordLength = 4;
    }
}