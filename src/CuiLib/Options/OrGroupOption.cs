using System;
using System.Collections.Generic;
using System.Linq;

namespace CuiLib.Options
{
    /// <summary>
    /// OR条件で結合されたオプションのグループを表します。
    /// </summary>
    [Serializable]
    public class OrGroupOption : GroupOption
    {
        /// <inheritdoc/>
        public override bool ValueAvailable => Children.Any(x => x.ValueAvailable);

        /// <inheritdoc/>
        public override sealed bool Required => Children.All(x => x.Required);

        /// <inheritdoc/>
        internal override sealed OptionType OptionType => OptionType.Group | OptionType.Valued | OptionType.MultiValue;

        /// <summary>
        /// <see cref="OrGroupOption"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="children">子オプション</param>
        /// <exception cref="ArgumentNullException"><paramref name="children"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="children"/>が空または名前に重複が生じている</exception>
        public OrGroupOption(IEnumerable<Option> children)
        {
            foreach (Option current in children) Children.Add(current);
            if (Children.Count == 0) throw new ArgumentException("子要素が空です", nameof(children));
        }

        /// <summary>
        /// <see cref="OrGroupOption"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="children">子オプション</param>
        /// <exception cref="ArgumentNullException"><paramref name="children"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="children"/>が空または名前に重複が生じている</exception>
        public OrGroupOption(params Option[] children)
            : this(children as IEnumerable<Option>)
        {
        }
    }
}
