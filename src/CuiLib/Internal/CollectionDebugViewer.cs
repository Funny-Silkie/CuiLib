using System.Collections.Generic;
using System.Diagnostics;

namespace CuiLib.Internal
{
    /// <summary>
    /// コレクション用のデバッグビューのクラスです。
    /// </summary>
    /// <typeparam name="T">要素の型</typeparam>
    internal class CollectionDebugViewer<T>
    {
        private readonly ICollection<T> source;

        /// <summary>
        /// 表示する要素一覧を取得します。
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] result = new T[source.Count];
                source.CopyTo(result, 0);
                return result;
            }
        }

        /// <summary>
        /// <see cref="CollectionDebugViewer{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="source">表示するコレクション</param>
        internal CollectionDebugViewer(ICollection<T> source)
        {
            this.source = source;
        }
    }
}
