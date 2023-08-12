using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CuiLib.Options
{
    /// <summary>
    /// グループ化されたオプションを表します。
    /// </summary>
    [Serializable]
    public abstract class GroupOption : Option, IEnumerable<Option>
    {
        /// <summary>
        /// 子オプション一覧を取得します。
        /// </summary>
        protected OptionCollection Children { get; }

        /// <inheritdoc/>
        internal override string? DefaultValueString => null;

        /// <inheritdoc/>
        public override string? ValueTypeName => null;

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Required
        {
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// <see cref="GroupOption"/>の新しいインスタンスを初期化します。
        /// </summary>
        protected GroupOption()
        {
            Children = new OptionCollection();
        }

        /// <inheritdoc/>
        internal override void ApplyValue(string name, string rawValue)
        {
            Children.TryGetValue(name, out Option? option);
            option!.ApplyValue(name, rawValue);
        }

        /// <inheritdoc/>
        internal override sealed void ClearValue()
        {
            foreach (Option child in Children) child.ClearValue();
        }

        /// <inheritdoc/>
        internal override sealed IEnumerable<string> GetAllNames(bool includeHyphen) => Children.SelectMany(x => x.GetAllNames(includeHyphen));

        /// <inheritdoc/>
        public override sealed bool MatchName(char name) => Children.TryGetValue(name.ToString(), out _);

        /// <inheritdoc/>
        public override sealed bool MatchName(string name) => Children.Contains(name);

        /// <inheritdoc/>
        public IEnumerator<Option> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
