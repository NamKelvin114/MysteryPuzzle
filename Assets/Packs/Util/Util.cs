using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Worldreaver.Utility
{
    /// <summary>
    /// utility
    /// </summary>
    public static partial class Util
    {
        #region Support

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
        public static float ClampRemap(this float value,
            float oldMin,
            float oldMax,
            float newMin,
            float newMax)
        {
            if (value < oldMin || value > oldMax)
            {
                throw Error.ArgumentOutOfRange($"value out of range [{oldMin}..{oldMax}]");
            }

            if (oldMax <= oldMin)
            {
                throw Error.ArgumentOutOfRange("[oldMin..oldMax] range not correct!");
            }

            if (newMax <= newMin)
            {
                throw Error.ArgumentOutOfRange("[newMin..newMax] range not correct!");
            }

            return Remap(value, oldMin, oldMax, newMin, newMax);
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
        public static float Remap(this float value,
            float oldMin,
            float oldMax,
            float newMin,
            float newMax)
        {
            return (value - oldMin) / (oldMax - oldMin) * (newMax - newMin) + newMin;
        }


        /// <summary>
        /// sub array from <paramref name="source"/> parameter a <paramref name="count"/> elements starting at index <paramref name="start"/> parameter.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="source">source array datas</param>
        /// <param name="start">start index</param>
        /// <param name="count">number sub</param>
        /// <returns>sub array</returns>
        public static T[] Sub<T>(this T[] source,
            int start,
            int count)
        {
            var result = new T[count];
            for (var i = 0; i < count; i++)
            {
                result[i] = source[start + i];
            }

            return result;
        }

        #region -- calculate --------------------

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

            if (len <= num) return value;

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
            else if (len > 3) stringBuilder.Append('K');

            return stringBuilder.ToString();
        }

        #endregion------------------------------------------------------------\\

        #endregion

        #region Collection

        /// <summary>
        /// safe add <paramref name="value"/> parameter in to <paramref name="collection"/>.
        /// </summary>
        /// <param name="collection">collection</param>
        /// <param name="value">element add to collection</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        /// <see langword="true" /> if the <paramref name="value" /> parameter is added success in to <paramref name="collection"/>; otherwise, <see langword="false" />.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException"><param name="collection"> parameter is null</param></exception>
        public static bool Add<T>(this IList<T> collection,
            T value)
        {
            if (collection == null)
            {
                throw Error.ArgumentNull("collection");
            }

            if (collection.Exists(value)) return false;
            collection.Add(value);
            return true;
        }

        /// <summary>
        /// Indicates whether the specified <paramref name="collection"/> parameter is <see langword="null" /> or an empty array ([]).
        /// </summary>
        /// <param name="collection">array</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        /// <see langword="true" /> if the <paramref name="collection" /> parameter is <see langword="null" /> or an empty array ([]); otherwise, <see langword="false" />.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this T[] collection)
        {
            if (collection == null)
            {
                return true;
            }

            return collection.Length == 0;
        }

        /// <summary>
        /// Indicates whether the specified <paramref name="collection"/> parameter is <see langword="null" /> or an empty ICollection (List, IEnumerable,...).
        /// </summary>
        /// <param name="collection">ICollection</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        /// <see langword="true" /> if the <paramref name="collection" /> parameter is <see langword="null" /> or an empty collection; otherwise, <see langword="false" />.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            if (collection == null)
            {
                return true;
            }

            return collection.Count == 0;
        }

        /// <summary>
        /// Indicates the relationship between <paramref name="x"/> parameter and <paramref name="y"/> parameter.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        /// <see langword="true" /> if the <paramref name="x" /> parameter is == <paramref name="y"/>; otherwise, <see langword="false" />.
        /// </returns>
        public static bool Equals<T>(T x,
            T y)
        {
            return EqualityComparer<T>.Default.Equals(x, y);
        }

        /// <summary>
        /// Indicates the relationship between <paramref name="x"/> parameter and <paramref name="y"/> parameter.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        /// 0 if the <paramref name="x" /> parameter == <paramref name="y"/> parameter.
        /// less than 0 if the <paramref name="x" /> parameter less than <paramref name="y"/> parameter.
        /// geater than 0 if the <paramref name="x" /> parameter geater than <paramref name="y"/> parameter.
        /// </returns>
        public static int Compare<T>(T x,
            T y)
        {
            return Comparer<T>.Default.Compare(x, y);
        }

        /// <summary>
        /// Indicates whether the <paramref name="value"/> parameter exists in the <paramref name="collection"/>.
        /// (Faster using default Contains)
        /// </summary>
        /// <param name="collection">array</param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <see langword="true" /> if the <paramref name="collection" /> parameter is contains <paramref name="value"/>; otherwise, <see langword="false" />.
        /// <exception cref="T:System.ArgumentNullException"><paramref name="collection"/> parameter is null</exception>
        public static bool Exists<T>(this T[] collection,
            T value)
        {
            if (collection == null)
            {
                throw Error.ArgumentNull("collection");
            }

            var length = collection.Length;
            if (length == 0)
            {
                return false;
            }

            for (var i = 0; i < length; i++)
            {
                if (Equals(collection[i], value)) return true;
            }

            return false;
        }

        /// <summary>
        /// Indicates whether the <paramref name="value"/> parameter exists in the <paramref name="collection"/>.
        /// (same using Contains, in case Contains faster than for loop iterator)
        /// </summary>
        /// <param name="collection">IList</param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <see langword="true" /> if the <paramref name="collection" /> parameter is contains <paramref name="value"/>; otherwise, <see langword="false" />.
        /// <exception cref="T:System.ArgumentNullException"><paramref name="collection"/> parameter is null</exception>
        public static bool Exists<T>(this IList<T> collection,
            T value)
        {
            if (collection == null)
            {
                throw Error.ArgumentNull("collection");
            }

            return collection.Contains(value);
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
            if (collection == null)
            {
                throw Error.ArgumentNull("collection");
            }

            return collection.Length == 0 ? default : collection[RandomInstance.This.Next(0, collection.Length)];
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
            if (collection == null)
            {
                throw Error.ArgumentNull("collection");
            }

            return collection.Count == 0 ? default : collection[RandomInstance.This.Next(0, collection.Count)];
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
            if (collection == null)
            {
                throw Error.ArgumentNull("collection");
            }

            if (collection.Count == 0)
                return default;
            var i = RandomInstance.This.Next(0, collection.Count);
            var value = collection[i];
            collection.RemoveAt(i);
            return value;
        }

        /// <summary>
        /// Indicates the largest value in the <paramref name="collection"/> parameter with condition <paramref name="selector"/> parameter
        /// ( faster than linq .Select(_=>_.something).Max() and .Max(_=>_something) and .OrderByDescending(_ => _.something).First())
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="selector">condition to query in collection</param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TU">expression selector</typeparam>
        /// <returns>T have max value follow expression selector</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="collection"/> parameter is null</exception>
        /// <exception cref="InvalidOperationException">sequence empty</exception>
        public static T MaxItem<T, TU>(this T[] collection,
            Func<T, TU> selector) where TU : IComparable<TU>
        {
            if (collection == null)
            {
                throw Error.ArgumentNull("collection");
            }

            var first = true;
            var max = default(T);
            var maxKey = default(TU);
            var length = collection.Length;
            for (var i = 0; i < length; i++)
            {
                if (first)
                {
                    max = collection[i];
                    maxKey = selector(max);
                    first = false;
                }
                else
                {
                    var currentKey = selector(collection[i]);
                    if (currentKey.CompareTo(maxKey) <= 0) continue;
                    maxKey = currentKey;
                    max = collection[i];
                }
            }

            if (first) throw new InvalidOperationException("Sequence is empty.");
            return max;
        }

        /// <summary>
        /// Indicates the largest value in the <paramref name="collection"/> parameter with condition <paramref name="selector"/> parameter
        /// ( faster than linq .Select(_=>_.something).Max() and .Max(_=>_something) and .OrderByDescending(_ => _.something).First())
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="selector">condition to query in collection</param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TU">expression selector</typeparam>
        /// <returns>T have max value follow expression selector</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="collection"/> parameter is null</exception>
        /// <exception cref="InvalidOperationException">sequence empty</exception>
        public static T MaxItem<T, TU>(this IList<T> collection,
            Func<T, TU> selector) where TU : IComparable<TU>
        {
            if (collection == null)
            {
                throw Error.ArgumentNull("collection");
            }

            var first = true;
            var max = default(T);
            var maxKey = default(TU);
            var length = collection.Count;
            for (var i = 0; i < length; i++)
            {
                if (first)
                {
                    max = collection[i];
                    maxKey = selector(max);
                    first = false;
                }
                else
                {
                    var currentKey = selector(collection[i]);
                    if (currentKey.CompareTo(maxKey) <= 0) continue;
                    maxKey = currentKey;
                    max = collection[i];
                }
            }

            if (first) throw new InvalidOperationException("Sequence is empty.");
            return max;
        }

        /// <summary>
        /// Indicates the smallest value in the <paramref name="collection"/> parameter with condition <paramref name="selector"/> parameter
        /// ( faster than linq .Select(_=>_.something).Min() and .Min(_=>_something) and .OrderBy(_ => _.something).First() )
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="selector">condition to query in collection</param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TU">expression selector</typeparam>
        /// <returns>T have min value follow expression selector</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="collection"/> parameter is null</exception>
        /// <exception cref="InvalidOperationException">sequence empty</exception>
        public static T MinItem<T, TU>(this T[] collection,
            Func<T, TU> selector) where TU : IComparable<TU>
        {
            if (collection == null)
            {
                throw Error.ArgumentNull("collection");
            }

            var first = true;
            var min = default(T);
            var minKey = default(TU);
            var length = collection.Length;
            for (var i = 0; i < length; i++)
            {
                if (first)
                {
                    min = collection[i];
                    minKey = selector(min);
                    first = false;
                }
                else
                {
                    var currentKey = selector(collection[i]);
                    if (currentKey.CompareTo(minKey) >= 0) continue;
                    minKey = currentKey;
                    min = collection[i];
                }
            }

            if (first) throw new InvalidOperationException("Sequence is empty.");
            return min;
        }

        /// <summary>
        /// Indicates the smallest value in the <paramref name="collection"/> parameter with condition <paramref name="selector"/> parameter
        /// ( faster than linq .Select(_=>_.something).Min() and .Min(_=>_something) and .OrderBy(_ => _.something).First() )
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="selector">condition to query in collection</param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TU">expression selector</typeparam>
        /// <returns>T have min value follow expression selector</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="collection"/> parameter is null</exception>
        /// <exception cref="InvalidOperationException">sequence empty</exception>
        public static T MinItem<T, TU>(this IList<T> collection,
            Func<T, TU> selector) where TU : IComparable<TU>
        {
            if (collection == null)
            {
                throw Error.ArgumentNull("collection");
            }

            var first = true;
            var min = default(T);
            var minKey = default(TU);
            var length = collection.Count;
            for (var i = 0; i < length; i++)
            {
                if (first)
                {
                    min = collection[i];
                    minKey = selector(min);
                    first = false;
                }
                else
                {
                    var currentKey = selector(collection[i]);
                    if (currentKey.CompareTo(minKey) >= 0) continue;
                    minKey = currentKey;
                    min = collection[i];
                }
            }

            if (first) throw new InvalidOperationException("Sequence is empty.");
            return min;
        }

        /// <summary>
        ///  Indicates the largest value in the <paramref name="collection"/> parameter with its index
        /// </summary>
        /// <param name="collection">Array</param>
        /// <typeparam name="T">primitive type</typeparam>
        /// <returns>(index, max value)</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> parameter is null</exception>
        public static (int, T) MaxIndex<T>(this T[] collection) where T : IComparable<T>
        {
            if (collection == null)
            {
                throw Error.ArgumentNull("collection");
            }

            var result = collection[0];
            var index = 0;

            for (var i = 1; i < collection.Length; i++)
            {
                if (Compare(result, collection[i]) >= 0) continue;
                index = i;
                result = collection[i];
            }

            return (index, result);
        }

        /// <summary>
        ///  Indicates the largest value in the <paramref name="collection"/> parameter with its index
        /// </summary>
        /// <param name="collection">IList</param>
        /// <typeparam name="T">primitive type</typeparam>
        /// <returns>(index, max value)</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static (int, T) MaxIndex<T>(this IList<T> collection) where T : IComparable<T>
        {
            if (collection == null)
            {
                throw Error.ArgumentNull("collection");
            }

            var result = collection[0];
            var index = 0;
            for (var i = 1; i < collection.Count; i++)
            {
                if (Compare(result, collection[i]) >= 0) continue;
                index = i;
                result = collection[i];
            }

            return (index, result);
        }

        /// <summary>
        ///  Indicates the smallest value in the <paramref name="collection"/> parameter with its index
        /// </summary>
        /// <param name="collection">Array</param>
        /// <typeparam name="T">primitive type</typeparam>
        /// <returns>(index, min value)</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> parameter is null</exception>
        public static (int, T) MinIndex<T>(this T[] collection) where T : IComparable<T>
        {
            if (collection == null)
            {
                throw Error.ArgumentNull("collection");
            }

            var result = collection[0];
            var index = 0;
            for (var i = 1; i < collection.Length; i++)
            {
                if (Compare(result, collection[i]) <= 0) continue;
                index = i;
                result = collection[i];
            }

            return (index, result);
        }

        /// <summary>
        ///  Indicates the smallest value in the <paramref name="collection"/> parameter with its index
        /// </summary>
        /// <param name="collection">IList</param>
        /// <typeparam name="T">primitive type</typeparam>
        /// <returns>(index, min value)</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> parameter is null</exception>
        public static (int, T) MinIndex<T>(this IList<T> collection) where T : IComparable<T>
        {
            if (collection == null)
            {
                throw Error.ArgumentNull("collection");
            }

            var result = collection[0];
            var index = 0;
            for (var i = 1; i < collection.Count; i++)
            {
                if (Compare(result, collection[i]) <= 0) continue;
                index = i;
                result = collection[i];
            }

            return (index, result);
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
                var k = RandomInstance.This.Next(n + 1);
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
                var k = RandomInstance.This.Next(n + 1);
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
                var k = RandomInstance.This.Next(n + 1);
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
        public static bool Add<TKey, TValue>(IDictionary<TKey, TValue> dictionary,
            TKey key,
            TValue value)
        {
            if (dictionary == null)
            {
                throw Error.ArgumentNull("dictionary");
            }

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
        public static IDictionary<TKey, TValue> MakeDictionary<TKey, TValue>(this TKey[] keys,
            TValue[] values)
        {
            if (keys == null)
            {
                throw Error.ArgumentNull("keys");
            }

            if (values == null)
            {
                throw Error.ArgumentNull("values");
            }

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
        public static IDictionary<TKey, TValue> MakeDictionary<TKey, TValue>(this IList<TKey> keys,
            IList<TValue> values)
        {
            if (keys == null)
            {
                throw Error.ArgumentNull("keys");
            }

            if (values == null)
            {
                throw Error.ArgumentNull("values");
            }

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

        #endregion

        #region Transform

        /// <summary>
        /// Convert <paramref name="position"/> parameter in world to position in canvas <paramref name="canvasRectTransform"/> parameter.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="canvasRectTransform"></param>
        /// <returns>Vector2</returns>
        public static Vector2 ToCanvasPosition(this Vector3 position,
            RectTransform canvasRectTransform)
        {
            if (Camera.main == null)
            {
                return Vector2.zero;
            }

            var viewport = Camera.main.WorldToViewportPoint(position);
            var sizeDelta = canvasRectTransform.sizeDelta;
            return new Vector3(viewport.x * sizeDelta.x - sizeDelta.x / 2, viewport.y * sizeDelta.y - sizeDelta.y / 2, viewport.z);
        }

        /// <summary>
        /// Convert <paramref name="position"/> parameter in canvas <paramref name="canvasRectTransform"/> parameter to position in world
        /// </summary>
        /// <param name="position"></param>
        /// <param name="canvasRectTransform"></param>
        /// <returns></returns>
        public static Vector2 ToWorldPosition(this Vector3 position,
            RectTransform canvasRectTransform)
        {
            if (Camera.main == null)
            {
                return Vector2.zero;
            }

            var sizeDelta = canvasRectTransform.sizeDelta;
            return Camera.main.ViewportToWorldPoint(new Vector3((position.x + sizeDelta.x / 2) / sizeDelta.x, (position.y + sizeDelta.y / 2) / sizeDelta.y));
        }

        public static Vector2 ConvertRectTransform(this RectTransform target)
        {
            var rect = target.rect;
            var pivot = target.pivot;
            var fromPivotDerivedOffset = new Vector2(rect.width * pivot.x + rect.xMin, rect.height * pivot.y + rect.yMin);
            return RectTransformUtility.WorldToScreenPoint(null, target.position) + fromPivotDerivedOffset;
        }

        /// <summary>
        /// Converts the anchoredPosition of the <paramref name="from"/> to the <paramref name="to"/>,
        /// taking into consideration offset, anchors and pivot, and returns the new anchoredPosition
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>to.anchoredPosition + localPoint - pivotDerivedOffset</returns>
        public static Vector2 SwitchToRectTransform(this RectTransform from,
            RectTransform to)
        {
            var screenP = from.ConvertRectTransform();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenP, null, out var localPoint);
            var rect = to.rect;
            var pivot = to.pivot;
            var pivotDerivedOffset = new Vector2(rect.width * pivot.x + rect.xMin, rect.height * pivot.y + rect.yMin);
            return to.anchoredPosition + localPoint - pivotDerivedOffset;
        }

        /// <summary>
        /// Converts the anchoredPosition of the <paramref name="from"/> to the <paramref name="to"/>,
        /// taking into consideration offset, anchors and pivot, and returns the new anchoredPosition
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>'localPoint - pivotDerivedOffset'</returns>
        public static Vector2 SwitchToRectTransform2(this RectTransform from,
            RectTransform to)
        {
            var screenP = from.ConvertRectTransform();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenP, null, out var localPoint);
            var rect = to.rect;
            var pivot = to.pivot;
            var pivotDerivedOffset = new Vector2(rect.width * pivot.x + rect.xMin, rect.height * pivot.y + rect.yMin);
            return localPoint - pivotDerivedOffset;
        }

        /// <summary>
        /// set <paramref name="pivot"/> parameter for <paramref name="rectTransform"/>
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="pivot"></param>
        public static void SetPivot(RectTransform rectTransform,
            Vector2 pivot)
        {
            if (rectTransform == null) return;

            var size = rectTransform.rect.size;
            var deltaPivot = rectTransform.pivot - pivot;
            var deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
            rectTransform.pivot = pivot;
            rectTransform.localPosition -= deltaPosition;
        }

        /// <summary>
        /// Sets the x/y/z transform.position using optional parameters, keeping all undefined values as they were before. Can be
        /// called with named parameters like transform.SetPosition(x: 5, z: 10), for example, only changing transform.position.x and z.
        /// </summary>
        /// <param name="transform">The transform to set the transform.position at.</param>
        /// <param name="x">If this is not null, transform.position.x is set to this value.</param>
        /// <param name="y">If this is not null, transform.position.y is set to this value.</param>
        /// <param name="z">If this is not null, transform.position.z is set to this value.</param>
        /// <returns>The transform itself.</returns>
        public static Transform SetPosition(this Transform transform,
            float? x = null,
            float? y = null,
            float? z = null)
        {
            transform.position = transform.position.Change(x, y, z);
            return transform;
        }

        /// <summary>
        /// Sets the x/y/z transform.localPosition using optional parameters, keeping all undefined values as they were before. Can be
        /// called with named parameters like transform.SetLocalPosition(x: 5, z: 10), for example, only changing transform.localPosition.x and z.
        /// </summary>
        /// <param name="transform">The transform to set the transform.localPosition at.</param>
        /// <param name="x">If this is not null, transform.localPosition.x is set to this value.</param>
        /// <param name="y">If this is not null, transform.localPosition.y is set to this value.</param>
        /// <param name="z">If this is not null, transform.localPosition.z is set to this value.</param>
        /// <returns>The transform itself.</returns>
        public static Transform SetLocalPosition(this Transform transform,
            float? x = null,
            float? y = null,
            float? z = null)
        {
            transform.localPosition = transform.localPosition.Change(x, y, z);
            return transform;
        }

        /// <summary>
        /// Sets the x/y/z transform.localScale using optional parameters, keeping all undefined values as they were before. Can be
        /// called with named parameters like transform.SetLocalScale(x: 5, z: 10), for example, only changing transform.localScale.x and z.
        /// </summary>
        /// <param name="transform">The transform to set the transform.localScale at.</param>
        /// <param name="x">If this is not null, transform.localScale.x is set to this value.</param>
        /// <param name="y">If this is not null, transform.localScale.y is set to this value.</param>
        /// <param name="z">If this is not null, transform.localScale.z is set to this value.</param>
        /// <returns>The transform itself.</returns>
        public static Transform SetLocalScale(this Transform transform,
            float? x = null,
            float? y = null,
            float? z = null)
        {
            transform.localScale = transform.localScale.Change(x, y, z);
            return transform;
        }

        /// <summary>
        /// Sets the x/y/z transform.lossyScale using optional parameters, keeping all undefined values as they were before. Can be
        /// called with named parameters like transform.SetLossyScale(x: 5, z: 10), for example, only changing transform.lossyScale.x and z.
        /// </summary>
        /// <param name="transform">The transform to set the transform.lossyScale at.</param>
        /// <param name="x">If this is not null, transform.lossyScale.x is set to this value.</param>
        /// <param name="y">If this is not null, transform.lossyScale.y is set to this value.</param>
        /// <param name="z">If this is not null, transform.lossyScale.z is set to this value.</param>
        /// <returns>The transform itself.</returns>
        public static Transform SetLossyScale(this Transform transform,
            float? x = null,
            float? y = null,
            float? z = null)
        {
            var lossyScale = transform.lossyScale.Change(x, y, z);

            transform.localScale = Vector3.one;
            // ReSharper disable once Unity.InefficientPropertyAccess
            transform.localScale = new Vector3(lossyScale.x / transform.lossyScale.x,
                // ReSharper disable once Unity.InefficientPropertyAccess
                lossyScale.y / transform.lossyScale.y,
                // ReSharper disable once Unity.InefficientPropertyAccess
                lossyScale.z / transform.lossyScale.z);

            return transform;
        }

        /// <summary>
        /// Sets the x/y/z transform.eulerAngles using optional parameters, keeping all undefined values as they were before. Can be
        /// called with named parameters like transform.SetEulerAngles(x: 5, z: 10), for example, only changing transform.eulerAngles.x and z.
        /// </summary>
        /// <param name="transform">The transform to set the transform.eulerAngles at.</param>
        /// <param name="x">If this is not null, transform.eulerAngles.x is set to this value.</param>
        /// <param name="y">If this is not null, transform.eulerAngles.y is set to this value.</param>
        /// <param name="z">If this is not null, transform.eulerAngles.z is set to this value.</param>
        /// <returns>The transform itself.</returns>
        public static Transform SetEulerAngles(this Transform transform,
            float? x = null,
            float? y = null,
            float? z = null)
        {
            transform.eulerAngles = transform.eulerAngles.Change(x, y, z);
            return transform;
        }

        /// <summary>
        /// Sets the x/y/z transform.localEulerAngles using optional parameters, keeping all undefined values as they were before. Can be
        /// called with named parameters like transform.SetLocalEulerAngles(x: 5, z: 10), for example, only changing transform.localEulerAngles.x and z.
        /// </summary>
        /// <param name="transform">The transform to set the transform.localEulerAngles at.</param>
        /// <param name="x">If this is not null, transform.localEulerAngles.x is set to this value.</param>
        /// <param name="y">If this is not null, transform.localEulerAngles.y is set to this value.</param>
        /// <param name="z">If this is not null, transform.localEulerAngles.z is set to this value.</param>
        /// <returns>The transform itself.</returns>
        public static Transform SetLocalEulerAngles(this Transform transform,
            float? x = null,
            float? y = null,
            float? z = null)
        {
            transform.localEulerAngles = transform.localEulerAngles.Change(x, y, z);
            return transform;
        }

        #endregion

        #region Number

        /// <summary>
        /// Compare with 0
        /// </summary>
        public static bool IsZero(this float value)
        {
            return Math.Abs(value) < Mathf.Epsilon;
        }

        /// <summary>
        /// Compare with 0
        /// </summary>
        public static bool IsZero(this float val,
            float epsilon)
        {
            return Math.Abs(val) < epsilon;
        }

        /// <summary>
        /// Compare with 0
        /// </summary>
        public static bool IsZero(this double value)
        {
            return Math.Abs(value) < double.Epsilon;
        }

        /// <summary>
        /// Check for equivalence
        /// </summary>
        public static bool Equal(this float valA,
            float valB)
        {
            return Math.Abs(valA - valB) < Mathf.Epsilon;
        }

        #endregion

        #region Rect

        /// <summary>
        /// Extends/shrinks the rect by extendDistance to each side and gets a random position from the resulting rect.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <param name="extendDistance">The distance to extend/shrink the rect to each side.</param>
        /// <returns>A random position inside the extended rect.</returns>
        public static Vector2 RandomPosition(this Rect rect,
            float extendDistance = 0f)
        {
            var xMax = rect.xMax + extendDistance;
            var xMin = rect.xMin - extendDistance;

            var yMax = rect.yMax + extendDistance;
            var yMin = rect.yMin - extendDistance;

            return new Vector2(RandomInstance.This.NextDouble(xMin, xMax), RandomInstance.This.NextDouble(yMin, yMax));
        }

        /// <summary>
        /// Gets a random subrect of the given width or height inside this rect.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <param name="width">The target width of the subrect. Clamped to the width of the given rect.</param>
        /// <param name="height">The target height of the subrect. Clamped to the height of the given rect.</param>
        /// <returns>A random subrect with the given width and height.</returns>
        public static Rect RandomSubRect(this Rect rect,
            float width,
            float height)
        {
            width = Mathf.Min(rect.width, width);
            height = Mathf.Min(rect.height, height);

            var halfWidth = width / 2f;
            var halfHeight = height / 2f;

            var centerX = RandomInstance.This.NextDouble(rect.xMin + halfWidth, rect.xMax - halfWidth);
            var centerY = RandomInstance.This.NextDouble(rect.yMin + halfHeight, rect.yMax - halfHeight);

            return new Rect(centerX - halfWidth, centerY - halfHeight, width, height);
        }

        /// <summary>
        /// Extends/shrinks the rect by extendDistance to each side and then restricts the given vector to the resulting rect.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <param name="position">A position that should be restricted to the rect.</param>
        /// <param name="extendDistance">The distance to extend/shrink the rect to each side.</param>
        /// <returns>The vector, clamped to the Rect.</returns>
        public static Vector2 Clamp2(this Rect rect,
            Vector2 position,
            float extendDistance = 0f)
        {
            return new Vector2(Mathf.Clamp(position.x, rect.xMin - extendDistance, rect.xMax + extendDistance), Mathf.Clamp(position.y, rect.yMin - extendDistance, rect.yMax + extendDistance));
        }

        /// <summary>
        /// Extends/shrinks the rect by extendDistance to each side and then restricts the given vector to the resulting rect.
        /// The z component is kept.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <param name="position">A position that should be restricted to the rect.</param>
        /// <param name="extendDistance">The distance to extend/shrink the rect to each side.</param>
        /// <returns>The vector, clamped to the Rect.</returns>
        public static Vector3 Clamp3(this Rect rect,
            Vector3 position,
            float extendDistance = 0f)
        {
            return new Vector3(Mathf.Clamp(position.x, rect.xMin - extendDistance, rect.xMax + extendDistance), Mathf.Clamp(position.y, rect.yMin - extendDistance, rect.yMax + extendDistance),
                position.z);
        }

        /// <summary>
        /// Extends/shrinks the rect by extendDistance to each side.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <param name="extendDistance">The distance to extend/shrink the rect to each side.</param>
        /// <returns>The rect, extended/shrunken by extendDistance to each side.</returns>
        public static Rect Extend(this Rect rect,
            float extendDistance)
        {
            var copy = rect;
            copy.xMin -= extendDistance;
            copy.xMax += extendDistance;
            copy.yMin -= extendDistance;
            copy.yMax += extendDistance;
            return copy;
        }

        /// <summary>
        /// Extends/shrinks the rect by extendDistance to each side and then checks if a given point is inside the resulting rect.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <param name="position">A position that should be restricted to the rect.</param>
        /// <param name="extendDistance">The distance to extend/shrink the rect to each side.</param>
        /// <returns>True if the position is inside the extended rect.</returns>
        public static bool Contains(this Rect rect,
            Vector2 position,
            float extendDistance)
        {
            return (position.x > rect.xMin + extendDistance) && (position.y > rect.yMin + extendDistance) && (position.x < rect.xMax - extendDistance) && (position.y < rect.yMax - extendDistance);
        }

        /// <summary>
        /// Creates an array containing the four corner points of a Rect.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <returns>An array containing the four corner points of the Rect.</returns>
        public static Vector2[] GetCornerPoints(this Rect rect)
        {
            return new[]
            {
                new Vector2(rect.xMin, rect.yMin),
                new Vector2(rect.xMax, rect.yMin),
                new Vector2(rect.xMax, rect.yMax),
                new Vector2(rect.xMin, rect.yMax)
            };
        }

        #endregion

        #region Others

        /// <summary>
        /// Indicates file with <paramref name="name"/> file has exist in <paramref name="path"/>
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns>
        /// <see langword="true" /> if the <paramref name="path" /> parameter is contains file with <paramref name="name"/>; otherwise, <see langword="false" />.
        /// </returns>
        public static bool ExistsFile(string path,
            string name)
        {
            return System.IO.File.Exists($"{path}/{name}.wr");
        }

        /// <summary>
        /// remove file with <paramref name="name"/> follow <paramref name="path"/>
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        public static void RemoveFile(string path,
            string name)
        {
            if (ExistsFile(path, name))
            {
                System.IO.File.Delete($"{path}/{name}.wr");
            }
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
        /// Makes a copy of the vector with a changed alpha value.
        /// </summary>
        /// <param name="color">The Color to copy.</param>
        /// <param name="a">The new a component.</param>
        /// <returns>A copy of the Color with a changed alpha.</returns>
        public static Color ChangeAlpha(this Color color,
            float a)
        {
            color.a = a;
            return color;
        }

        /// <summary>
        /// Makes a copy of the Vector2 with changed x/y values, keeping all undefined values as they were before. Can be
        /// called with named parameters like vector.Change2(y: 5), for example, only changing the y component.
        /// </summary>
        /// <param name="vector">The Vector2 to be copied with changed values.</param>
        /// <param name="x">If this is not null, the x component is set to this value.</param>
        /// <param name="y">If this is not null, the y component is set to this value.</param>
        /// <returns>A copy of the Vector2 with changed values.</returns>
        public static Vector2 Change(this Vector2 vector,
            float? x = null,
            float? y = null)
        {
            if (x.HasValue) vector.x = x.Value;
            if (y.HasValue) vector.y = y.Value;
            return vector;
        }

        /// <summary>
        /// Makes a copy of the Vector3 with changed x/y/z values, keeping all undefined values as they were before. Can be
        /// called with named parameters like vector.Change3(x: 5, z: 10), for example, only changing the x and z components.
        /// </summary>
        /// <param name="vector">The Vector3 to be copied with changed values.</param>
        /// <param name="x">If this is not null, the x component is set to this value.</param>
        /// <param name="y">If this is not null, the y component is set to this value.</param>
        /// <param name="z">If this is not null, the z component is set to this value.</param>
        /// <returns>A copy of the Vector3 with changed values.</returns>
        public static Vector3 Change(this Vector3 vector,
            float? x = null,
            float? y = null,
            float? z = null)
        {
            if (x.HasValue) vector.x = x.Value;
            if (y.HasValue) vector.y = y.Value;
            if (z.HasValue) vector.z = z.Value;
            return vector;
        }

        /// <summary>
        /// Makes a copy of the Vector4 with changed x/y/z/w values, keeping all undefined values as they were before. Can be
        /// called with named parameters like vector.Change4(x: 5, z: 10), for example, only changing the x and z components.
        /// </summary>
        /// <param name="vector">The Vector4 to be copied with changed values.</param>
        /// <param name="x">If this is not null, the x component is set to this value.</param>
        /// <param name="y">If this is not null, the y component is set to this value.</param>
        /// <param name="z">If this is not null, the z component is set to this value.</param>
        /// <param name="w">If this is not null, the w component is set to this value.</param>
        /// <returns>A copy of the Vector4 with changed values.</returns>
        public static Vector4 Change(this Vector4 vector,
            float? x = null,
            float? y = null,
            float? z = null,
            float? w = null)
        {
            if (x.HasValue) vector.x = x.Value;
            if (y.HasValue) vector.y = y.Value;
            if (z.HasValue) vector.z = z.Value;
            if (w.HasValue) vector.w = w.Value;
            return vector;
        }


        /// <summary>
        /// Rotates a Vector2.
        /// </summary>
        /// <param name="v">The Vector2 to rotate.</param>
        /// <param name="angleRad">How far to rotate the Vector2 in radians.</param>
        /// <returns>The rotated Vector2.</returns>
        public static Vector2 RotateRad(this Vector2 v,
            float angleRad)
        {
            // http://answers.unity3d.com/questions/661383/whats-the-most-efficient-way-to-rotate-a-vector2-o.html
            var sin = Mathf.Sin(angleRad);
            var cos = Mathf.Cos(angleRad);

            var tx = v.x;
            var ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);

            return v;
        }

        /// <summary>
        /// Rotates a Vector2.
        /// </summary>
        /// <param name="v">The Vector2 to rotate.</param>
        /// <param name="angleDeg">How far to rotate the Vector2 in degrees.</param>
        /// <returns>The rotated Vector2.</returns>
        public static Vector2 RotateDeg(this Vector2 v,
            float angleDeg)
        {
            return v.RotateRad(angleDeg * Mathf.Deg2Rad);
        }

        /// <summary>
        /// Creates a Vector2 with a length of 1 pointing towards a certain angle.
        /// </summary>
        /// <param name="angleRad">The angle in radians.</param>
        /// <returns>The Vector2 pointing towards the angle.</returns>
        public static Vector2 CreateVector2AngleRad(float angleRad)
        {
            return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        /// <summary>
        /// Creates a Vector2 with a length of 1 pointing towards a certain angle.
        /// </summary>
        /// <param name="angleDeg">The angle in degrees.</param>
        /// <returns>The Vector2 pointing towards the angle.</returns>
        public static Vector2 CreateVector2AngleDeg(float angleDeg)
        {
            return CreateVector2AngleRad(angleDeg * Mathf.Deg2Rad);
        }

        /// <summary>
        /// Gets the rotation of a Vector2.
        /// </summary>
        /// <param name="vector">The Vector2.</param>
        /// <returns>The rotation of the Vector2 in radians.</returns>
        public static float GetAngleRad(this Vector2 vector)
        {
            return Mathf.Atan2(vector.y, vector.x);
        }

        /// <summary>
        /// Gets the rotation of a Vector2.
        /// </summary>
        /// <param name="vector">The Vector2.</param>
        /// <returns>The rotation of the Vector2 in degrees.</returns>
        public static float GetAngleDeg(this Vector2 vector)
        {
            return vector.GetAngleRad() * Mathf.Rad2Deg;
        }

        public static double NextDouble(this RandomFaster randomFaster,
            double min,
            double max)
        {
            return randomFaster.NextDouble() * (max - min) + min;
        }

        public static float NextDouble(this RandomFaster randomFaster,
            float min,
            float max)
        {
            return (float) (randomFaster.NextDouble() * (max - min) + min);
        }

        #endregion
    }

    public static class Error
    {
        public static Exception ArgumentNull(string argumentName,
            string message = "")
        {
            return new ArgumentNullException(argumentName, message);
        }

        public static Exception ArgumentOutOfRange(string argumentName)
        {
            return new ArgumentOutOfRangeException(argumentName);
        }

        public static Exception MoreThanOneElement()
        {
            return new InvalidOperationException("Sequence contains more than one element");
        }

        public static Exception MoreThanOneMatch()
        {
            return new InvalidOperationException("Sequence contains more than one matching element");
        }

        public static Exception NoElements()
        {
            return new InvalidOperationException("Sequence contains no elements");
        }

        public static Exception NoMatch()
        {
            return new InvalidOperationException("Sequence contains no matching element");
        }

        public static Exception NotSupported()
        {
            return new NotSupportedException();
        }
    }
}