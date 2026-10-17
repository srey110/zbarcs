using System;
using System.Runtime.InteropServices;

/// <summary>
/// ZBar 条码读取库。
/// </summary>
namespace zbarcs
{
    internal static class NativeZBar
    {
        private const string DllName = "libzbar-0";

        // zbar 的 fourcc 和图像数据长度使用 unsigned long。本项目面向
        // Windows x64，该平台上 unsigned long 为 32 位，因此使用 uint 作为
        // 对应的托管类型；在 LP64 平台上这些值也能放入 32 位，.NET 会把
        // 参数扩展到原生寄存器宽度。
        #region 全局接口
        /// <summary>
        /// 获取运行时库版本信息。
        /// </summary>
        /// <param name="major">主版本号（可为 NULL）。</param>
        /// <param name="minor">次版本号（可为 NULL）。</param>
        /// <param name="patch">补丁版本号（可为 NULL）。</param>
        /// <returns>成功返回 0。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern unsafe int zbar_version(uint* major, uint* minor, uint* patch);

        /// <summary>
        /// 设置全局库调试级别。
        /// </summary>
        /// <param name="verbosity">调试级别，数值越大输出越多。</param>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void zbar_set_verbosity(int verbosity);

        /// <summary>
        /// 提高全局库调试级别，例如对应 -vvvv。
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void zbar_increase_verbosity();

        /// <summary>
        /// 获取符号编码的名称。
        /// </summary>
        /// <param name="sym">符号类型（SymbolType）。</param>
        /// <returns>指定符号类型的静态名称；无法识别时返回 "UNKNOWN"。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_get_symbol_name(int sym);

        /// <summary>
        /// 获取附加码编码的名称。
        /// </summary>
        /// <param name="sym">符号类型（SymbolType）。</param>
        /// <returns>任意附加码的静态名称；没有附加码时返回空字符串。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_get_addon_name(int sym);

        /// <summary>
        /// 获取配置项的名称。
        /// </summary>
        /// <param name="config">配置项（Config）。</param>
        /// <returns>配置项的静态名称；未知配置返回空字符串。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_get_config_name(int config);

        /// <summary>
        /// 获取修饰符的名称。
        /// </summary>
        /// <param name="modifier">修饰符标志（Modifier）。</param>
        /// <returns>修饰符的静态名称；未知标志返回空字符串。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_get_modifier_name(int modifier);

        /// <summary>
        /// 获取方向的名称。
        /// </summary>
        /// <param name="orientation">方向编码（Orientation）。</param>
        /// <returns>方向的静态名称。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_get_orientation_name(int orientation);

        /// <summary>
        /// 解析形如 "[symbology.]config[=value]" 的配置字符串。
        /// </summary>
        /// <param name="config_string">配置字符串。</param>
        /// <param name="symbology">输入的符号类型；未指定时为 0（SymbolType）。</param>
        /// <param name="config">解析出的配置项（Config）。</param>
        /// <param name="value">未指定值时默认为 1。</param>
        /// <returns>解析成功返回 0。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern unsafe int zbar_parse_config(string config_string, int* symbology, int* config, int* value);
        #endregion 全局接口

        #region 错误接口
        /// <summary>
        /// 获取原生对象的错误描述字符串。
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr _zbar_error_string(IntPtr obj, int verbosity);

        /// <summary>
        /// 获取原生对象的错误码。
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int _zbar_get_error_code(IntPtr obj);
        #endregion 错误接口

        #region 符号接口
        /// <summary>
        /// 调整符号引用计数。
        /// </summary>
        /// <remarks>
        /// 保存新的符号引用时增加计数，不再使用时减少计数。计数减少后不得再
        /// 引用该符号；如果需要在图像被回收或销毁后继续使用符号，才需要调用
        /// 本方法，因为图像本身已经持有一个符号引用。
        /// </remarks>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void zbar_symbol_ref(IntPtr symbol, int refs);

        /// <summary>
        /// 获取解码符号的类型。
        /// </summary>
        /// <returns>zbar_symbol_type_t（SymbolType）。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int zbar_symbol_get_type(IntPtr symbol);

        /// <summary>
        /// 获取符号类型的布尔配置位掩码。
        /// </summary>
        /// <returns>解码期间为该符号类型设置的配置位掩码。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern uint zbar_symbol_get_configs(IntPtr symbol);

        /// <summary>
        /// 获取符号类型的修饰符位掩码。
        /// </summary>
        /// <returns>解码期间检测到的特征位掩码。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern uint zbar_symbol_get_modifiers(IntPtr symbol);

        /// <summary>
        /// 获取符号解码出的数据。
        /// </summary>
        /// <returns>数据字符串指针。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_symbol_get_data(IntPtr symbol);

        /// <summary>
        /// 获取解码数据的长度。
        /// </summary>
        /// <returns>解码数据的字节长度。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern uint zbar_symbol_get_data_length(IntPtr symbol);

        /// <summary>
        /// 获取符号置信度指标。
        /// </summary>
        /// <returns>未缩放的相对值，越大越可靠，具体含义由应用决定。</returns>
        /// <remarks>
        /// 该指标的精确定义可能随算法改进而变化；目前只保证两个值之间的
        /// 大小关系稳定。
        /// </remarks>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int zbar_symbol_get_quality(IntPtr symbol);

        /// <summary>
        /// 获取当前缓存计数。
        /// </summary>
        /// <remarks>
        /// 图像扫描器启用缓存后，该值提供视频流帧间可靠性和冗余信息：
        /// 小于 0 表示符号仍不确定，0 表示刚被确认，大于 0 表示重复符号。
        /// </remarks>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int zbar_symbol_get_count(IntPtr symbol);

        /// <summary>
        /// 获取位置多边形中的点数。
        /// </summary>
        /// <returns>位置多边形中的点数。</returns>
        /// <remarks>
        /// 当前实现返回的不是多边形，而是解码出符号时的扫描位置点。
        /// </remarks>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern uint zbar_symbol_get_loc_size(IntPtr symbol);

        /// <summary>
        /// 获取位置多边形中指定点的 x 坐标。
        /// </summary>
        /// <param name="index">从 0 开始的索引。</param>
        /// <returns>点的 x 坐标；索引越界时返回 -1。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int zbar_symbol_get_loc_x(IntPtr symbol, uint index);

        /// <summary>
        /// 获取位置多边形中指定点的 y 坐标。
        /// </summary>
        /// <param name="index">从 0 开始的索引。</param>
        /// <returns>点的 y 坐标；索引越界时返回 -1。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int zbar_symbol_get_loc_y(IntPtr symbol, uint index);

        /// <summary>
        /// 获取解码符号的粗略方向。
        /// </summary>
        /// <returns>zbar_orientation_t（Orientation）。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int zbar_symbol_get_orientation(IntPtr symbol);

        /// <summary>
        /// 遍历符号所属的结果集合。
        /// </summary>
        /// <returns>下一个符号；没有更多结果时返回 NULL。</returns>
        /// <remarks>供符号迭代器内部使用。</remarks>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_symbol_next(IntPtr symbol);

        /// <summary>
        /// 获取复合结果的组件符号集合。
        /// </summary>
        /// <returns>
        /// 包含组件的符号集合；如果符号本身已经是物理符号，则返回 NULL
        /// （zbar_symbol_set_t）。
        /// </returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_symbol_get_components(IntPtr symbol);

        /// <summary>
        /// 遍历复合结果的组件。
        /// </summary>
        /// <returns>
        /// 复合结果的第一个物理组件符号；如果符号本身已经是物理符号，
        /// 则返回 NULL（zbar_symbol_t）。
        /// </returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_symbol_first_component(IntPtr symbol);

        /// <summary>
        /// 把符号的 XML 元素表示输出到用户缓冲区。
        /// </summary>
        /// <remarks>模式定义见 http://zbar.sourceforge.net/2008/barcode.xsd。</remarks>
        /// <param name="symbol">要输出的符号。</param>
        /// <param name="buffer">输入/输出缓冲区指针；必要时会重新分配更大的空间。</param>
        /// <param name="buflen">输入/输出缓冲区长度。</param>
        /// <returns>缓冲区指针。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_symbol_xml(IntPtr symbol, out IntPtr buffer, out uint buflen);
        #endregion 符号接口

        #region 符号集合接口
        /// <summary>
        /// 调整符号集合的引用计数。
        /// </summary>
        /// <remarks>
        /// 保存新的引用时增加计数，不再使用时减少计数；引用释放后不得再访问
        /// 该对象。
        /// </remarks>
        /// <param name="symbols">zbar_symbol_set_t。</param>
        /// <param name="refs">引用计数变化量。</param>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void zbar_symbol_set_ref(IntPtr symbols, int refs);

        /// <summary>
        /// 获取集合中的符号数量。
        /// </summary>
        /// <param name="symbols">zbar_symbol_set_t。</param>
        /// <returns>集合中的符号数量。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int zbar_symbol_set_get_size(IntPtr symbols);

        /// <summary>
        /// 获取集合的第一个符号。
        /// </summary>
        /// <param name="symbols">zbar_symbol_set_t。</param>
        /// <returns>集合中第一个解码符号；集合为空时返回 NULL（zbar_symbol_t）。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_symbol_set_first_symbol(IntPtr symbols);

        /// <summary>
        /// 获取过滤前的第一个原始结果。
        /// </summary>
        /// <param name="symbols">zbar_symbol_set_t。</param>
        /// <returns>过滤前集合中的第一个解码符号；集合为空时返回 NULL（zbar_symbol_t）。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_symbol_set_first_unfiltered(IntPtr symbols);
        #endregion 符号集合接口

        #region 图像接口
        /// <summary>
        /// 新建图像对象。
        /// </summary>
        /// <returns>
        /// 数据与格式均未初始化的新图像。应用不再使用时必须调用
        /// zbar_image_destroy() 销毁（zbar_image_t）。
        /// </returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_image_create();

        /// <summary>
        /// 销毁图像对象。
        /// </summary>
        /// <remarks>
        /// 如果设置了数据清理回调，销毁时会调用它。不要假设图像或数据缓冲区
        /// 会立即释放；库仍在使用时可能延后处理。如有需要，可通过清理回调
        /// 跟踪图像数据缓冲区。
        /// </remarks>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void zbar_image_destroy(IntPtr image);

        /// <summary>
        /// 调整图像引用计数。
        /// </summary>
        /// <remarks>
        /// 保存新的图像引用时增加计数，不再使用时减少计数；计数减少后不得再
        /// 访问该图像。zbar_image_ref(image, -1) 等价于
        /// zbar_image_destroy(image)。
        /// </remarks>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void zbar_image_ref(IntPtr image, int refs);

        /// <summary>
        /// 转换图像格式。支持的格式请参考文档。
        /// </summary>
        /// <returns>
        /// 新图像，包含原图像转换到目标格式后的采样数据；原图像不受影响。
        /// </returns>
        /// <remarks>受格式约束，转换后的尺寸可能向上取整。</remarks>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_image_convert(IntPtr image, uint format);

        /// <summary>
        /// 转换图像格式并按指定尺寸裁剪/填充。
        /// </summary>
        /// <remarks>
        /// 目标尺寸更大时复制最后一行/列补齐；目标尺寸更小时丢弃右侧/底部
        /// 多出的行/列。图像不会被缩放。
        /// </remarks>
        /// <returns>转换并调整尺寸后的新图像。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_image_convert_resize(IntPtr image, uint format, uint width, uint height);

        /// <summary>
        /// 获取图像格式。
        /// </summary>
        /// <returns>描述采样数据格式的 FourCC。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern uint zbar_image_get_format(IntPtr image);

        /// <summary>
        /// 获取与图像关联的“序列号”（页号/帧号）。
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern uint zbar_image_get_sequence(IntPtr image);

        /// <summary>
        /// 获取图像宽度。
        /// </summary>
        /// <returns>采样列数。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern uint zbar_image_get_width(IntPtr image);

        /// <summary>
        /// 获取图像高度。
        /// </summary>
        /// <returns>采样行数。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern uint zbar_image_get_height(IntPtr image);

        /// <summary>
        /// 获取图像宽高。
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern unsafe void zbar_image_get_size(IntPtr image, uint* width, uint* height);

        /// <summary>
        /// 获取裁剪矩形。
        /// </summary>
        /// <remarks>
        /// 返回将要扫描的轴对齐矩形的左上角坐标和尺寸，默认为整幅图像。
        /// </remarks>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern unsafe void zbar_image_get_crop(IntPtr image, uint* x, uint* y, uint* width, uint* height);

        /// <summary>
        /// 获取图像采样数据指针。
        /// </summary>
        /// <remarks>返回的数据缓冲区仅在 zbar_image_destroy() 之前有效。</remarks>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_image_get_data(IntPtr image);

        /// <summary>
        /// 获取图像数据长度。
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern uint zbar_image_get_data_length(IntPtr img);

        /// <summary>
        /// 获取解码结果。
        /// </summary>
        /// <returns>
        /// 解码符号集合（可能为空）；图像尚未扫描时返回 NULL
        /// （zbar_symbol_set_t）。
        /// </returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_image_get_symbols(IntPtr image);

        /// <summary>
        /// 关联指定符号集合到图像，替换已有结果。
        /// </summary>
        /// <remarks>
        /// 传 NULL 可释放图像当前结果。参见
        /// zbar_image_scanner_recycle_image。
        /// </remarks>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void zbar_image_set_symbols(IntPtr image, IntPtr symbols);

        /// <summary>
        /// 获取图像的第一个解码符号。
        /// </summary>
        /// <returns>第一个符号；没有结果时返回 NULL。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_image_first_symbol(IntPtr image);

        /// <summary>
        /// 设置图像采样数据的 FourCC 格式码。
        /// </summary>
        /// <remarks>该操作不转换数据；如需转换请调用 zbar_image_convert()。</remarks>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void zbar_image_set_format(IntPtr image, uint format);

        /// <summary>
        /// 设置与图像关联的“序列号”（页号/帧号）。
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void zbar_image_set_sequence(IntPtr image, uint sequence_num);

        /// <summary>
        /// 设置图像像素尺寸。
        /// </summary>
        /// <remarks>该操作不影响图像数据。</remarks>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void zbar_image_set_size(IntPtr image, uint width, uint height);

        /// <summary>
        /// 设置要扫描的图像矩形区域。
        /// </summary>
        /// <remarks>
        /// 矩形会被裁剪到图像边界内；默认为 zbar_image_set_size() 指定的
        /// 整幅图像。
        /// </remarks>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void zbar_image_set_crop(IntPtr image, uint x, uint y, uint width, uint height);

        /// <summary>
        /// 图像数据清理回调。
        /// </summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void zbar_image_cleanup_handler(IntPtr image);

        /// <summary>
        /// 设置图像采样数据。
        /// </summary>
        /// <remarks>
        /// 库不再需要数据时会调用指定的清理回调（除非为 NULL）。库不会修改
        /// 应用传入的图像数据。
        /// </remarks>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void zbar_image_set_data(IntPtr image, IntPtr data, uint data_byte_length, zbar_image_cleanup_handler cleanup_handler);

        /// <summary>
        /// 内置清理回调：把图像数据缓冲区交给 free()。
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void zbar_image_free_data(IntPtr image);

        /// <summary>
        /// 关联用户自定义数据到图像。
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void zbar_image_set_userdata(IntPtr image, IntPtr userdata);

        /// <summary>
        /// 获取图像关联的用户自定义数据。
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_image_get_userdata(IntPtr image);

        /// <summary>
        /// 把原始图像数据导出到文件，用于调试。
        /// </summary>
        /// <param name="filebase">
        /// 基础文件名；实际文件会追加 ".XXXX.zimg"，其中 XXXX 为格式的
        /// FourCC。
        /// </param>
        /// <returns>成功返回 0。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int zbar_image_write(IntPtr image, string filebase);
        #endregion 图像接口

        #region 图像扫描器接口
        /// <summary>
        /// 新建图像扫描器。
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_image_scanner_create();

        /// <summary>
        /// 销毁图像扫描器。
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void zbar_image_scanner_destroy(IntPtr scanner);

        /// <summary>
        /// 图像结果回调：图像解码出新结果时调用。
        /// </summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void zbar_image_data_handler(IntPtr image, IntPtr userdata);

        /// <summary>
        /// 设置结果回调。
        /// </summary>
        /// <remarks>
        /// 扫描器解码出新结果时会调用指定函数；传 NULL 可禁用回调。
        /// </remarks>
        /// <returns>之前注册的回调。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_image_scanner_set_data_handler(IntPtr scanner, zbar_image_data_handler handler, IntPtr userdata);

        /// <summary>
        /// 请求通过 D-Bus 发送解码结果。
        /// </summary>
        /// <remarks>参见 zbar_processor_parse_config()。</remarks>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int zbar_image_scanner_request_dbus(IntPtr scanner, int req_dbus_enabled);

        /// <summary>
        /// 为指定符号类型（传 0 表示全部）设置配置值。
        /// </summary>
        /// <param name="symbology">符号类型（SymbolType）。</param>
        /// <param name="config">配置项（Config）。</param>
        /// <returns>
        /// 成功返回 0；失败返回非 0（配置不适用于该符号类型，或值越界）。
        /// </returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int zbar_image_scanner_set_config(IntPtr scanner, int symbology, int config, int value);

        /// <summary>
        /// 获取指定符号类型的配置值。
        /// </summary>
        /// <param name="symbology">符号类型（SymbolType）。</param>
        /// <param name="config">配置项（Config）。</param>
        /// <returns>成功返回 0，并在 value 中写入配置值。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern unsafe int zbar_image_scanner_get_config(IntPtr scanner, int symbology, int config, int* value);

        /// <summary>
        /// 启用或禁用帧间结果缓存（默认禁用）。
        /// </summary>
        /// <remarks>
        /// 主要用于视频帧扫描。缓存会过滤连续图像中的重复结果，同时增加
        /// 一致性检查和迟滞；该接口也会清空缓存。
        /// </remarks>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void zbar_image_scanner_enable_cache(IntPtr scanner, int enable);

        /// <summary>
        /// 从图像扫描器和指定图像中移除之前解码出的结果。
        /// </summary>
        /// <remarks>
        /// 相当于更高效的 zbar_image_set_symbols(image, NULL)，可能保留内存
        /// 供后续解码复用。
        /// </remarks>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void zbar_image_scanner_recycle_image(IntPtr scanner, IntPtr image);

        /// <summary>
        /// 获取最近一次扫描图像的解码结果。
        /// </summary>
        /// <remarks>
        /// 返回的符号集合不会调整引用计数；如果结果需要在下次扫描后继续
        /// 使用，必须自行增加引用计数。
        /// </remarks>
        /// <returns>zbar_symbol_set_t。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_image_scanner_get_results(IntPtr scanner);

        /// <summary>
        /// 扫描指定图像中的符号。图像格式必须是 "Y800" 或 "GRAY"。
        /// </summary>
        /// <returns>
        /// 成功解码出符号时大于 0；未找到符号返回 0；发生错误返回 -1。
        /// </returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int zbar_scan_image(IntPtr scanner, IntPtr image);
        #endregion 图像扫描器接口

        #region 解码器接口
        /// <summary>
        /// 新建解码器。
        /// </summary>
        /// <returns>zbar_decoder_t。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_decoder_create();

        /// <summary>
        /// 销毁解码器。
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void zbar_decoder_destroy(IntPtr decoder);

        /// <summary>
        /// 为指定符号类型（传 0 表示全部）设置配置值。
        /// </summary>
        /// <param name="symbology">符号类型（SymbolType）。</param>
        /// <param name="config">配置项（Config）。</param>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int zbar_decoder_set_config(IntPtr decoder, int symbology, int config, int value);

        /// <summary>
        /// 获取指定符号类型的配置值。
        /// </summary>
        /// <param name="symbology">符号类型（SymbolType）。</param>
        /// <param name="config">配置项（Config）。</param>
        /// <returns>成功返回 0。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern unsafe int zbar_decoder_get_config(IntPtr decoder, int symbology, int config, int* value);

        /// <summary>
        /// 获取指定符号类型的布尔配置位掩码。
        /// </summary>
        /// <param name="symbology">符号类型（SymbolType）。</param>
        /// <returns>当前已启用配置的位掩码。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern uint zbar_decoder_get_configs(IntPtr decoder, int symbology);

        /// <summary>
        /// 清除解码器全部状态，并冲刷所有未完成的符号。
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void zbar_decoder_reset(IntPtr decoder);

        /// <summary>
        /// 标记新一轮扫描开始。
        /// </summary>
        /// <remarks>
        /// 清除符号内部状态并将颜色重置为 ZBAR_SPACE；已经部分解码的符号
        /// 状态保留。
        /// </remarks>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void zbar_decoder_new_scan(IntPtr decoder);

        /// <summary>
        /// 处理输入流中的下一个条/空宽度。
        /// </summary>
        /// <remarks>
        /// 宽度使用任意相对单位；一次扫描的第一个值是 ZBAR_SPACE 宽度，
        /// 之后条和空交替出现。
        /// </remarks>
        /// <returns>
        /// zbar_symbol_type_t（SymbolType）：宽度完成符号解码时返回对应符号
        /// 类型（可读取数据）；部分解码时返回 PARTIAL；没有新数据时返回
        /// NONE（0）。
        /// </returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int zbar_decode_width(IntPtr decoder, uint width);

        /// <summary>
        /// 获取下一个传给 zbar_decode_width() 的元素的颜色。
        /// </summary>
        /// <returns>zbar_color_t（Color）。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int zbar_decoder_get_color(IntPtr decoder);

        /// <summary>
        /// 获取最近一次解码出的数据。
        /// </summary>
        /// <returns>数据字符串；没有新数据时返回 NULL。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_decoder_get_data(IntPtr decoder);

        /// <summary>
        /// 获取解码数据的长度。
        /// </summary>
        /// <returns>解码数据的字节长度；没有新数据时返回 0。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern uint zbar_decoder_get_data_length(IntPtr decoder);

        /// <summary>
        /// 获取最近一次解码出的符号类型。
        /// </summary>
        /// <returns>zbar_symbol_type_t（SymbolType）。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int zbar_decoder_get_type(IntPtr decoder);

        /// <summary>
        /// 获取最近一次解码符号的修饰符位掩码。
        /// </summary>
        /// <returns>解码期间检测到的特征位掩码。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern uint zbar_decoder_get_modifiers(IntPtr decoder);

        /// <summary>
        /// 获取最近一次解码方向。
        /// </summary>
        /// <returns>1 表示正向，-1 表示反向，0 表示未知或不适用。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int zbar_decoder_get_direction(IntPtr decoder);

        /// <summary>
        /// 解码器数据回调：刚解码出新数据时调用。
        /// </summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void zbar_decoder_handler(IntPtr decoder);

        /// <summary>
        /// 设置数据回调。
        /// </summary>
        /// <remarks>
        /// 注册的函数会在 zbar_decode_width() 返回非 0 值之前由解码器调用；
        /// 传 NULL 可禁用回调。
        /// </remarks>
        /// <returns>之前注册的回调。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_decoder_set_handler(IntPtr decoder, zbar_decoder_handler handler);

        /// <summary>
        /// 关联用户自定义数据到解码器。
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void zbar_decoder_set_userdata(IntPtr decoder, IntPtr userdata);

        /// <summary>
        /// 获取解码器关联的用户自定义数据。
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_decoder_get_userdata(IntPtr decoder);
        #endregion 解码器接口

        #region 扫描器接口
        /// <summary>
        /// 新建扫描器。
        /// </summary>
        /// <param name="decoder">要关联的解码器；不关联时传 IntPtr.Zero。</param>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern IntPtr zbar_scanner_create(IntPtr decoder);

        /// <summary>
        /// 销毁扫描器。
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void zbar_scanner_destroy(IntPtr scanner);

        /// <summary>
        /// 清除扫描器全部状态，同时重置关联的解码器。
        /// </summary>
        /// <returns>zbar_symbol_type_t（SymbolType）。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int zbar_scanner_reset(IntPtr scanner);

        /// <summary>
        /// 标记新一轮扫描开始，并把颜色重置为 ZBAR_SPACE。
        /// </summary>
        /// <remarks>
        /// 同时更新关联的解码器。不使用回调时，返回值应像 zbar_scan_y()
        /// 一样检查。调用前应先调用 zbar_scanner_flush() 至少两次，以免
        /// 丢失解码结果。
        /// </remarks>
        /// <returns>zbar_symbol_type_t（SymbolType）。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int zbar_scanner_new_scan(IntPtr scanner);

        /// <summary>
        /// 冲刷扫描器处理管线，把当前位置强制作为扫描边界。
        /// </summary>
        /// <remarks>
        /// 多次调用（最多 3 次）可完全冲刷解码器。不使用回调时，返回值应像
        /// zbar_scan_y() 一样检查。
        /// </remarks>
        /// <returns>zbar_symbol_type_t（SymbolType）。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int zbar_scanner_flush(IntPtr scanner);

        /// <summary>
        /// 处理下一个采样强度值，强度使用任意相对单位。
        /// </summary>
        /// <returns>
        /// zbar_symbol_type_t（SymbolType）：关联解码器时返回
        /// zbar_decode_width() 的结果；否则检测到新边缘时返回 PARTIAL，
        /// 没有新边缘时返回 NONE（0）。
        /// </returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int zbar_scan_y(IntPtr scanner, int y);

        /// <summary>
        /// 获取最近一次扫描的宽度。
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern uint zbar_scanner_get_width(IntPtr scanner);

        /// <summary>
        /// 获取最近一条边缘的采样位置。
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern uint zbar_scanner_get_edge(IntPtr scn, uint offset, int prec);

        /// <summary>
        /// 获取最近一次扫描的颜色。
        /// </summary>
        /// <returns>zbar_color_t（Color）。</returns>
        [DllImport(DllName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int zbar_scanner_get_color(IntPtr scanner);
        #endregion 扫描器接口
    }
}
