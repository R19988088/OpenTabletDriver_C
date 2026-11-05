OpenTabletDriver 单文件绿色版
==============================

版本: 0.6.8.1 + codex 改动（中文界面、笔迹平滑、压力修复等）
平台: Windows 10/11 x64

用法
  双击 OpenTabletDriver.exe 即可。不需要安装 .NET，也不需要其它文件。

关于便携数据
  首次运行后，配置文件会生成在:
      %LOCALAPPDATA%\OpenTabletDriver
  如果想改成绿色便携模式（数据放在 exe 旁边），在 exe 同目录新建一个名为
  userdata 的文件夹，然后再启动即可。所有设置、预设、插件、日志都会写进去。

驱动后端
  图形界面会自动在后台启动驱动进程（同一个 exe，带 --daemon 参数）。
  任务管理器里看到两个 OpenTabletDriver.exe 属于正常现象，这是界面和驱动后端。

首次使用
  若系统弹窗要求安装/绑定 WinUSB 驱动组件，请允许。
  程序不以管理员身份运行；若你的账户是内置 Administrator，功能可能受限。
