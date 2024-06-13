using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CuiLib.Checkers
{
    /// <summary>
    /// 複数の評価をOR結合で実行します。
    /// </summary>
    /// <typeparam name="T">検証する値の型</typeparam>
    [Serializable]
    internal class OrValueChecker<T> : IValueChecker<T>
    {
        private IValueChecker<T>[] checkers;

        /// <summary>
        /// <see cref="OrValueChecker{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="first">最初の評価</param>
        /// <param name="second">2番目の評価</param>
        /// <exception cref="ArgumentNullException"><paramref name="first"/>または<paramref name="second"/>がnull</exception>
        internal OrValueChecker(IValueChecker<T> first, IValueChecker<T> second)
        {
            Initialize(first, second);
        }

        /// <summary>
        /// <see cref="OrValueChecker{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="source">評価する関数のリスト</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="source"/>の要素がnull</exception>
        internal OrValueChecker(params IValueChecker<T>[] source)
        {
            ThrowHelpers.ThrowIfNull(source);

            if (source.Length == 2)
            {
                try
                {
                    Initialize(source[0], source[1]);
                }
                catch (ArgumentNullException)
                {
                    throw new ArgumentException("要素がnullです", nameof(source));
                }
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
        [MemberNotNull(nameof(checkers))]
        private void Initialize(IValueChecker<T> first, IValueChecker<T> second)
        {
            ThrowHelpers.ThrowIfNull(first);
            ThrowHelpers.ThrowIfNull(second);

            if (first is OrValueChecker<T> c1)
            {
                if (second is OrValueChecker<T> c2)
                {
                    checkers = new IValueChecker<T>[c1.checkers.Length + c2.checkers.Length];
                    Array.Copy(c1.checkers, 0, checkers, 0, c1.checkers.Length);
                    Array.Copy(c2.checkers, 0, checkers, c1.checkers.Length, c2.checkers.Length);
                }
                else
                {
                    checkers = new IValueChecker<T>[c1.checkers.Length + 1];
                    Array.Copy(c1.checkers, 0, checkers, 0, c1.checkers.Length);
                    checkers[^1] = second;
                }
            }
            else if (second is OrValueChecker<T> c2)
            {
                checkers = new IValueChecker<T>[c2.checkers.Length + 1];
                checkers[0] = first;
                Array.Copy(c2.checkers, 0, checkers, 1, c2.checkers.Length);
            }
            else checkers = [first, second];
        }

        /// <summary>
        /// 初期化を行います。
        /// </summary>
        /// <param name="source">評価する関数のリスト</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="source"/>の要素がnull</exception>
        [MemberNotNull(nameof(checkers))]
        private void Initialize(IValueChecker<T>[] source)
        {
            int length = 0;
            for (int i = 0; i < source.Length; i++)
            {
                IValueChecker<T> current = source[i] ?? throw new ArgumentException("要素がnullです", nameof(source));
                length += current is OrValueChecker<T> c ? c.checkers.Length : 1;
            }

            checkers = new IValueChecker<T>[length];
            int index = 0;

            for (int i = 0; i < source.Length; i++)
            {
                IValueChecker<T> current = source[i];
                if (current is OrValueChecker<T> c)
                {
                    int currentLength = c.checkers.Length;
                    Array.Copy(c.checkers, 0, checkers, index, currentLength);
                    index += currentLength;
                }
                else checkers[index++] = current;
            }
        }

        /// <inheritdoc/>
        public ValueCheckState CheckValue(T value)
        {
            if (checkers.Length == 0) return ValueCheckState.Success;

            var builder = new StringBuilder();
            builder.AppendLine("以下の問題の何れかを解決して下さい");

            for (int i = 0; i < checkers.Length; i++)
            {
                ValueCheckState result = checkers[i].CheckValue(value);
                if (result.IsValid) return result;
                builder.Append("- ");
                builder.AppendLine(result.Error);
            }
            return ValueCheckState.AsError(builder.ToString());
        }
    }
}
