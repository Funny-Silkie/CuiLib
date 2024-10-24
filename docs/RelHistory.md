# リリースログ

## v2.0.0

### 破壊的変更
- 名前空間の変更
  - `Log` → `Logging`
  - `IValueChecker` 関連： `Options` → `Checkers`
  - `IValueConverter` 関連： `Options` → `Converters`
  - `Parameter` 関連： `Options` → `Parameters`
- クラス名変更
  - `Util` を `Utils` に変更
  - `IOHelper` を `IOHelpers` に変更
- Obsoleteな実装の削除
- Checkersのファクトリメソッドの名称の変更
  - AlwaysSuccess -> AlwaysValid
  - StartWith -> StartsWith
  - EndWith -> EndsWith
  - Contains -> ContainedIn
  - Larger -> GreaterThan
  - LargerOrEqual -> GreaterThanOrEqualTo
  - Lower -> LessThan
  - LowerOrEqual -> LessThanOrEqualTo
  - Equals -> EqualTo
  - NotEquals -> NotEqualTo
  - IsRegexMatch -> Matches
  - FileExists -> ExistsAsFile
  - DirectoryExists -> ExistsAsDirectory
  - VerifySourceFile -> ValidSourceFile
  - VerifyDestinationFile -> ValidDestinationFile
  - VerifySourceDirectory -> ValidSourceDirectory
- Logger
  - コンストラクタでファイルパスを指定した際のデフォルトの挙動が新規作成から追加モードに変更
- Parameter
  - `Parameter<T>` を値の個数に応じて `SingleValueParameter<T>` と `MultpileValueParameter<T>` に分割， `SingleValueOption<T>`, `MultipleValueOption<T>` と同じ使用感に
  - `Parameter<T>.Checker`, `Parameter<T>.Converter`, `Parameter<T>.Values` を削除
  - `Parameter<T>.CreateAsArray<T>(string, int)`, `ParameterCollection.CreateAndAddAsArray<T>(string)` の戻り値が `Parameter<T>` から `MultipleValueParameter<T>` （`Parameter<T[]>`）に変更

### 変更
- .NET 6.0，.NET Standard 2.1 (.NET Core 3.1) , .NET Standard 2.0 (.NET Framework 4.8.1) へのサポート拡大
- 例外処理修正
- 一部メソッドのnull許容・非許容の修正
- `Logger`
  - `Flush`, `FlushAsync` 実装
  - `FileInfo` を取るコンストラクタの追加
  - ファイルパスを取るコンストラクタで追加or新規作成の選択，文字コードの設定が可能に
  - デフォルトの文字コードを指定できるように `DefaultEncoding` プロパティを実装
  - `AddLog(TextWriter, bool)` のオーバーロードを追加，インスタンス破棄時に同時に破棄するかを設定可能に
- Data
  - 範囲を表すオブジェクトである `ValueRange`, `ValueRangeCollection` を追加
- Checkers
  - 新たなファクトリメソッドの追加
    - `Empty()`, `Empty<TElement>()`, `NotEmpty()`, `NotEmpty<TElement>()`
- Converters
  - `ValueConverter.GetDefault<T>()`：Enumや配列を指定した際に変換できない不具合を修正
  - 新規実装
    - `string` からプリミティブ型への変換を行うConverter
    - `Combine<TIn, TMid, TOut>(this IValueConverter<TIn, TMid>, Converter<TMid, TOut>)` のオーバーロード
    - `string` から `StreamWriter`, `StreamReader` への変換
    - `string` から `In128`, `UInt128` への変換
    - `string` から `DateTime`, `DateOnly`, `TimeOnly`, `DateTimeOffset`, `TimeSpan` 変換時にフォーマットを指定できるように
- Options
  - `Required` と重複しており且つ機能していなかった `IsRequired` プロパティを削除
  - `ValueTypeName`：型名に配列を指定した際のエラーを修正
  - `XorGroupOption` に `AndGroupOption` や `MultipleValueOption<T>` を格納した際に値が複数設定できない不具合を修正
  - `ValueTypeName` を設定可能に
  - `MultipleValueOption<T>` の値と見做す連続した値の個数を設定可能に
- Parameters
  - 配列パラメータが存在する場合の `ParameterCollection` の挙動を修正
  - `ParameterCollection` の `ContainsAt(int)` の挙動を `TryGetValue(int, out Parameter?)` のものと同一化
  - `ParameterCollection.Remove(Parameter)` の致命的な挙動を修正
- Extensions
  - `SpanExtensions`
    - `SliceOrDefault` の挙動を変更
    - 各メソッドの `Span<T>` 版を実装
  - `StringExtensions`
    - 各メソッドの `string` 版を実装
    - `ExcapedSplit(string)` を `null` ・空文字非許容に，そして長い文字列を `separator` とした際の挙動を修正
  - `CollectionExtensions.GetOrDefault<T>(IList<T>, int)`, `SpanExtensions.GetOrDefault<T>(ReadOnlySpan<T>, int)` の戻り値をnull許容に
- `ThrowHelper` 公開の停止
- `CommandCollection.Remove(string)` で親コマンドが解除されない不具合を修正
- ヘルプメッセージのフォーマット変更

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
