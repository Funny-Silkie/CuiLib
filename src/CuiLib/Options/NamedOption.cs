using System;
using System.Collections.Generic;

namespace CuiLib.Options
{
    /// <summary>
    /// 名前を持つオプションを表します。
    /// </summary>
    [Serializable]
    public abstract class NamedOption : Option
    {
        /// <summary>
        /// オプション名を取得します。
        /// </summary>
        public string? FullName { get; }

        /// <summary>
        /// 短縮名を取得します。
        /// </summary>
        public string? ShortName { get; }

        /// <summary>
        /// <see cref="NamedOption"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="shortName">短縮名</param>
        protected NamedOption(char shortName)
        {
            ShortName = shortName.ToString();
        }

        /// <summary>
        /// <see cref="NamedOption"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="fullName">完全名</param>
        /// <exception cref="ArgumentNullException"><paramref name="fullName"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="fullName"/>が空文字</exception>
        protected NamedOption(string fullName)
        {
            ThrowHelper.ThrowIfNullOrEmpty(fullName);

            FullName = fullName;
        }

        /// <summary>
        /// <see cref="NamedOption"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="shortName">短縮名</param>
        /// <param name="fullName">完全名</param>
        /// <exception cref="ArgumentNullException"><paramref name="fullName"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="fullName"/>が空文字</exception>
        protected NamedOption(char shortName, string fullName)
        {
            ThrowHelper.ThrowIfNullOrEmpty(fullName);

            FullName = fullName;
            ShortName = shortName.ToString();
        }

        /// <inheritdoc/>
        public override bool MatchName(char name)
        {
            return ShortName is not null && ShortName[0] == name;
        }

        /// <inheritdoc/>
        public override bool MatchName(string name)
        {
            ThrowHelper.ThrowIfNullOrEmpty(name);

            return (ShortName != null && ShortName == name) || (FullName != null && FullName == name);
        }

        /// <inheritdoc/>
        internal override Option GetActualOption(string name, bool isSingle)
        {
            if (isSingle)
            {
                if (name == ShortName) return this;
                throw new ArgumentException($"無効なオプション名'-{name}'です");
            }

            if (name == FullName) return this;
            throw new ArgumentException($"無効なオプション名'--{name}'です");
        }

        /// <inheritdoc/>
        internal override IEnumerable<string> GetAllNames(bool includeHyphen)
        {
            if (ShortName is not null) yield return includeHyphen ? $"-{ShortName}" : ShortName;
            if (FullName is not null) yield return includeHyphen ? $"--{FullName}" : FullName;
        }
    }
}
