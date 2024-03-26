using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lance.Common
{
    public static partial class Util
    {
        #region helper

        /// <summary>
        /// safe convert the <paramref name="value"/> parameter in range [<paramref name="oldMin"/> to <paramref name="oldMax"/>] to new value in range [<paramref name="newMin"/> to <paramref name="newMax"/>].
        /// <paramref name="value"/> parameter can not out_of_range [<paramref name="oldMin"/> to <paramref name="oldMax"/>]
        /// <paramref name="oldMax"/> parameter must greater than <paramref name="oldMin"/> parameter
        /// <paramref name="newMax"/> parameter must greater than <paramref name="newMin"/> parameter
        /// </summary>
        /// <param name="oldMin">old min value</param>
        /// <param name="oldMax">old max value</param>
        /// <param name="value">value compare</param>
        /// <param name="newMin">new min value</param>
        /// <param name="newMax">new max value</param>
        /// <returns>new value in range [<paramref name="newMin"/> to <paramref name="newMax"/>]</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">out_of_range</exception>
        public static float ClampRemap(this float value, float oldMin, float oldMax, float newMin, float newMax)
        {
            if (value < oldMin || value > oldMax) throw new ArgumentOutOfRangeException($"value out of range [{oldMin}..{oldMax}]");
            if (oldMax <= oldMin) throw new ArgumentOutOfRangeException($"[{oldMin}..{oldMax}] range not correct!");
            if (newMax <= newMin) throw new ArgumentOutOfRangeException($"[{newMin}..{newMax}] range not correct!");

            return Remap(value,
                oldMin,
                oldMax,
                newMin,
                newMax);
        }

        /// <summary>
        /// convert the <paramref name="value"/> parameter in range [<paramref name="oldMin"/> to <paramref name="oldMax"/>] to new value in range [<paramref name="newMin"/> to <paramref name="newMax"/>].
        /// </summary>
        /// <param name="oldMin">old min value</param>
        /// <param name="oldMax">old max value</param>
        /// <param name="value">value compare</param>
        /// <param name="newMin">new min value</param>
        /// <param name="newMax">new max value</param>
        /// <returns>new value in range [<paramref name="newMin"/> to <paramref name="newMax"/>]</returns>
        public static float Remap(this float value, float oldMin, float oldMax, float newMin, float newMax)
        {
            return (value - oldMin) / (oldMax - oldMin) * (newMax - newMin) + newMin;
        }

        /// <summary>
        /// Indicates the random value in the <paramref name="collection"/>
        /// if <paramref name="collection"/> is empty return default vaule of T
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T PickRandom<T>(this T[] collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            return collection.Length == 0 ? default : collection[Rand.ThreadSafe.Next(0, collection.Length)];
        }

        /// <summary>
        /// Indicates the random value in the <paramref name="collection"/>
        /// if <paramref name="collection"/> is empty return default vaule of T
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static (T, int) PickRandomWithIndex<T>(this T[] collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            var index = Rand.ThreadSafe.Next(0, collection.Length);
            return collection.Length == 0 ? (default, -1) : (collection[index], index);
        }

        /// <summary>
        /// Indicates the random value in the <paramref name="collection"/>
        /// if <paramref name="collection"/> is empty return default vaule of T
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T PickRandom<T>(this IList<T> collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            return collection.Count == 0 ? default : collection[Rand.ThreadSafe.Next(0, collection.Count)];
        }
        
        /// <summary>
        /// Indicates the random value in the <paramref name="collection"/>
        /// if <paramref name="collection"/> is empty return default vaule of T
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static (T, int) PickRandomWithIndex<T>(this IList<T> collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            var index = Rand.ThreadSafe.Next(0, collection.Count);
            return collection.Count == 0 ? (default, -1) : (collection[index], index);
        }

        /// <summary>
        /// Indicates the random value in the <paramref name="collection"/> and also remove that element
        /// if <paramref name="collection"/> is empty return default vaule of T
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T PopRandom<T>(this IList<T> collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            if (collection.Count == 0)
                return default;
            var i = Rand.ThreadSafe.Next(0, collection.Count);
            var value = collection[i];
            collection.RemoveAt(i);
            return value;
        }
        
        /// <summary>
        /// Indicates the random value in the <paramref name="collection"/> and also remove that element and return index of element
        /// if <paramref name="collection"/> is empty return default vaule of T
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static (T, int) PopRandomWithIndex<T>(this IList<T> collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            if (collection.Count == 0)
                return default;
            var i = Rand.ThreadSafe.Next(0, collection.Count);
            var value = collection[i];
            collection.RemoveAt(i);
            return (value, i);
        }

        /// <summary>
        /// shuffle element in array <paramref name="source"/> parameter
        /// </summary>
        /// <param name="source">array</param>
        /// <typeparam name="T"></typeparam>
        public static void Shuffle<T>(this T[] source)
        {
            var n = source.Length;
            while (n > 1)
            {
                n--;
                var k = Rand.ThreadSafe.Next(n + 1);
                var value = source[k];
                source[k] = source[n];
                source[n] = value;
            }
        }

        /// <summary>
        /// shuffle element in <paramref name="source"/> parameter.
        /// </summary>
        /// <param name="source">IList</param>
        /// <typeparam name="T"></typeparam>
        public static void Shuffle<T>(this IList<T> source)
        {
            var n = source.Count;
            while (n > 1)
            {
                n--;
                var k = Rand.ThreadSafe.Next(n + 1);
                var value = source[k];
                source[k] = source[n];
                source[n] = value;
            }
        }

        /// <summary>
        /// shuffle element in dictionary <paramref name="source"/> parameter.
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <returns>new dictionary (shuffled)</returns>
        public static IDictionary<T1, T2> Shuffle<T1, T2>(this IDictionary<T1, T2> source)
        {
            var keys = source.Keys.ToArray();
            var values = source.Values.ToArray();

            var n = source.Count;
            while (n > 1)
            {
                n--;
                var k = Rand.ThreadSafe.Next(n + 1);
                var keyValue = keys[k];
                keys[k] = keys[n];
                keys[n] = keyValue;

                var value = values[k];
                values[k] = values[n];
                values[n] = value;
            }

            return MakeDictionary(keys, values);
        }

        /// <summary>
        /// sub array from <paramref name="source"/> parameter a <paramref name="count"/> elements starting at index <paramref name="start"/> parameter.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="source">source array datas</param>
        /// <param name="start">start index</param>
        /// <param name="count">number sub</param>
        /// <returns>sub array</returns>
        public static T[] Sub<T>(this T[] source, int start, int count)
        {
            var result = new T[count];
            for (var i = 0; i < count; i++)
            {
                result[i] = source[start + i];
            }

            return result;
        }

        /// <summary>
        /// Add element <paramref name="value"/> parameter with <paramref name="key"/> parameter in to <paramref name="dictionary"/> and indicate whether additional success or faild
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns>
        /// <see langword="true" /> if the <paramref name="key" /> parameter added success in to <paramref name="dictionary"/>; otherwise, <see langword="false" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> parameter is null</exception>
        public static bool Add<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));

            if (dictionary.ContainsKey(key)) return false;
            dictionary.Add(key, value);
            return true;
        }

        /// <summary>
        /// make dictionay from elements has <paramref name="values"/> parameter with <paramref name="keys"/> parameter
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="keys"/> parameter is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> parameter is null</exception>
        /// <exception cref="ArgumentException">Size <paramref name="keys"/> and size <paramref name="values"/> diffirent!</exception>
        public static IDictionary<TKey, TValue> MakeDictionary<TKey, TValue>(this TKey[] keys, TValue[] values)
        {
            if (keys == null) throw new ArgumentNullException(nameof(keys));
            if (values == null) throw new ArgumentNullException(nameof(values));

            if (keys.Length != values.Length)
            {
                throw new ArgumentException("Size keys and size values diffirent!");
            }

            IDictionary<TKey, TValue> result = new Dictionary<TKey, TValue>();
            for (var i = 0; i < keys.Length; i++)
            {
                result.Add(keys[i], values[i]);
            }

            return result;
        }

        /// <summary>
        /// make dictionay from elements has <paramref name="values"/> parameter with <paramref name="keys"/> parameter
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="keys"/> parameter is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> parameter is null</exception>
        /// <exception cref="ArgumentException">Size <paramref name="keys"/> and size <paramref name="values"/> diffirent!</exception>
        public static IDictionary<TKey, TValue> MakeDictionary<TKey, TValue>(this IList<TKey> keys, IList<TValue> values)
        {
            if (keys == null) throw new ArgumentNullException(nameof(keys));
            if (values == null) throw new ArgumentNullException(nameof(values));

            if (keys.Count != values.Count)
            {
                throw new ArgumentException("Size keys and size values diffirent!");
            }

            IDictionary<TKey, TValue> result = new Dictionary<TKey, TValue>();
            for (var i = 0; i < keys.Count; i++)
            {
                result.Add(keys[i], values[i]);
            }

            return result;
        }

        /// <summary>
        /// get or default same linq
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="defaultOverride"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TK"></typeparam>
        /// <returns></returns>
        public static TK GetOrDefault<T, TK>(this IDictionary<T, TK> dictionary, T key, TK defaultOverride = default)
        {
            return dictionary.TryGetValue(key, out var value) ? value : defaultOverride;
        }

        /// <summary>
        /// copy <paramref name="data"/> parameter data in to clipboard
        /// </summary>
        /// <param name="data">string</param>
        public static void CopyToClipboard(this string data)
        {
            var textEditor = new TextEditor {text = data};
            textEditor.SelectAll();
            textEditor.Copy();
        }

        /// <summary>
        /// Generates a random double. Values returned are from <paramref name="lowerBound"/> up to but not including <paramref name="upperBound"/>.
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="lowerBound"></param>
        /// <param name="upperBound"></param>
        /// <returns></returns>
        public static double Next(this Random rand, double lowerBound, double upperBound) { return rand.NextDouble() * (upperBound - lowerBound) + lowerBound; }

        /// <summary>
        /// Generates a random float. Values returned are from <paramref name="lowerBound"/> up to but not including <paramref name="upperBound"/>.
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="lowerBound"></param>
        /// <param name="upperBound"></param>
        /// <returns></returns>
        public static float Next(this Random rand, float lowerBound, float upperBound) { return (float) (rand.NextDouble() * (upperBound - lowerBound) + lowerBound); }

        /// <summary>
        /// formatting Big Numbers: The “aa” Notation
        ///
        /// number                alphabet
        /// 1                        1
        /// 1000                     1K
        /// 1000000                  1M
        /// 1000000000               1B
        /// 1000000000000            1T
        /// 1000000000000000         1AA
        ///
        /// source  : https://gram.gs/gramlog/formatting-big-numbers-aa-notation/
        /// </summary>
        /// <param name="number">BigInteger</param>
        /// <returns></returns>
        public static string ToAlphabet(this System.Numerics.BigInteger number)
        {
            var str = number.ToString();
            var len = str.Length;
            if (number.Sign < 0 && len <= 4 || number.Sign > 0 && len <= 3) return str;
            var stringBuilder = new System.Text.StringBuilder();
            var index = 0;
            if (number.Sign < 0)
            {
                stringBuilder.Append('-');
                len--;
                index = 1;
            }

            //{0, ""}, {1, "K"}, {2, "M"}, {3, "B"}, {4, "T"}
            var intPart = len % 3;
            if (intPart == 0) intPart = 3;
            intPart += index;
            intPart += 2; // for floating point
            if (intPart > len) intPart = len;

            var tempString = stringBuilder.ToString();

            stringBuilder.Clear();
            for (int i = index; i < intPart; i++)
            {
                stringBuilder.Append(str[i]);
            }

            var floating = double.Parse(stringBuilder.ToString());
            floating /= 100;
            stringBuilder.Clear();
            stringBuilder.Append(tempString).Append(floating);

            if (len > 15)
            {
                var n = (len - 16) / 3;
                var firstChar = (char) (65 + n / 26);
                var secondChar = (char) (65 + n % 26);
                stringBuilder.Append(firstChar);
                stringBuilder.Append(secondChar);
            }
            else if (len > 12) stringBuilder.Append('T');
            else if (len > 9) stringBuilder.Append('B');
            else if (len > 6) stringBuilder.Append('M');
            else if (len > 3) stringBuilder.Append('K');

            return stringBuilder.ToString();
        }

        /// <summary>
        /// formatting Big Numbers: The “aa” Notation
        ///
        /// number                alphabet
        /// 1                        1
        /// 1000                     1K
        /// 1000000                  1M
        /// 1000000000               1B
        /// 1000000000000            1T
        /// 1000000000000000         1AA
        ///
        /// </summary>
        /// <param name="value">string number</param>
        /// <returns></returns>
        public static string ToAlphabet(this string value)
        {
            value = value.Split('.')[0];
            var len = value.Length;
            var stringBuilder = new System.Text.StringBuilder();
            var index = 0;
            var num = 3;
            if (value[0] == '-')
            {
                stringBuilder.Append('-');
                len--;
                index = 1;
                num = 4;
            }

            if (len <= num) return value; // return here if not converted to alphabet

            //{0, ""}, {1, "K"}, {2, "M"}, {3, "B"}, {4, "T"}
            var intPart = len % 3;
            if (intPart == 0) intPart = 3;
            intPart += index;
            intPart += 2; // for floating point
            if (intPart > len) intPart = len;

            var tempString = stringBuilder.ToString();

            stringBuilder.Clear();
            for (int i = index; i < intPart; i++)
            {
                stringBuilder.Append(value[i]);
            }

            var floating = double.Parse(stringBuilder.ToString());
            floating /= 100;
            stringBuilder.Clear();
            stringBuilder.Append(tempString).Append(floating);

            if (len > 15)
            {
                var n = (len - 16) / 3;
                var firstChar = (char) (65 + n / 26);
                var secondChar = (char) (65 + n % 26);
                stringBuilder.Append(firstChar);
                stringBuilder.Append(secondChar);
            }
            else if (len > 12) stringBuilder.Append('T');
            else if (len > 9) stringBuilder.Append('B');
            else if (len > 6) stringBuilder.Append('M');
            else stringBuilder.Append('K');

            return stringBuilder.ToString();
        }

        /// <summary>This method gives you the time-independent 't' value for lerp when used for dampening. This returns 1 in edit mode, or if dampening is less than 0.</summary>
        public static float DampenFactor(float dampening, float elapsed)
        {
            if (dampening < 0.0f)
            {
                return 1.0f;
            }
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                return 1.0f;
            }
#endif
            return 1.0f - Mathf.Exp(-dampening * elapsed);
        }

        #endregion
    }
}