#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class AppleTangle
    {
        private static byte[] data = System.Convert.FromBase64String("mWfFybzXgJbR3rQTd0vj+4djFa+54KGzs7WtpbPgoaOjpbC0oa6jpXX6bTTPzsBSy3Hh1u60FfzNG6LWyOvGwcXFx8LB1t6otLSws/rv77fv8EEDxsjrxsHFxcfCwvBBdtpBc6yl4Imuo+7x5vDkxsOVxMvT3YGwiRi2X/PUpWG3VAntwsPBwMFjQsHPXf0z64no2gg+DnV5zhme3BYL/VVeusxkh0ubFNb38wsEz40O1KkRhb7fjKuQVoFJBLSiy9BDgUfzSkEJ2bI1nc4Vv59bMuXDepVPjZ3NMe6AZjeHjb/InvDfxsOV3ePE2PDW5vDkxsOVxMvT3YGwsKyl4IOlsrT18vH08PP2mtfN8/Xw8vD58vH08PBCxHvwQsNjYMPCwcLCwcLwzcbJ5CIrEXewH8+FIecKMa24LSd119cAo/O3N/rH7JYrGs/hzhp6s9mPdbKho7Spo6Xgs7ShtKWtpa60s+7wsKyl4JKvr7Tgg4Hw3tfN8Pbw9PLG8M/Gw5Xd08HBP8TF8MPBwT/w3brwQsG28M7Gw5Xdz8HBP8TEw8LBQsHAxsnqRohGN6OkxcHwQTLw6saupOCjr66kqbSpr66z4K+m4LWzpeChrqTgo6WytKmmqaOhtKmvruCwpPXj1YvVmd1zVDc2XF4PkHoBmJBoHL7i9QrlFRnPFqsUYuTj0TdhbOzgo6WytKmmqaOhtKXgsK+sqaO54IOB8ELB4vDNxsnqRohGN83BwcGppqmjobSpr67ggbW0qK+yqbS58UvZSR45i6w1x2vi8MIo2P44kMkTGfa/AUeVGWdZefKCOxgVsV6+YZJ+NLNbLhKkzwu5j/QYYv45uD+rCKKspeCztKGupKGypOC0pbKts+Ch31Eb3oeQK8UtnrlE7Sv2YpeMlSy0qK+yqbS58dbw1MbDlcTD082BsL+BaFg5EQqmXOSr0RBjeyTb6gPfT7NBoAbbm8nvUnI4hIgwoPhe1TXW8NTGw5XEw9PNgbCwrKXgkq+vtKdPyHTgNwts7OCvsHb/wfBMd4MPt7fuobCwrKXuo6+t76GwsKylo6GSpaypoa6jpeCvruC0qKmz4KOlsupGiEY3zcHBxcXA8KLxy/DJxsOVtKmmqaOhtKXgorngoa654LChsrTGw5XdzsTWxNTrEKmHVLbJPjSrTcTG08KVk/HT8NHGw5XEytPKgbCwzcbJ6kaIRjfNwcHFxcDDQsHBwJywrKXgg6WytKmmqaOhtKmvruCBtXfbfVOC5NLqB8/ddo1cnqMIi0DX30VDRdtZ/Yf3MmlbgE7sFHFQ0hhA1OsQqYdUtsk+NKtN7oBmN4eNv8XAw0LBz8DwQsHKwkLBwcAkUWnJyJ7wQsHRxsOV3eDEQsHI8ELBxPDw0cbDlcTK08qBsLCspeCJrqPu8WtjsVKHk5UBb++Bczg7I7ANJmOM8/aa8KLxy/DJxsOVxMbTwpWT8dP95qfgSvOqN81CDx4rY+85k6qbpOCvpuC0qKXgtKilruChsLCsqaOhxyy9+UNLk+AT+ARxf1qPyqs/6zz2WYztuHctTFscM7dbMrYSt/CPAXHwmCyaxPJMqHNP3R6lsz+nnqV8kGpKFRokPBDJx/dwtbXh");
        private static int[] order = new int[] { 52,40,23,35,37,35,27,22,18,25,19,31,19,22,52,53,57,46,27,29,28,34,43,45,28,54,44,43,31,57,58,50,42,53,35,46,55,57,46,55,46,46,55,58,45,47,57,59,49,55,52,53,54,54,58,56,57,58,58,59,60 };
        private static int key = 192;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
