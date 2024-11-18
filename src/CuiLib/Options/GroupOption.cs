using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
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
        internal override OptionType OptionType => OptionType.Group;

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Required
        {
            [DoesNotReturn]
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// <see cref="GroupOption"/>の新しいインスタンスを初期化します。
        /// </summary>
        protected GroupOption()
        {
            Children = [];
        }

        /// <inheritdoc/>
        internal override void ApplyValue(string name, string rawValue)
        {
            Children.TryGetValue(name, out Option? option);
            if (option is null) throw new ArgumentAnalysisException("存在しないオプションが指定されました");

            option.ApplyValue(name, rawValue);
        }

        /// <inheritdoc/>
        internal override sealed void ClearValue()
        {
            foreach (Option child in Children) child.ClearValue();
        }

        /// <inheritdoc/>
        internal override sealed IEnumerable<string> GetAllNames(bool includeHyphen) => Children.SelectMany(x => x.GetAllNames(includeHyphen));

        /// <inheritdoc/>
        internal override sealed Option GetActualOption(string name, bool isSingle)
        {
            Option result = Children.SingleOrDefault(x => x.MatchName(name)) ?? throw new ArgumentException($"無効なオプション名'-{(isSingle ? string.Empty : "-")}{name}'です");
            if (result is GroupOption) return result.GetActualOption(name, isSingle);
            return result;
        }

        /// <inheritdoc/>
        public override sealed bool MatchName(char name) => Children.TryGetValue(name.ToString(), out _);

        /// <inheritdoc/>
        public override sealed bool MatchName(string name)
        {
            ThrowHelpers.ThrowIfNullOrEmpty(name);

            return Children.Contains(name);
        }

        /// <inheritdoc/>
        public IEnumerator<Option> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
