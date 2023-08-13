# リリースログ

## v1.1.0

### 破壊的変更

- `ValueConverter<TIn, TOut>` クラスの代替として `IValueConverter<TIn, TOut>` インターフェイスが新たに実装
  - クラスの方はインターフェイスに置き換えられて `Obsolate` に
  - インターフェイス化によって共変・反変の恩恵
- `ValueChecker<T>` クラスの代替として `IValueChecker<T>` インターフェイスが新たに実装
  - クラスの方はインターフェイスに置き換えられて `Obsolate` に
  - インターフェイス化によって共変・反変の恩恵
- `SourceFileChecker`, `DestinationFileChecker`, `SourceDirectoryChecker` が `ValueChecker` クラスのメソッドに統合，クラスは `Obsolate` に
- `Option` クラスの派生方式が変更
  - 新たに実装: `NamedOption`, `ValueSpecifiedOption<T>`
  - 廃止: `Option<T>`

### 変更

- `Logger` クラスが `TextWriter` クラスを継承するように変更
- `Logger` クラスにて `TextWriter` を直接追加・削除できるように変更
- `CommandCollection`, `OptionCollection`, `ParameterCollection` クラスに `DebuggerDisplayAttribute` と `DebuggerTypeProxy` を設定，従来のコレクションと同様のデバッグビューを提供
- `ValueConverter.StringToEnum<TEnum>()` メソッドのオーバーロードでCase-sensitive or Case-insensitiveの選択ができる`ValueConverter.StringToEnum<TEnum>(bool)` が実装
- `ValueConverter` に，文字列を分割して配列を生成するインスタンスを返す `SplitToArray` メソッドを実装
- `ValueChecker` に，正規表現にマッチするかどうかを判定するインスタンスを返す `IsRegexMatch` メソッドを実装

## v1.0.1

### 変更

- ヘルプメッセージのフォーマット変更
- `ValueConverter` クラスで用意されている一部 `ValueConverter<TIn, TOut>` の `null` 許容の変更
- `SingleValueOption<T>.Converter` ・ `MultiValueOption<T>` ・ `Parameter<T>.Converter` の `null` 許容の変更
- `ParameterCollection` に `AllowAutomaticallyCreate` プロパティを実装， `false` の時に過剰な数のパラメータが与えられると `InvalidOperationException` がスロー
