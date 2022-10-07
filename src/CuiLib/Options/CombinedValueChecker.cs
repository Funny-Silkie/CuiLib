﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace CuiLib.Options
{
    /// <summary>
    /// 複数の評価を実装します。
    /// </summary>
    /// <typeparam name="T">検証する値の型</typeparam>
    [Serializable]
    internal class CombinedValueChecker<T> : ValueChecker<T>
    {
        private ValueChecker<T>[] checkers;

        /// <summary>
        /// <see cref="CombinedValueChecker{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="first">最初の評価</param>
        /// <param name="second">2番目の評価</param>
        /// <exception cref="ArgumentNullException"><paramref name="first"/>または<paramref name="second"/>がnull</exception>
        internal CombinedValueChecker(ValueChecker<T> first, ValueChecker<T> second)
        {
            Initialize(first, second);
        }

        /// <summary>
        /// <see cref="CombinedValueChecker{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="source">評価する関数のリスト</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="source"/>の要素がnull</exception>
        internal CombinedValueChecker(params ValueChecker<T>[] source)
        {
            ArgumentNullException.ThrowIfNull(source);

            if (source.Length == 2)
            {
                Initialize(source[0], source[1]);
                return;
            }

            Initialize(source);
        }

        /// <summary>
        /// 初期化を行います。
        /// </summary>
        /// <param name="first">最初の評価</param>
        /// <param name="second">2番目の評価</param>
        /// <exception cref="ArgumentNullException"><paramref name="first"/>または<paramref name="second"/>がnull</exception>
        [MemberNotNull("checkers")]
        private void Initialize(ValueChecker<T> first, ValueChecker<T> second)
        {
            ArgumentNullException.ThrowIfNull(first);
            ArgumentNullException.ThrowIfNull(second);

            if (first is CombinedValueChecker<T> c1)
            {
                if (second is CombinedValueChecker<T> c2)
                {
                    checkers = new ValueChecker<T>[c1.checkers.Length + c2.checkers.Length];
                    Array.Copy(c1.checkers, 0, checkers, 0, c1.checkers.Length);
                    Array.Copy(c2.checkers, 0, checkers, c1.checkers.Length, c2.checkers.Length);
                }
                else
                {
                    checkers = new ValueChecker<T>[c1.checkers.Length + 1];
                    Array.Copy(c1.checkers, 0, checkers, 0, c1.checkers.Length);
                    checkers[^1] = second;
                }
            }
            else if (second is CombinedValueChecker<T> c2)
            {
                checkers = new ValueChecker<T>[c2.checkers.Length + 1];
                checkers[0] = first;
                Array.Copy(c2.checkers, 0, checkers, 1, c2.checkers.Length);
            }
            else checkers = new[] { first, second };
        }

        /// <summary>
        /// 初期化を行います。
        /// </summary>
        /// <param name="source">評価する関数のリスト</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="source"/>の要素がnull</exception>
        [MemberNotNull("checkers")]
        private void Initialize(ValueChecker<T>[] source)
        {
            int length = 0;
            for (int i = 0; i < source.Length; i++)
            {
                ValueChecker<T> current = source[i];
                if (current == null) throw new ArgumentException("要素がnullです", nameof(source));

                length += current is CombinedValueChecker<T> c ? c.checkers.Length : 1;
            }

            checkers = new ValueChecker<T>[length];
            int index = 0;

            for (int i = 0; i < source.Length; i++)
            {
                ValueChecker<T> current = source[i];
                if (current is CombinedValueChecker<T> c)
                {
                    int currentLength = c.checkers.Length;
                    Array.Copy(c.checkers, 0, checkers, index, currentLength);
                    index += currentLength;
                }
                else checkers[index++] = current;
            }
        }

        /// <inheritdoc/>
        public override ValueCheckState CheckValue(T? value)
        {
            if (checkers.Length == 0) return ValueCheckState.Success;

            for (int i = 0; i < checkers.Length; i++)
            {
                ValueCheckState result = checkers[i].CheckValue(value);
                if (!result.IsValid) return result;
            }
            return ValueCheckState.Success;
        }
    }
}