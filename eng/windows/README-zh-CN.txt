OpenTabletDriver 绿色版（Windows x64）
======================================

版本: 0.6.8.1 + codex 改动（中文界面、笔迹平滑、压力修复等）
平台: Windows 10 / 11 x64

一、用法
  解压后直接双击 OpenTabletDriver.UX.Wpf.exe 即可启动图形界面。
  不需要安装 .NET 运行时，本目录已经包含完整运行时。
  请保持目录内所有文件在一起，不要单独移动某个 exe。

  目录里主要有：
    OpenTabletDriver.UX.Wpf.exe   图形界面（双击这个）
    OpenTabletDriver.Daemon.exe   驱动后端（由界面自动启动，不要手动关掉）
    userdata\                     便携数据目录

二、便携模式
  本目录已预置 userdata 文件夹，程序会把它当作便携模式：
  所有设置、预设、插件、日志都写在这里，不会写入 %LOCALAPPDATA%。

  想恢复为系统用户目录存储，删除 userdata 文件夹即可。

三、驱动后端
  图形界面会自动启动 OpenTabletDriver.Daemon.exe。
  任务管理器里同时看到两个进程属于正常现象。
  如果界面一直显示“正在连接 OpenTabletDriver 后台服务”，请确认
  OpenTabletDriver.Daemon.exe 与界面在同一个目录内，并且没有被杀软隔离。

四、首次使用
  若系统弹窗要求安装 / 绑定 WinUSB 驱动组件，请允许。
  程序不建议以管理员身份运行；若你的账户是内置 Administrator，部分功能可能受限。

五、卸载
  直接删除整个文件夹即可，系统目录不会留下配置（因为使用了 userdata）。
