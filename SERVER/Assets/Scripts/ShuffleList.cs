
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

static class ShuffleList 
{
   
        private static System.Random rng = new System.Random();
  
    public static void ShuffleCrypto<T>(this IList<T> list)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void ShuffleYates<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static void ShuffleTravis<T>(this List<T> list)

        {

            for (int i = list.Count - 1; i > 0; i--)

            {

                int r = UnityEngine.Random.Range(0, i);

                T temp = list[i];

                list[i] = list[r];

                list[r] = temp;

            }

        }

    public static void Shuffle<T>(this T[] arr)

    {

        for (int i = arr.Length - 1; i > 0; i--)

        {

            int r = UnityEngine.Random.Range(0, i);

            T temp = arr[i];

            arr[i] = arr[r];

            arr[r] = temp;

        }

    }
    public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
    {
        for (var i = 0; i < (float)array.Length / size; i++)
        {
            yield return array.Skip(i * size).Take(size);
        }
    }

}
