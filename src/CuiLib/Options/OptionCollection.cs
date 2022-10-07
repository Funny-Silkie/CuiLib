using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CuiLib.Options
{
    /// <summary>
    /// オプションのコレクションのクラスです。
    /// </summary>
    [Serializable]
    public class OptionCollection : ICollection<Option>, IReadOnlyCollection<Option>, ICollection
    {
        private static readonly IEqualityComparer<Option> Comparer = new OptionEqualityComparer();

        private readonly HashSet<Option> options;

        /// <inheritdoc/>
        public int Count => options.Count;

        /// <summary>
        /// <see cref="OptionCollection"/>の新しいインスタンスを初期化します。
        /// </summary>
        public OptionCollection()
        {
            options = new HashSet<Option>(Comparer);
        }

        /// <summary>
        /// オプションを追加します。
        /// </summary>
        /// <param name="option">追加するオプション</param>
        /// <exception cref="ArgumentNullException"><paramref name="option"/>がnull</exception>
        /// <exception cref="ArgumentException">既にオプションが追加されている-または-オプション名が衝突している</exception>
        public void Add(Option option)
        {
            ArgumentNullException.ThrowIfNull(option);

            if (!options.Add(option)) throw new ArgumentException("オプション名が重複しています");
        }

        /// <summary>
        /// 全ての要素を削除します。
        /// </summary>
        public void Clear() => options.Clear();

        /// <summary>
        /// 指定したオプションが存在するかどうかを取得します。
        /// </summary>
        /// <param name="name">検索するオプション</param>
        /// <returns><paramref name="name"/>に対応するオプションが存在したらtrue，それ以外でfalse</returns>
        public bool Contains(string? name)
        {
            if (name is null) return false;

            foreach (Option current in options)
                if (current.ShortName == name || current.FullName == name)
                    return true;
            return false;
        }

        /// <inheritdoc/>
        public bool Contains(Option option)
        {
            return options.TryGetValue(option, out Option? actual) && option == actual;
        }

        /// <inheritdoc/>
        public IEnumerator<Option> GetEnumerator() => options.GetEnumerator();

        /// <summary>
        /// 指定したオプションを削除します。
        /// </summary>
        /// <param name="option">削除するオプション</param>
        /// <returns><paramref name="option"/>を削除できたらtrue，それ以外でfalse</returns>
        public bool Remove(Option? option)
        {
            if (option is null) return false;
            return options.Remove(option);
        }

        /// <summary>
        /// 指定した名前のオプションを取得します。
        /// </summary>
        /// <param name="shortName">検索するオプション名</param>
        /// <param name="option"><paramref name="shortName"/>に対応するオプション。取得できなかったらnull</param>
        /// <returns><paramref name="option"/>を取得できたらtrue，それ以外でfalse</returns>
        public bool TryGetValue(char shortName, [NotNullWhen(true)] out Option? option)
        {
            foreach (Option current in options)
                if (current.ShortName != null && current.ShortName[0] == shortName)
                {
                    option = current;
                    return true;
                }

            option = null;
            return false;
        }

        /// <summary>
        /// 指定した名前のオプションを取得します。
        /// </summary>
        /// <param name="name">検索するオプション名</param>
        /// <param name="option"><paramref name="name"/>に対応するオプション。取得できなかったらnull</param>
        /// <returns><paramref name="option"/>を取得できたらtrue，それ以外でfalse</returns>
        public bool TryGetValue(string? name, [NotNullWhen(true)] out Option? option)
        {
            if (name is null)
            {
                option = null;
                return false;
            }
            foreach (Option current in options)
                if (current.ShortName == name || current.FullName == name)
                {
                    option = current;
                    return true;
                }

            option = null;
            return false;
        }

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion IEnumerable

        #region ICollection

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => ((ICollection)options).SyncRoot;

        void ICollection.CopyTo(Array array, int index) => ((ICollection)options).CopyTo(array, index);

        #endregion ICollection

        #region ICollection<T>

        bool ICollection<Option>.IsReadOnly => false;

        void ICollection<Option>.CopyTo(Option[] array, int arrayIndex) => options.CopyTo(array, arrayIndex);

        #endregion ICollection<T>
    }
}
