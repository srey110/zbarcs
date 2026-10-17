using System;
using System.Collections.Generic;
using System.Drawing;

namespace zbarcs
{
    /// <summary>
    /// 表示一个已解码的符号。
    /// </summary>
    /// <remarks>
    /// 构造对象时会从原生符号复制全部值，因此即使图像已被回收或销毁，
    /// 当前 Symbol 仍然有效。
    /// </remarks>
    public class Symbol
    {
        private readonly byte[] dataBytes;
        private readonly string data;
        private readonly int quality;
        private readonly int count;
        private readonly uint configs;
        private readonly uint modifiers;
        private readonly Orientation orientation;
        private readonly Point[] points;
        private readonly Symbol[] components;
        private readonly SymbolType type;

        internal Symbol(IntPtr symbol)
        {
            if (symbol == IntPtr.Zero)
            {
                throw new ArgumentException("Can't initialize a symbol from a null pointer.", "symbol");
            }

            IntPtr pData = NativeZBar.zbar_symbol_get_data(symbol);
            uint length = NativeZBar.zbar_symbol_get_data_length(symbol);
            this.dataBytes = Interop.CopyBytes(pData, length);
            this.data = Interop.DecodeText(this.dataBytes);

            this.type = (SymbolType)NativeZBar.zbar_symbol_get_type(symbol);
            this.quality = NativeZBar.zbar_symbol_get_quality(symbol);
            this.count = NativeZBar.zbar_symbol_get_count(symbol);
            this.configs = NativeZBar.zbar_symbol_get_configs(symbol);
            this.modifiers = NativeZBar.zbar_symbol_get_modifiers(symbol);
            this.orientation = (Orientation)NativeZBar.zbar_symbol_get_orientation(symbol);

            uint pointCount = NativeZBar.zbar_symbol_get_loc_size(symbol);
            if (pointCount > int.MaxValue)
            {
                throw new OutOfMemoryException("The symbol location contains too many points.");
            }

            this.points = new Point[(int)pointCount];
            for (uint i = 0; i < pointCount; i++)
            {
                this.points[(int)i] = new Point(
                    NativeZBar.zbar_symbol_get_loc_x(symbol, i),
                    NativeZBar.zbar_symbol_get_loc_y(symbol, i));
            }

            IntPtr componentSet = NativeZBar.zbar_symbol_get_components(symbol);
            if (componentSet == IntPtr.Zero)
            {
                this.components = new Symbol[0];
            }
            else
            {
                List<Symbol> componentList = new List<Symbol>();
                foreach (Symbol component in SymbolEnumerable.FromSet(componentSet))
                {
                    componentList.Add(component);
                }
                this.components = componentList.ToArray();
            }
        }

        /// <summary>
        /// 解码出的数据，尽可能按 UTF-8 解码。
        /// </summary>
        public string Data
        {
            get
            {
                return this.data;
            }
        }

        /// <summary>
        /// 解码出的原始字节。
        /// </summary>
        /// <remarks>
        /// 当负载是二进制数据时（例如启用了 <see cref="Config.Binary"/>），
        /// 应使用本属性而不是 <see cref="Data"/>。
        /// </remarks>
        public byte[] DataBytes
        {
            get
            {
                return (byte[])this.dataBytes.Clone();
            }
        }

        /// <summary>
        /// 解码数据的长度（字节）。
        /// </summary>
        public int DataLength
        {
            get
            {
                return this.dataBytes.Length;
            }
        }

        /// <summary>
        /// 符号置信度指标。数值越大越好，具体含义由应用决定。
        /// </summary>
        public int Quality
        {
            get
            {
                return this.quality;
            }
        }

        /// <summary>
        /// 当前缓存计数。
        /// </summary>
        /// <remarks>
        /// 启用图像扫描器缓存后，该值提供视频流中帧间可靠性和冗余信息：
        /// 小于 0 表示符号仍不确定，0 表示符号刚被确认，大于 0 表示重复符号。
        /// </remarks>
        public int Count
        {
            get
            {
                return this.count;
            }
        }

        /// <summary>
        /// 解码期间为该符号类型设置的配置位掩码。
        /// </summary>
        public uint Configs
        {
            get
            {
                return this.configs;
            }
        }

        /// <summary>
        /// 解码期间检测到的特征位掩码。
        /// </summary>
        public uint Modifiers
        {
            get
            {
                return this.modifiers;
            }
        }

        /// <summary>
        /// 符号的粗略方向。
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return this.orientation;
            }
        }

        /// <summary>
        /// 解码出该符号时的扫描位置点。
        /// </summary>
        public Point[] Points
        {
            get
            {
                return (Point[])this.points.Clone();
            }
        }

        /// <summary>
        /// 复合结果中包含的物理符号组件。
        /// </summary>
        /// <remarks>
        /// 如果本身已经是物理符号，则返回空数组。
        /// </remarks>
        public Symbol[] Components
        {
            get
            {
                return (Symbol[])this.components.Clone();
            }
        }

        /// <summary>
        /// 解码出的符号类型。
        /// </summary>
        public SymbolType Type
        {
            get
            {
                return this.type;
            }
        }

        public override string ToString()
        {
            return this.type.ToString() + " " + this.data;
        }
    }

    /// <summary>
    /// 遍历原生 zbar_symbol_set_t 并复制其中的每个符号。
    /// </summary>
    internal static class SymbolEnumerable
    {
        internal static IEnumerable<Symbol> FromSet(IntPtr symbols)
        {
            if (symbols == IntPtr.Zero)
            {
                yield break;
            }

            IntPtr symbol = NativeZBar.zbar_symbol_set_first_symbol(symbols);
            while (symbol != IntPtr.Zero)
            {
                yield return new Symbol(symbol);
                symbol = NativeZBar.zbar_symbol_next(symbol);
            }
        }
    }
}
