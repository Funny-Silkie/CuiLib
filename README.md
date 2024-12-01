# CuiLib

![GitHub branch check runs](https://img.shields.io/github/check-runs/Funny-Silkie/CuiLib/master)
![GitHub License](https://img.shields.io/github/license/Funny-Silkie/CuiLib)
![GitHub Release](https://img.shields.io/github/v/release/Funny-Silkie/CuiLib)
![NuGet Downloads](https://img.shields.io/nuget/dt/CuiLib)

.NETのCUIアプリケーション制作用ライブラリです。
コマンドライン引数を解析してオプションやパラメータなどの情報を取得します。

リリースログ：[RelHistory.md](https://github.com/Funny-Silkie/CuiLib/blob/master/docs/RelHistory.md)

## Usage

- `Command` クラスをコマンドの単位として使用します
  - `Command.Children` に `Command` クラスのインスタンスを登録することでサブコマンドを実装可能です
  - `Command.OnExecution()` または `Command.OnExecutionAsync()` をオーバーライドすることでコマンドの処理内容を記述できます
  - `Command.WriteHelp(Logger)` で標準的なヘルプコマンドを出力できます
- `Option` クラスをオプションとして使用します
  - `FlagOption` では `--help` のようなフラグとしてのオプションを扱います
  - `SingleValueOption<T>` では `-i hoge.txt` のように値を取るオプションを扱います
  - `MultipleValueOption<T>` では `-i hoge.txt -i fuga.txt` のように複数の値を取るオプションを扱います
  - `IValueConverter<T>` インターフェイスで文字列からの値の変換をカスタマイズできます
  - `IValueChecker<T>` インターフェイスで値のエラーチェックをカスタマイズできます
- `Parameter<T>` ではパラメータ引数を扱います
  - `IValueConverter<T>` インターフェイスで文字列からの値の変換をカスタマイズできます
  - `IValueChecker<T>` インターフェイスで値のエラーチェックをカスタマイズできます

サンプルアプリが[こちら](./sample)に用意されています。