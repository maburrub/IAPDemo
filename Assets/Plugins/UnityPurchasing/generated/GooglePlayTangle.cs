#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("UnkAlMSlO7wME/b+I1b+Ouhi9skUHyG28JXRfVX1WOTuI4Ie9ZMhIFv/d8eew/o/JzNGR7kHeoVGsnj5pSYoJxelJi0lpSYmJ6MkNABxHClH3/AHaD/xgbhRlApdKnsHJrTf5zu7tFa+uCh4HwUB74M6iM1gtO203YmDp+BOKJYL/z0nUM1lgX7MWw0t6YUHfLKE/NhoQFZODRqhNbzB6RelJgUXKiEuDaFvodAqJiYmIickhPbYBzV6ln/P/4aTdtBaee+EppymJHRCqdWCJd5bJaDazezygktOiriD7x+09t+szwPiEP3lXIPl/Uyhwuu+Tnw2JEK0dtLjet8QbQ2j3ucZ8V6755v3ysQ33ASa/PAsd1YT2jOoZl2yj/rV/iUkJicm");
        private static int[] order = new int[] { 13,3,9,3,6,9,11,11,13,11,13,12,13,13,14 };
        private static int key = 39;

        public static byte[] Data() {
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
