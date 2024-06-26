﻿using System;
using System.Collections.Generic;

namespace CuiLib.Options
{
    /// <summary>
    /// コマンドのオプションを表します。
    /// </summary>
    [Serializable]
    public abstract class Option
    {
        /// <summary>
        /// オプションの説明を取得または設定します。
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 取得する値の種類を取得または設定します。
        /// </summary>
        public string? ValueTypeName { get; set; }

        /// <summary>
        /// 値を受け取ったかどうかを表す値を取得します。
        /// </summary>
        public abstract bool ValueAvailable { get; }

        /// <summary>
        /// オプションの種類を取得します。
        /// </summary>
        internal abstract OptionType OptionType { get; }

        /// <summary>
        /// オプションが値をとるかどうかを取得します。
        /// </summary>
        internal bool IsValued => OptionType.HasFlag(OptionType.Valued);

        /// <summary>
        /// 複数の値を取れるかどうかを取得します。
        /// </summary>
        internal bool CanMultiValue => OptionType.HasFlag(OptionType.MultiValue);

        /// <summary>
        /// グループを表すかどうかを取得します。
        /// </summary>
        internal bool IsGroup => OptionType.HasFlag(OptionType.Group);

        /// <summary>
        /// 必須のオプションかどうかを取得または設定します。
        /// </summary>
        public abstract bool Required { get; set; }

        /// <summary>
        /// <see cref="Option"/>の新しいインスタンスを初期化します。
        /// </summary>
        protected Option()
        {
        }

        /// <summary>
        /// 設定されている値をクリアします。
        /// </summary>
        internal abstract void ClearValue();

        /// <summary>
        /// 名前に応じた実際のオプションを取得します。
        /// </summary>
        /// <param name="name">オプション名</param>
        /// <param name="isSingle">ハイフン一つの短いオプションであるかどうか</param>
        /// <returns><paramref name="name"/>に応じたオプション</returns>
        /// <exception cref="ArgumentException"><paramref name="name"/>が無効</exception>
        internal abstract Option GetActualOption(string name, bool isSingle);

        /// <summary>
        /// 値を設定します。
        /// </summary>
        /// <param name="name">オプション名</param>
        /// <param name="rawValue">文字列としての値</param>
        internal abstract void ApplyValue(string name, string rawValue);

        /// <summary>
        /// 指定した名前がインスタンスのオプション名に一致するかどうかを判定します。
        /// </summary>
        /// <param name="name">判定するオプション名</param>
        /// <returns><paramref name="name"/>がインスタンスのオプション名だったらtrue，それ以外でfalse</returns>
        public abstract bool MatchName(char name);

        /// <summary>
        /// 指定した名前がインスタンスのオプション名に一致するかどうかを判定します。
        /// </summary>
        /// <param name="name">判定するオプション名</param>
        /// <returns><paramref name="name"/>がインスタンスのオプション名だったらtrue，それ以外でfalse</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/>が空文字</exception>
        public abstract bool MatchName(string name);

        /// <summary>
        /// 全てのオプション名を取得します。
        /// </summary>
        /// <param name="includeHyphen">ハイフンを含めるかどうか</param>
        /// <returns>全てのオプション名</returns>
        internal abstract IEnumerable<string> GetAllNames(bool includeHyphen);
    }
}
