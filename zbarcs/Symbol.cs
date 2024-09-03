using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace zbarcs
{
    /// <summary>
    /// Representation of a decoded symbol
    /// </summary>
    /// <remarks>This symbol does not hold any references to unmanaged resources.</remarks>
    public class Symbol
    {
        private string data;
        private int quality;
        private int count;
        private uint configs;
        private uint modifiers;
        private Orientation ori;
        private Point[] points;
        private SymbolType type;
        internal Symbol(IntPtr symbol)
        {
            if (IntPtr.Zero == symbol)
            {
                throw new Exception("Can't initialize symbol from null pointer.");
            }
            //Get data from the symbol
            IntPtr pData = NativeZBar.zbar_symbol_get_data(symbol);
            int length = (int)NativeZBar.zbar_symbol_get_data_length(symbol);
            this.data = Marshal.PtrToStringAnsi(pData, length);
            //Get the other fields
            this.type = (SymbolType)NativeZBar.zbar_symbol_get_type(symbol);
            this.quality = NativeZBar.zbar_symbol_get_quality(symbol);
            this.count = NativeZBar.zbar_symbol_get_count(symbol);
            this.configs = NativeZBar.zbar_symbol_get_configs(symbol);
            this.modifiers = NativeZBar.zbar_symbol_get_modifiers(symbol);
            this.ori = (Orientation)NativeZBar.zbar_symbol_get_orientation(symbol);
            //取多边形上的点
            uint size = NativeZBar.zbar_symbol_get_loc_size(symbol);
            points = new Point[size];
            for (uint i = 0; i < size; i++)
            {
                points[i].X = NativeZBar.zbar_symbol_get_loc_x(symbol, i);
                points[i].Y = NativeZBar.zbar_symbol_get_loc_y(symbol, i);
            }
        }
        public override string ToString()
        {
            return this.type.ToString() + " " + this.data;
        }
        /// <summary>
        /// a bitmask indicating which characteristics were detected during decoding.Config
        /// </summary>
        public uint Configs
        {
            get
            {
                return this.configs;
            }
        }
        /// <summary>
        /// a bitmask indicating which characteristics were detected during decoding. Modifier
        /// </summary>
        public uint Modifiers
        {
            get
            {
                return this.modifiers;
            }
        }
        public Point[] Points
        {
            get
            {
                return this.points;
            }
        }
        /// <summary>
        /// retrieve general orientation of decoded symbol
        /// </summary>
        public Orientation ORI
        {
            get
            {
                return this.ori;
            }
        }
        /// <value>
        /// Retrieve current cache count.
        /// </value>
        /// <remarks>
        /// When the cache is enabled for the image_scanner this provides inter-frame reliability and redundancy information for video streams.
        /// 	&lt; 0 if symbol is still uncertain.
        /// 	0 if symbol is newly verified.
        /// 	&gt; 0 for duplicate symbols
        /// </remarks>
        public int Count
        {
            get
            {
                return this.count;
            }
        }
        /// <value>
        /// Data decoded from symbol.
        /// </value>
        public string Data
        {
            get
            {
                return this.data;
            }
        }
        /// <value>
        /// Get a symbol confidence metric.
        /// </value>
        /// <remarks>
        /// An unscaled, relative quantity: larger values are better than smaller values, where "large" and "small" are application dependent.
        /// </remarks>
        public int Quality
        {
            get
            {
                return this.quality;
            }
        }
        /// <value>
        /// Type of decoded symbol
        /// </value>
        public SymbolType Type
        {
            get
            {
                return this.type;
            }
        }
    }
}