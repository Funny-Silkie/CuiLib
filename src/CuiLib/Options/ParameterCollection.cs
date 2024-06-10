using CuiLib.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CuiLib.Options
{
    /// <summary>
    /// パラメータのコレクションのクラスです。
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(CollectionDebugViewer<Parameter>))]
    public class ParameterCollection : ICollection<Parameter>, IReadOnlyCollection<Parameter>, ICollection
    {
        private int arrayStart = -1;
        private readonly SortedList<int, Parameter> items;

        /// <summary>
        /// 未定義のパラメータの自動生成を許すかどうかを取得または設定します。
        /// </summary>
        public bool AllowAutomaticallyCreate { get; set; }

        /// <inheritdoc/>
        public int Count => items.Count;

        /// <summary>
        /// 配列を表すパラメータを格納しているかどうかを表す値を取得します。
        /// </summary>
        public bool HasArray => arrayStart >= 0;

        /// <summary>
        /// <see cref="ParameterCollection"/>の新しいインスタンスを初期化します。
        /// </summary>
        public ParameterCollection()
        {
            items = [];
        }

        /// <summary>
        /// 指定したインデックスの要素を取得します。
        /// </summary>
        /// <param name="index">インデックス</param>
        /// <returns><paramref name="index"/>に対応する要素</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/>が無効な値</exception>
        public Parameter this[int index]
        {
            get
            {
                ThrowHelpers.ThrowIfNegative(index);

                if (arrayStart >= 0 && index > arrayStart) return items[arrayStart];
                try
                {
                    return items[index];
                }
                catch (KeyNotFoundException)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "無効なインデックスです");
                }
            }
        }

        /// <summary>
        /// 値を設定します。
        /// </summary>
        /// <param name="values">設定する値</param>
        internal void SetValues(ReadOnlySpan<string> values)
        {
            if (arrayStart == -1 || values.Length - 1 < arrayStart)
            {
                for (int i = 0; i < values.Length; i++) SetOrCreate(i, values[i]);
            }
            else
            {
                for (int i = 0; i < arrayStart; ++i) SetOrCreate(i, values[i]);
                items[arrayStart].SetValue(values[arrayStart..]);
            }
        }

        /// <summary>
        /// <see cref="Parameter{T}"/>の新しいインスタンスを生成して空きインデックスのうち先頭のものに追加します。
        /// </summary>
        /// <typeparam name="T">値の型</typeparam>
        /// <param name="name">パラメータ名</param>
        /// <returns>追加された<see cref="Parameter{T}"/>のインスタンス</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/>が空文字</exception>
        /// <exception cref="InvalidOperationException">追加できる空きインデックスが存在しない</exception>
        public Parameter<T> CreateAndAdd<T>(string name)
        {
            int next = GetNextIndex();
            if (next < 0) throw new InvalidOperationException("空きインデックスが存在しません");

            Parameter<T> result = Parameter.Create<T>(name, next);
            items.Add(next, result);
            return result;
        }

        /// <summary>
        /// <see cref="Parameter{T}"/>の新しいインスタンスを生成して末尾の空きインデックスに追加します。
        /// </summary>
        /// <typeparam name="T">値の型</typeparam>
        /// <param name="name">パラメータ名</param>
        /// <returns>追加された<see cref="Parameter{T}"/>のインスタンス</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/>が空文字</exception>
        /// <exception cref="ArgumentAnalysisException">配列が既に含まれている</exception>
        public Parameter<T> CreateAndAddAsArray<T>(string name)
        {
            if (arrayStart != -1) throw new ArgumentAnalysisException("既に配列が含まれています");

            int index = Count == 0 ? 0 : items.Last().Key + 1;
            Parameter<T> result = Parameter.CreateAsArray<T>(name, index);
            items.Add(index, result);
            arrayStart = index;
            return result;
        }

        /// <summary>
        /// 指定したインデックスの値を設定し，存在しない場合は新たにパラメータを生成して設定します。
        /// </summary>
        /// <param name="index">インデックス</param>
        /// <param name="value">設定する値</param>
        /// <returns>値を設定したパラメータ</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/>が0未満</exception>
        /// <exception cref="InvalidOperationException"><see cref="AllowAutomaticallyCreate"/>が<see langword="false"/>の時に未定義インデックスのパラメータが存在</exception>
        private Parameter SetOrCreate(int index, string value)
        {
            if (index < 0) ThrowHelpers.ThrowIfNegative(index);

            if (!items.TryGetValue(index, out Parameter? result))
            {
                if (!AllowAutomaticallyCreate) throw new InvalidOperationException($"{index}番目のパラメータは存在しません");
                result = Parameter.Create<string>($"Param {index}", index);
                items[index] = result;
            }
            result.SetValue(value);
            return result;
        }

        /// <summary>
        /// 空きインデックスを取得します。
        /// </summary>
        /// <returns>空きインデックス。存在しない場合は-1</returns>
        private int GetNextIndex()
        {
            int result = 0;
            foreach ((int index, _) in items)
            {
                if (index != result) return result;
                result++;
            }
            if (arrayStart != -1) return -1;
            return result;
        }

        #region Collection Opreations

        /// <summary>
        /// 要素を追加します。
        /// </summary>
        /// <param name="parameter">追加するパラメータ</param>
        /// <exception cref="ArgumentNullException"><paramref name="parameter"/>がnull</exception>
        /// <exception cref="InvalidOperationException"><paramref name="parameter"/>が配列を表す且つ既に配列が含まれている</exception>
        /// <exception cref="ArgumentException"><paramref name="parameter"/>のインデックスが配列の領域を指すまたはインデックスが衝突している</exception>
        public void Add(Parameter parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter);

            if (arrayStart >= 0)
            {
                if (parameter.IsArray) throw new InvalidOperationException("既に配列が含まれています");
                if (parameter.Index >= arrayStart) throw new ArgumentException("配列に指定されているインデックスです", nameof(parameter));
            }
            if (parameter.IsArray) arrayStart = parameter.Index;
            items.Add(parameter.Index, parameter);
        }

        /// <summary>
        /// 指定した要素が格納されているかどうかを検証します。
        /// </summary>
        /// <param name="parameter">検索する要素</param>
        /// <returns><paramref name="parameter"/>が格納されていたらtrue，それ以外でfalse</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parameter"/>がnull</exception>
        public bool Contains(Parameter parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter);

            return TryGetValue(parameter.Index, out Parameter? actual) && parameter == actual;
        }

        /// <summary>
        /// 指定したインデックスに対応する要素が格納されているかどうかを検証します。
        /// </summary>
        /// <param name="index">検索する要素のインデックス</param>
        /// <returns><paramref name="index"/>に対応する要素が格納されていたらtrue，それ以外でfalse</returns>
        public bool ContainsAt(int index)
        {
            return items.ContainsKey(index) || (HasArray && index >= arrayStart);
        }

        /// <summary>
        /// 全ての要素を削除します。
        /// </summary>
        public void Clear()
        {
            if (Count == 0) return;

            items.Clear();
            arrayStart = -1;
        }

        /// <inheritdoc/>
        public void CopyTo(Parameter[] array, int arrayIndex) => items.Values.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        public IEnumerator<Parameter> GetEnumerator() => items.Values.GetEnumerator();

        /// <summary>
        /// 指定した要素を削除します。
        /// </summary>
        /// <param name="parameter">削除する要素</param>
        /// <returns><paramref name="parameter"/>を削除できたらtrue，それ以外でfalse</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parameter"/>がnull</exception>
        public bool Remove(Parameter parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter);

            if (!Contains(parameter) || !items.Remove(parameter.Index)) return false;
            if (parameter.IsArray) arrayStart = -1;
            return true;
        }

        /// <summary>
        /// 指定したインデックスの要素を削除します。
        /// </summary>
        /// <param name="index">インデックス</param>
        /// <returns><paramref name="index"/>に対応する値を削除できたらtrue，それ以外でfalse</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/>が0未満</exception>
        public bool RemoveAt(int index)
        {
            ThrowHelpers.ThrowIfNegative(index);

            if (!items.Remove(index, out Parameter? removed)) return false;
            if (removed.IsArray) arrayStart = -1;
            return true;
        }

        /// <summary>
        /// 指定したインデックスの要素を取得します。
        /// </summary>
        /// <param name="index">インデックス</param>
        /// <param name="parameter"><paramref name="index"/>に対応する要素。存在しなかったらnull</param>
        /// <returns><paramref name="parameter"/>を取得できたらtrue，それ以外でfalse</returns>
        public bool TryGetValue(int index, [NotNullWhen(true)] out Parameter? parameter)
        {
            if (index < 0)
            {
                parameter = null;
                return false;
            }
            if (HasArray && index >= arrayStart)
            {
                parameter = items[arrayStart];
                return true;
            }
            return items.TryGetValue(index, out parameter);
        }

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion IEnumerable

        #region ICollection

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        void ICollection.CopyTo(Array array, int index) => ((ICollection)items.Values).CopyTo(array, index);

        #endregion ICollection

        #region ICollection<T>

        bool ICollection<Parameter>.IsReadOnly => false;

        #endregion ICollection<T>

        #endregion Collection Opreations
    }
}
