using System;
using System.Collections.Generic;
using System.Linq;

namespace CuiLib.Options
{
    /// <summary>
    /// AND条件で結合されたオプションのグループを表します。
    /// </summary>
    [Serializable]
    public class AndGroupOption : GroupOption
    {
        /// <inheritdoc/>
        public override bool ValueAvailable => Children.All(x => x.ValueAvailable);

        /// <inheritdoc/>
        public override sealed bool Required => Children.Any(x => x.Required);

        /// <summary>
        /// <see cref="AndGroupOption"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="children">子オプション</param>
        /// <exception cref="ArgumentNullException"><paramref name="children"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="children"/>が空または名前に重複が生じている</exception>
        public AndGroupOption(params IEnumerable<Option> children)
        {
            ThrowHelpers.ThrowIfNull(children);

            foreach (Option current in children) Children.Add(current);
            if (Children.Count == 0) throw new ArgumentException("子要素が空です", nameof(children));
        }

        /// <summary>
        /// <see cref="AndGroupOption"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="children">子オプション</param>
        /// <exception cref="ArgumentNullException"><paramref name="children"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="children"/>が空または名前に重複が生じている</exception>
        [Obsolete("Use 'new AddGroupOption(IEnumerable<Option>)' instead.")]
        public AndGroupOption(params Option[] children)
            : this(children as IEnumerable<Option>)
        {
        }
    }
}
