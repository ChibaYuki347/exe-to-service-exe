# .NET 8 Launcher + Services (FDE)

このサンプルは、ランチャー exe からサービス exe を子プロセス起動し、各サービスは自身の deps.json に基づいて独立に依存解決する構成を検証します。

## セットアップ

```powershell
# 共通ライブラリを NuGet パック
pwsh -NoProfile -ExecutionPolicy Bypass -File .\scripts\pack-common.ps1

# 復元 & 発行（Framework-dependent Executable: -r win-x64, --self-contained false）
pwsh -NoProfile -ExecutionPolicy Bypass -File .\scripts\restore-and-publish.ps1
```

## 実行

```powershell
# ランチャーを起動（通常）
cd .\dist\Launcher
./StartLauncher.exe

# 終了前に待機したい場合（--pause オプション）
./StartLauncher.exe --pause

# 環境変数で待機を有効化
$env:PAUSE_ON_EXIT = '1'
./StartLauncher.exe
```

### ダブルクリック（エクスプローラー）で開いた場合の挙動

- エクスプローラーから起動したと検知したときは、自動的に終了前に「Press Enter to exit...」で待機します。
- 自動待機を無効化したい場合は、`--no-pause` オプションを付けて起動してください。

## 生成物

- dist\Launcher\StartLauncher.exe (+ .deps.json / .runtimeconfig.json)
- dist\Services\Order\Order.exe (+ .deps.json / .runtimeconfig.json)
- dist\Services\Inventory\Inventory.exe (+ .deps.json / .runtimeconfig.json)

## ディレクトリ構造（主要）

```
.
├─ Common/
│  └─ Contoso.Common/                 # 共通ライブラリ（NuGet 化）
│     ├─ Contoso.Common.csproj
│     └─ src/
│        ├─ IClock.cs
│        ├─ Retry.cs
│        └─ SystemClock.cs
├─ Launcher/
│  └─ StartLauncher/                  # ランチャー（子プロセス起動）
│     ├─ StartLauncher.csproj
│     └─ Program.cs                   # --pause / --no-pause / Explorer自動ポーズ
├─ Services/
│  ├─ Order/
│  │  ├─ Order.csproj                 # PackageReference Contoso.Common
│  │  └─ Program.cs
│  └─ Inventory/
│     ├─ Inventory.csproj             # PackageReference Contoso.Common
│     └─ Program.cs
├─ scripts/                           # pack & publish スクリプト
│  ├─ pack-common.ps1
│  ├─ pack-common.sh
│  ├─ restore-and-publish.ps1
│  └─ restore-and-publish.sh
├─ local-packages/                    # ローカル NuGet フィード（.nupkg 配置先）
├─ dist/                              # publish 出力（FDE）
│  ├─ Launcher/
│  │  ├─ StartLauncher.exe
│  │  ├─ StartLauncher.deps.json
│  │  └─ StartLauncher.runtimeconfig.json
│  └─ Services/
│     ├─ Order/
│     │  ├─ Order.exe
│     │  ├─ Order.deps.json
│     │  └─ Order.runtimeconfig.json
│     └─ Inventory/
│        ├─ Inventory.exe
│        ├─ Inventory.deps.json
│        └─ Inventory.runtimeconfig.json
├─ nuget.config                       # local-packages を参照
└─ dotnet8_launcher_services_sample_20250818_215157.sln
```

## サンプル出力（実行結果）

```
Launcher: starting
[Order] exit=0
[Order] starting
[Order] now: 2025-08-19T16:52:58.1688938+09:00
[Order] ok
[Inventory] exit=0
[Inventory] starting
2025-08-19T16:52:58.6165872+09:00
[Inventory] checked
Launcher: done
```

各サービスは自身のフォルダの *.deps.json を基準に依存解決を行うため、ランチャーとは独立して動作します。
