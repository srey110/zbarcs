using System;

namespace zbarcs
{
    /// <summary>
    /// 解码出的符号类型，对应 zbar_symbol_type_t。
    /// </summary>
    [Flags]
    public enum SymbolType
    {
        /// <summary>
        /// 未解码出符号。
        /// </summary>
        None = 0,
        /// <summary>
        /// 中间状态。
        /// </summary>
        Partial = 1,
        /// <summary>
        /// GS1 两位附加码。
        /// </summary>
        EAN2 = 2,
        /// <summary>
        /// GS1 五位附加码。
        /// </summary>
        EAN5 = 5,
        /// <summary>
        /// EAN-8。
        /// </summary>
        EAN8 = 8,
        /// <summary>
        /// UPC-E。
        /// </summary>
        UPCE = 9,
        /// <summary>
        /// ISBN-10（由 EAN-13 得到）。
        /// </summary>
        ISBN10 = 10,
        /// <summary>
        /// UPC-A。
        /// </summary>
        UPCA = 12,
        /// <summary>
        /// EAN-13。
        /// </summary>
        EAN13 = 13,
        /// <summary>
        /// ISBN-13（由 EAN-13 得到）。
        /// </summary>
        ISBN13 = 14,
        /// <summary>
        /// EAN/UPC 复合码。
        /// </summary>
        Composite = 15,
        /// <summary>
        /// Interleaved 2 of 5。
        /// </summary>
        I25 = 25,
        /// <summary>
        /// GS1 DataBar（RSS），自 0.11 起支持。
        /// </summary>
        DataBar = 34,
        /// <summary>
        /// GS1 DataBar Expanded，自 0.11 起支持。
        /// </summary>
        DataBar_EXP = 35,
        /// <summary>
        /// Codabar，自 0.11 起支持。
        /// </summary>
        Codabar = 38,
        /// <summary>
        /// Code 39。
        /// </summary>
        CODE39 = 39,
        /// <summary>
        /// PDF417。
        /// </summary>
        PDF417 = 57,
        /// <summary>
        /// QR Code。
        /// </summary>
        QRCODE = 64,
        /// <summary>
        /// SQ Code，自 0.20.1 起支持。
        /// </summary>
        SQCODE = 80,
        /// <summary>
        /// Code 93，自 0.11 起支持。
        /// </summary>
        CODE93 = 93,
        /// <summary>
        /// Code 128。
        /// </summary>
        CODE128 = 128,
        /// <summary>
        /// 基础符号类型掩码。
        /// </summary>
        Symbol = 0x00ff,
        /// <summary>
        /// 基础符号类型掩码。
        /// </summary>
        [Obsolete("Use Symbol instead.")]
        Symbole = 0x00ff,
        /// <summary>
        /// 两位附加码标志。
        /// </summary>
        Addon2 = 0x0200,
        /// <summary>
        /// 五位附加码标志。
        /// </summary>
        Addon5 = 0x0500,
        /// <summary>
        /// 附加码标志掩码。
        /// </summary>
        Addon = 0x0700
    }

    /// <summary>
    /// 解码符号的粗略方向，对应 zbar_orientation_t。
    /// </summary>
    public enum Orientation
    {
        /// <summary>
        /// 无法确定方向。
        /// </summary>
        Unknown = -1,
        /// <summary>
        /// 正向，从左到右读取。
        /// </summary>
        Up,
        /// <summary>
        /// 侧向，从上到下读取。
        /// </summary>
        Right,
        /// <summary>
        /// 倒置，从右到左读取。
        /// </summary>
        Down,
        /// <summary>
        /// 侧向，从下到上读取。
        /// </summary>
        Left
    }

    /// <summary>
    /// 错误码，对应 zbar_error_t。
    /// </summary>
    /// <remarks>
    /// 枚举顺序与 zbar.h 中的 zbar_error_t 保持一致。
    /// </remarks>
    public enum ZBarError
    {
        /// <summary>
        /// 无错误，或 ZBar 未记录错误。
        /// </summary>
        Ok = 0,
        /// <summary>
        /// 内存不足。
        /// </summary>
        OutOfMemory,
        /// <summary>
        /// 内部库错误。
        /// </summary>
        InternalLibraryError,
        /// <summary>
        /// 不支持的请求。
        /// </summary>
        Unsupported,
        /// <summary>
        /// 无效请求。
        /// </summary>
        InvalidRequest,
        /// <summary>
        /// 系统错误。
        /// </summary>
        SystemError,
        /// <summary>
        /// 加锁错误。
        /// </summary>
        LockingError,
        /// <summary>
        /// 所有资源均忙。
        /// </summary>
        AllResourcesBusyError,
        /// <summary>
        /// X11 显示错误。
        /// </summary>
        X11DisplayError,
        /// <summary>
        /// X11 协议错误。
        /// </summary>
        X11ProtocolError,
        /// <summary>
        /// 输出窗口已关闭。
        /// </summary>
        OutputWindowClosed,
        /// <summary>
        /// Windows 系统错误。
        /// </summary>
        WindowsAPIError,
        /// <summary>
        /// 错误码数量。
        /// </summary>
        Num
    }

    /// <summary>
    /// 解码器配置项，对应 zbar_config_t。
    /// </summary>
    public enum Config
    {
        /// <summary>
        /// 启用符号类型/功能。
        /// </summary>
        Enable = 0,
        /// <summary>
        /// 可选时启用校验位。
        /// </summary>
        AddCheck,
        /// <summary>
        /// 存在校验位时输出校验位。
        /// </summary>
        EmitCheck,
        /// <summary>
        /// 启用完整 ASCII 字符集。
        /// </summary>
        ASCII,
        /// <summary>
        /// 不把二进制数据转换为文本。
        /// </summary>
        Binary,
        /// <summary>
        /// 布尔型解码器配置项数量。
        /// </summary>
        Num,
        /// <summary>
        /// 有效解码的最小数据长度。
        /// </summary>
        MinimumLength = 0x20,
        /// <summary>
        /// 有效解码的最大数据长度。
        /// </summary>
        MaximumLength,
        /// <summary>
        /// 视频一致性所需帧数。
        /// </summary>
        Uncertainty = 0x40,
        /// <summary>
        /// 启用扫描器位置数据收集。
        /// </summary>
        Position = 0x80,
        /// <summary>
        /// 解码失败时尝试反色图像。
        /// </summary>
        TestInverted,
        /// <summary>
        /// 图像扫描器垂直扫描密度。
        /// </summary>
        XDensity = 0x100,
        /// <summary>
        /// 图像扫描器水平扫描密度。
        /// </summary>
        YDensity
    }

    /// <summary>
    /// 解码器符号类型修饰符标志，对应 zbar_modifier_t。
    /// </summary>
    public enum Modifier
    {
        /// <summary>
        /// 条码带有 GS1（EAN.UCC）保留标记，例如第一个数据字符前的 FNC1。
        /// 数据可以按 GS1 AI 序列解析。
        /// </summary>
        GS1 = 0,
        /// <summary>
        /// 条码带有 AIM 保留标记，例如第一个字符或数字对之后的 FNC1。
        /// </summary>
        AIM,
        /// <summary>
        /// 修饰符数量。
        /// </summary>
        NUM
    }

    /// <summary>
    /// 视频控制项类型，对应 video_control_type_t。
    /// </summary>
    public enum ControlType
    {
        Integer = 1,
        Menu,
        Button,
        Integer64,
        String,
        Boolean
    }

    /// <summary>
    /// 扫描元素颜色。
    /// </summary>
    public enum Color
    {
        /// <summary>
        /// 亮区或条之间的空白。
        /// </summary>
        Space = 0,
        /// <summary>
        /// 暗区或彩色条段。
        /// </summary>
        Bar,
    }
}
