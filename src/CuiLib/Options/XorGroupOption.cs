using System;
using System.Collections.Generic;
using System.Linq;

namespace CuiLib.Options
{
    /// <summary>
    /// XOR条件で結合されたオプションのグループを表します。
    /// </summary>
    [Serializable]
    public class XorGroupOption : GroupOption
    {
        /// <inheritdoc/>
        public override bool ValueAvailable => Children.Any(x => x.ValueAvailable);

        /// <inheritdoc/>
        public override sealed bool Required => Children.All(x => x.Required);

        /// <summary>
        /// <see cref="XorGroupOption"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="children">子オプション</param>
        /// <exception cref="ArgumentNullException"><paramref name="children"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="children"/>が空または名前に重複が生じている</exception>
        public XorGroupOption(params IEnumerable<Option> children)
        {
            ThrowHelpers.ThrowIfNull(children);

            foreach (Option current in children) Children.Add(current);
            if (Children.Count == 0) throw new ArgumentException("子要素が空です", nameof(children));
        }

        /// <inheritdoc/>
        internal override void ApplyValue(string name, string rawValue)
        {
            base.ApplyValue(name, rawValue);

            if (Children.Count(x => x.ValueAvailable) > 1)
            {
                IEnumerable<string> optionNames = Children.Select(x =>
                {
                    List<string> names = x.GetAllNames(true).ToList();
                    if (names.Count == 0) return string.Empty;
                    if (names.Count == 1) return names[0];
                    return string.Join(", ", names);
                }).Where(x => !string.IsNullOrEmpty(x));
                throw new ArgumentAnalysisException($"オプション[{string.Join(", ", optionNames)}]が同時に2つ以上指定されています");
            }
        }
    }
}
