using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zbarcs
{
    /// <summary>
    /// decoded symbol type. zbar_symbol_type_t
    /// </summary>
    [Flags]
    public enum SymbolType
    {
        /// <summary>
        /// No symbol decoded
        /// </summary>
        None = 0,
        /// <summary>
        /// Intermediate status
        /// </summary>
        Partial = 1,
        /// <summary>
        /// GS1 2-digit add-on
        /// </summary>
        EAN2 = 2,
        /// <summary>
        /// GS1 5-digit add-on
        /// </summary>
        EAN5 = 5,
        /// <summary>
        /// EAN-8
        /// </summary>
        EAN8 = 8,
        /// <summary>
        /// UPC-E
        /// </summary>
        UPCE = 9,
        /// <summary>
        /// ISBN-10 (from EAN-13)
        /// </summary>
        ISBN10 = 10,
        /// <summary>
        /// UPC-A
        /// </summary>
        UPCA = 12,
        /// <summary>
        /// EAN-13
        /// </summary>
        EAN13 = 13,
        /// <summary>
        /// ISBN-13 (from EAN-13)
        /// </summary>
        ISBN13 = 14,
        /// <summary>
        /// EAN/UPC composite
        /// </summary>
        Composite = 15,
        /// <summary>
        /// Interleaved 2 of 5.
        /// </summary>
        I25 = 25,
        /// <summary>
        /// GS1 DataBar (RSS). @since 0.11
        /// </summary>
        DataBar = 34,
        /// <summary>
        /// GS1 DataBar Expanded. @since 0.11
        /// </summary>
        DataBar_EXP = 35,
        /// <summary>
        /// Codabar. @since 0.11
        /// </summary>
        Codabar = 38,
        /// <summary>
        /// Code 39.
        /// </summary>
        CODE39 = 39,
        /// <summary>
        /// PDF417
        /// </summary>
        PDF417 = 57,
        /// <summary>
        /// QR Code
        /// </summary>
        QRCODE = 64,
        /// <summary>
        /// SQ Code. @since 0.20.1
        /// </summary>
        SQCODE = 80,
        /// <summary>
        /// Code 93. @since 0.11
        /// </summary>
        CODE93 = 93,
        /// <summary>
        /// Code 128
        /// </summary>
        CODE128 = 128,
        /// <summary>
        /// mask for base symbol type
        /// </summary>
        Symbole = 0x00ff,
        /// <summary>
        /// 2-digit add-on flag
        /// </summary>
        Addon2 = 0x0200,
        /// <summary>
        /// 5-digit add-on flag
        /// </summary>
        Addon5 = 0x0500,
        /// <summary>
        /// add-on flag mask
        /// </summary>
        Addon = 0x0700
    }
    /// <summary>
    /// decoded symbol coarse orientation.zbar_orientation_t
    /// </summary>
    public enum Orientation
    {
        /// <summary>
        /// unable to determine orientation
        /// </summary>
        Unknown = -1,
        /// <summary>
        /// upright, read left to right
        /// </summary>
        Up,
        /// <summary>
        /// sideways, read top to bottom
        /// </summary>
        Right,
        /// <summary>
        /// upside-down, read right to left
        /// </summary>
        Down,
        /// <summary>
        /// sideways, read bottom to top
        /// </summary>
        Left
    }
    /// <summary>
    /// Error codes.zbar_error_t
    /// </summary>
    /// <remarks>
    /// The ordering matches zbar_error_t from zbar.h
    /// </remarks>
    public enum ZBarError
    {
        /// <summary>
        /// No error, or zbar is not aware of the error
        /// </summary>
        Ok = 0,
        /// <summary>
        /// Out of memory
        /// </summary>
        OutOfMemory,
        /// <summary>
        /// Internal library error
        /// </summary>
        InternalLibraryError,
        /// <summary>
        /// Unsupported request
        /// </summary>
        Unsupported,
        /// <summary>
        /// Invalid request
        /// </summary>
        InvalidRequest,
        /// <summary>
        /// System error
        /// </summary>
        SystemError,
        /// <summary>
        /// Locking error
        /// </summary>
        LockingError,
        /// <summary>
        /// All resources busy
        /// </summary>
        AllResourcesBusyError,
        /// <summary>
        /// X11 display error
        /// </summary>
        X11DisplayError,
        /// <summary>
        /// X11 Protocol error
        /// </summary>
        X11ProtocolError,
        /// <summary>
        /// Output window closed
        /// </summary>
        OutputWindowClosed,
        /// <summary>
        /// Windows system error
        /// </summary>
        WindowsAPIError
    }
    /// <summary>
    /// decoder configuration options.zbar_config_t
    /// </summary>
    public enum Config
    {
        /// <summary>
        /// Enable symbology/feature
        /// </summary>
        Enable = 0,
        /// <summary>
        /// Enable check digit when optional
        /// </summary>
        AddCheck,
        /// <summary>
        /// Return check digit when present
        /// </summary>
        EmitCheck,
        /// <summary>
        /// Enable full ASCII character set
        /// </summary>
        ASCII,
        /// <summary>
        /// don't convert binary data to text
        /// </summary>
        Binary,
        /// <summary>
        /// Number of boolean decoder configs
        /// </summary>
        Num,
        /// <summary>
        /// Minimum data length for valid decode
        /// </summary>
        MinimumLength = 0x20,
        /// <summary>
        /// Maximum data length for valid decode
        /// </summary>
        MaximumLength,
        /// <summary>
        /// required video consistency frames
        /// </summary>
        Uncertainty = 0x40,
        /// <summary>
        /// Enable scanner to collect position data
        /// </summary>
        Position = 0x80,
        /// <summary>
        /// if fails to decode, test inverted
        /// </summary>
        TestInverted,
        /// <summary>
        /// Image scanner vertical scan density
        /// </summary>
        XDensity = 0x100,
        /// <summary>
        /// Image scanner horizontical scan density
        /// </summary>
        YDensity
    }
    /// <summary>
    /// decoder symbology modifier flags.zbar_modifier_t
    /// </summary>
    public enum Modifier
    {
        /// <summary>
        /// barcode tagged as GS1 (EAN.UCC) reserved
        ///(eg, FNC1 before first data character).
        ///data may be parsed as a sequence of GS1 AIs
        /// </summary>
        GS1 = 0,
        /// <summary>
        /// barcode tagged as AIM reserved
        ///(eg, FNC1 after first character or digit pair)
        /// </summary>
        AIM,
        /// <summary>
        /// number of modifiers
        /// </summary>
        NUM
    }
    /// <summary>
    /// video_control_type_t
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
    public enum Color
    {
        /// <summary>
        /// light area or space between bars
        /// </summary>
        Space = 0,
        /// <summary>
        /// dark area or colored bar segment
        /// </summary>
        Bar,
    }
}
