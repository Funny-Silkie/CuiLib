using CuiLib.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace CuiLib.Commands
{
    /// <summary>
    /// コマンドのコレクションのクラスです。
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(CollectionDebugViewer<Command>))]
    public class CommandCollection : ICollection<Command>, IReadOnlyCollection<Command>, ICollection
    {
        private readonly Command? parent;
        private readonly Dictionary<string, Command> items;

        /// <inheritdoc/>
        public int Count => items.Count;

        /// <summary>
        /// <see cref="CommandCollection"/>の新しいインスタンスを初期化します。
        /// </summary>
        public CommandCollection()
        {
            items = new Dictionary<string, Command>();
        }

        /// <summary>
        /// <see cref="CommandCollection"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="parent">親となるコマンド</param>
        internal CommandCollection(Command parent)
        {
            this.parent = parent;
            items = new Dictionary<string, Command>();
        }

        /// <summary>
        /// コマンドを追加します。
        /// </summary>
        /// <param name="command">追加するコマンド</param>
        /// <exception cref="ArgumentNullException"><paramref name="command"/>がnull</exception>
        /// <exception cref="ArgumentException">同じ名前のコマンドが存在する</exception>
        public void Add(Command command)
        {
            ArgumentNullException.ThrowIfNull(command);

            items.Add(command.Name, command);
            command.Parent = parent;
        }

        /// <summary>
        /// 全てのコマンドを削除します。
        /// </summary>
        public void Clear()
        {
            if (Count == 0) return;

            foreach ((_, Command command) in items) command.Parent = null;
            items.Clear();
        }

        /// <summary>
        /// 指定した名前のコマンドが存在するかどうかを取得します。
        /// </summary>
        /// <param name="commandName">検索するコマンド名</param>
        /// <returns><paramref name="commandName"/>のコマンドが存在したらtrue，それ以外でfalse</returns>
        /// <exception cref="ArgumentNullException"><paramref name="commandName"/>がnull</exception>
        public bool Contains(string commandName)
        {
            ArgumentNullException.ThrowIfNull(commandName);

            return items.ContainsKey(commandName);
        }

        /// <summary>
        /// 指定したコマンドが存在するかどうかを取得します。
        /// </summary>
        /// <param name="command">検索するコマンド</param>
        /// <returns><paramref name="command"/>が存在したらtrue，それ以外でfalse</returns>
        /// <exception cref="ArgumentNullException"><paramref name="command"/>がnull</exception>
        public bool Contains(Command command)
        {
            ArgumentNullException.ThrowIfNull(command);

            return items.TryGetValue(command.Name, out Command? actual) && command == actual;
        }

        /// <inheritdoc/>
        public void CopyTo(Command[] array, int arrayIndex) => items.Values.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        public IEnumerator<Command> GetEnumerator()
        {
            foreach ((_, Command current) in items) yield return current;
        }

        /// <summary>
        /// 指定した名前のコマンドを削除します。
        /// </summary>
        /// <param name="commandName">削除するコマンド名</param>
        /// <returns><paramref name="commandName"/>のコマンドを削除できたらtrue，それ以外でfalse</returns>
        /// <exception cref="ArgumentNullException"><paramref name="commandName"/>がnull</exception>
        public bool Remove(string commandName)
        {
            return items.Remove(commandName);
        }

        /// <summary>
        /// 指定したコマンドを削除します。
        /// </summary>
        /// <param name="command">削除するコマンド</param>
        /// <returns><paramref name="command"/>を削除できたらtrue，それ以外でfalse</returns>
        /// <exception cref="ArgumentNullException"><paramref name="command"/>がnull</exception>
        public bool Remove(Command command)
        {
            if (!Contains(command)) return false;
            items.Remove(command.Name);
            command.Parent = null;
            return true;
        }

        /// <summary>
        /// 指定した名前のコマンドを取得します。
        /// </summary>
        /// <param name="commandName">取得するコマンド名</param>
        /// <param name="command"><paramref name="commandName"/>に対応するコマンド。存在しなかったらnull</param>
        /// <returns><paramref name="command"/>を取得できたらtrue，それ以外でfalse</returns>
        public bool TryGetCommand(string commandName, [NotNullWhen(true)] out Command? command)
        {
            return items.TryGetValue(commandName, out command);
        }

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion IEnumerable

        #region ICollection

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => ((ICollection)items).SyncRoot;

        void ICollection.CopyTo(Array array, int index) => ((ICollection)items.Values).CopyTo(array, index);

        #endregion ICollection

        #region ICollection<T>

        bool ICollection<Command>.IsReadOnly => false;

        #endregion ICollection<T>
    }
}
