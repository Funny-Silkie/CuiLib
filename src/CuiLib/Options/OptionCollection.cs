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
    /// オプションのコレクションのクラスです。
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(CollectionDebugViewer<Option>))]
    public class OptionCollection : ICollection<Option>, IReadOnlyCollection<Option>, ICollection
    {
        /// <summary>
        /// オプション比較時のキーを表します。
        /// </summary>
        [Serializable]
        internal sealed class OptionKey : IEquatable<OptionKey>, IEnumerable<string>
        {
            private readonly HashSet<string> names;

            /// <summary>
            /// <see cref="OptionKey"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="names">名前一覧</param>
            /// <exception cref="ArgumentException"><paramref name="names"/>の名前が指定されていない</exception>
            public OptionKey(IEnumerable<string> names)
            {
                this.names = names.ToHashSet(StringComparer.Ordinal);
                if (this.names.Count == 0) throw new ArgumentException("名前が指定されていません", nameof(names));
            }

            /// <summary>
            /// 指定した名前が存在するかどうかを検証します。
            /// </summary>
            /// <param name="name">検証する名前</param>
            /// <returns><paramref name="name"/>が存在していたら<see langword="true"/>，それ以外で<see langword="false"/></returns>
            public bool ContainName(string name) => names.Contains(name);

            /// <summary>
            /// 指定した名前セットと一致するかどうかを検証します。
            /// </summary>
            /// <param name="names">検証する名前セット</param>
            /// <returns><paramref name="names"/>と一致していたら<see langword="true"/>，それ以外で<see langword="false"/></returns>
            public bool HasSameNames(IEnumerable<string> names) => this.names.SetEquals(names);

            /// <inheritdoc/>
            public override bool Equals(object? obj) => Equals(obj as OptionKey);

            /// <inheritdoc/>
            public bool Equals(OptionKey? other) => other is not null && names.SetEquals(other.names);

            /// <inheritdoc/>
            public override int GetHashCode()
            {
                var hashCode = new HashCode();
                foreach (string current in names) hashCode.Add(current, names.Comparer);
                return hashCode.ToHashCode();
            }

            /// <inheritdoc/>
            public IEnumerator<string> GetEnumerator() => names.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private readonly Dictionary<string, OptionKey> keys;
        private readonly Dictionary<OptionKey, Option> options;

        /// <inheritdoc/>
        public int Count => options.Count;

        /// <summary>
        /// <see cref="OptionCollection"/>の新しいインスタンスを初期化します。
        /// </summary>
        public OptionCollection()
        {
            keys = new Dictionary<string, OptionKey>(StringComparer.Ordinal);
            options = [];
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
            if (option.GetAllNames(false).Any(keys.ContainsKey)) throw new ArgumentException("オプション名が重複しています");

            var key = new OptionKey(option.GetAllNames(false));
            foreach (string currentName in key) keys.Add(currentName, key);
            options.Add(key, option);
        }

        /// <summary>
        /// 全ての要素を削除します。
        /// </summary>
        public void Clear()
        {
            keys.Clear();
            options.Clear();
        }

        /// <summary>
        /// 指定したオプションが存在するかどうかを取得します。
        /// </summary>
        /// <param name="name">検索するオプション</param>
        /// <returns><paramref name="name"/>に対応するオプションが存在したらtrue，それ以外でfalse</returns>
        public bool Contains(string? name)
        {
            if (name is null) return false;

            return keys.ContainsKey(name);
        }

        /// <inheritdoc/>
        public bool Contains(Option option) => options.ContainsValue(option);

        /// <inheritdoc/>
        public IEnumerator<Option> GetEnumerator() => options.Values.GetEnumerator();

        /// <summary>
        /// 指定したオプションを削除します。
        /// </summary>
        /// <param name="option">削除するオプション</param>
        /// <returns><paramref name="option"/>を削除できたらtrue，それ以外でfalse</returns>
        public bool Remove(Option? option)
        {
            if (option is null) return false;

            foreach ((OptionKey key, Option value) in options)
                if (option == value)
                {
                    options.Remove(key);
                    foreach (string name in key) keys.Remove(name);
                    return true;
                }
            return false;
        }

        /// <summary>
        /// 指定した名前のオプションを取得します。
        /// </summary>
        /// <param name="shortName">検索するオプション名</param>
        /// <param name="option"><paramref name="shortName"/>に対応するオプション。取得できなかったらnull</param>
        /// <returns><paramref name="option"/>を取得できたらtrue，それ以外でfalse</returns>
        public bool TryGetValue(char shortName, [NotNullWhen(true)] out Option? option)
        {
            if (keys.TryGetValue(shortName.ToString(), out OptionKey? key))
            {
                option = options[key];
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

            if (keys.TryGetValue(name, out OptionKey? key))
            {
                option = options[key];
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

        void ICollection<Option>.CopyTo(Option[] array, int arrayIndex) => options.Values.CopyTo(array, arrayIndex);

        #endregion ICollection<T>
    }
}
