using System;
using System.Collections.Generic;

namespace OpenTabletDriver.UX
{
    public static class Language
    {
        public static bool IsChinese { get; private set; } = true;
        public static event EventHandler? Changed;

        private static readonly Dictionary<string, string> Chinese = new()
        {
            ["Save"] = "保存", ["Apply"] = "应用", ["Output"] = "输出", ["Filters"] = "滤镜",
            ["Pen Settings"] = "笔设置", ["Auxiliary Settings"] = "辅助设置", ["Mouse Settings"] = "鼠标设置",
            ["Tools"] = "工具", ["Info"] = "信息", ["Console"] = "控制台", ["No tablets are detected."] = "未检测到数位板。",
            ["Copy All"] = "全部复制", ["Copy"] = "复制", ["Align"] = "对齐", ["Resize"] = "调整大小",
            ["Flip"] = "翻转", ["Left"] = "左", ["Right"] = "右", ["Top"] = "上", ["Bottom"] = "下",
            ["Center"] = "居中", ["Full area"] = "完整区域", ["Quarter area"] = "四分之一区域",
            ["Horizontal"] = "水平", ["Vertical"] = "垂直", ["Lock to usable area"] = "锁定到可用区域",
            ["Language"] = "语言", ["Chinese"] = "中文", ["English"] = "English"
            , ["Quit"] = "退出", ["About..."] = "关于...", ["Open Wiki..."] = "打开 Wiki...",
            ["Help"] = "帮助", ["File"] = "文件", ["Tablets"] = "数位板", ["Plugins"] = "插件",
            ["Reset to defaults"] = "恢复默认设置"
        };

        public static string T(string text)
        {
            if (IsChinese && Chinese.TryGetValue(text, out var translated)) return translated;
            foreach (var item in Chinese)
                if (item.Value == text) return item.Key;
            return text;
        }

        public static void SetChinese(bool chinese)
        {
            if (IsChinese == chinese) return;
            IsChinese = chinese;
            Changed?.Invoke(null, EventArgs.Empty);
        }
    }
}
