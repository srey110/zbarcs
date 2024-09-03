using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

/// <summary>
/// ZBar is a library for reading bar codes from video streams
/// </summary>
namespace zbarcs
{
    internal static class NativeZBar
    {
        private const string DllName = "libzbar-0";
        #region global
        /// <summary>
        /// retrieve runtime library version information.
        /// </summary>
        /// <param name="major">set to the running major version (unless NULL)</param>
        /// <param name="minor">set to the running minor version (unless NULL)</param>
        /// <param name="patch">set to the running patch version (unless NULL)</param>
        /// <returns>0 successfully</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int zbar_version(uint* major, uint* minor, uint* patch);
        /// <summary>
        /// set global library debug level.
        /// </summary>
        /// <param name="verbosity">desired debug level.  higher values create more spew</param>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void zbar_set_verbosity(int verbosity);
        /// <summary>
        /// increase global library debug level.eg, for -vvvv
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void zbar_increase_verbosity();
        /// <summary>
        /// retrieve string name for symbol encoding.
        /// </summary>
        /// <param name="sym">symbol type encoding(SymbolType)</param>
        /// <returns>the static string name for the specified symbol type,or "UNKNOWN" if the encoding is not recognized</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zbar_get_symbol_name(int sym);
        /// <summary>
        /// retrieve string name for addon encoding.
        /// </summary>
        /// <param name="sym">symbol type encoding(SymbolType)</param>
        /// <returns>static string name for any addon, or the empty string if no addons were decoded</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zbar_get_addon_name(int sym);
        /// <summary>
        /// retrieve string name for configuration setting.
        /// </summary>
        /// <param name="config">setting to name(Config)</param>
        /// <returns>static string name for config, or the empty string if value is not a known config</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zbar_get_config_name(int config);
        /// <summary>
        /// retrieve string name for modifier.
        /// </summary>
        /// <param name="modifier">flag to name(Modifier)</param>
        /// <returns>static string name for modifier,or the empty string if the value is not a known flag</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zbar_get_modifier_name(int modifier);
        /// <summary>
        /// retrieve string name for orientation.
        /// </summary>
        /// <param name="orientation">orientation encoding(Orientation)</param>
        /// <returns></returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zbar_get_orientation_name(int orientation);
        /// <summary>
        /// parse a configuration string of the form "[symbology.]config[=value]".the config must match one of the recognized names.
        /// </summary>
        /// <param name="config_string">config string</param>
        /// <param name="symbology">if present, must match one of the recognized names,if symbology is unspecified, it will be set to 0.(SymbolType)</param>
        /// <param name="config">Config</param>
        /// <param name="value">if value is unspecified it will be set to 1.</param>
        /// <returns>returns 0 if the config is parsed successfully</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int zbar_parse_config(string config_string, int* symbology, int* config, int* value);
        #endregion global

        #region Error interface
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr _zbar_error_string(IntPtr obj, int verbosity);
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int _zbar_get_error_code(IntPtr obj);
        #endregion Error interface

        #region Symbol interface
        /// <summary>
        /// symbol reference count manipulation.
        /// </summary>
        /// <remarks>
        /// increment the reference count when you store a new reference to the
        /// symbol.  decrement when the reference is no longer used.  do not
        /// refer to the symbol once the count is decremented and the
        /// containing image has been recycled or destroyed.
        /// the containing image holds a reference to the symbol, so you
        /// only need to use this if you keep a symbol after the image has been
        /// destroyed or reused.
        /// </remarks>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void zbar_symbol_ref(IntPtr symbol, int refs);
        /// <summary>
        /// retrieve type of decoded symbol.
        /// </summary>
        /// <returns>zbar_symbol_type_t SymbolType</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zbar_symbol_get_type(IntPtr symbol);
        /// <summary>
        /// retrieve symbology boolean config settings.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns>a bitmask indicating which configs were set for the detected symbology during decoding.</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint zbar_symbol_get_configs(IntPtr symbol);
        /// <summary>
        /// retrieve symbology modifier flag settings.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns>a bitmask indicating which characteristics were detected during decoding.</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint zbar_symbol_get_modifiers(IntPtr symbol);
        /// <summary>
        /// retrieve data decoded from symbol.
        /// </summary>
        /// <returns> the data string</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zbar_symbol_get_data(IntPtr symbol);
        /// <summary>
        /// retrieve length of binary data.
        /// </summary>
        /// <returns> the length of the decoded data</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint zbar_symbol_get_data_length(IntPtr symbol);
        /// <summary>
        /// retrieve a symbol confidence metric.
        /// </summary>
        /// <returns> an unscaled, relative quantity: larger values are better
        /// than smaller values, where "large" and "small" are application
        /// dependent.
        /// </returns>
        /// <remarks>expect the exact definition of this quantity to change as the
        /// metric is refined.  currently, only the ordered relationship
        /// between two values is defined and will remain stable in the future
        /// </remarks>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zbar_symbol_get_quality(IntPtr symbol);
        /// <summary>
        /// retrieve current cache count.
        /// </summary>
        /// <remarks>when the cache is enabled for the
        /// image_scanner this provides inter-frame reliability and redundancy
        /// information for video streams.
        /// </remarks>
        /// <returns>
        /// < 0 if symbol is still uncertain.
        /// 0 if symbol is newly verified.
        /// > 0 for duplicate symbols
        /// </returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zbar_symbol_get_count(IntPtr symbol);
        /// <summary>
        /// retrieve the number of points in the location polygon.  the
        /// location polygon defines the image area that the symbol was
        /// extracted from.
        /// </summary>
        /// <returns> the number of points in the location polygon</returns>
        /// <remarks>this is currently not a polygon, but the scan locations
        /// where the symbol was decoded</remarks>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint zbar_symbol_get_loc_size(IntPtr symbol);
        /// <summary>
        /// retrieve location polygon x-coordinates.
        /// points are specified by 0-based index.
        /// </summary>
        /// <returns> the x-coordinate for a point in the location polygon.
        /// -1 if index is out of range</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zbar_symbol_get_loc_x(IntPtr symbol, uint index);
        /// <summary>
        /// retrieve location polygon y-coordinates.
        /// points are specified by 0-based index.
        /// </summary>
        /// <returns> the y-coordinate for a point in the location polygon.
        ///  -1 if index is out of range</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zbar_symbol_get_loc_y(IntPtr symbol, uint index);
        /// <summary>
        /// retrieve general orientation of decoded symbol
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns>zbar_orientation_t Orientation</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zbar_symbol_get_orientation(IntPtr symbol);
        /// <summary>
        /// iterate the result set.
        /// </summary>
        /// <returns> the next result symbol, or
        /// NULL when no more results are available</returns>
        /// <remarks>Marked internal because it is used by the symbol iterators.</remarks>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zbar_symbol_next(IntPtr symbol);
        /// <summary>
        /// retrieve components of a composite result.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns>the symbol set containing the components, 
        /// NULL if the symbol is already a physical symbol (zbar_symbol_set_t)</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zbar_symbol_get_components(IntPtr symbol);
        /// <summary>
        /// iterate components of a composite result.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns>the first physical component symbol of a composite result,
        /// NULL if the symbol is already a physical symbol (zbar_symbol_t)</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zbar_symbol_first_component(IntPtr symbol);
        /// <summary>
        /// print XML symbol element representation to user result buffer.
        /// </summary>
        /// <remarks>see http://zbar.sourceforge.net/2008/barcode.xsd for the schema.</remarks>
        /// <param name="symbol">is the symbol to print</param>
        /// <param name="buffer"> is the inout result pointer, it will be reallocated
        /// with a larger size if necessary.</param>
        /// <param name="buflen">  is inout length of the result buffer.</param>
        /// <returns> the buffer pointer</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zbar_symbol_xml(IntPtr symbol, out IntPtr buffer, out uint buflen);
        #endregion Symbol interface

        #region Symbol Set interface
        /// <summary>
        /// reference count manipulation.
        /// increment the reference count when you store a new reference.
        /// decrement when the reference is no longer used.  do not refer to
        /// the object any longer once references have been released.
        /// </summary>
        /// <param name="symbol">zbar_symbol_set_t</param>
        /// <param name="refs"></param>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void zbar_symbol_set_ref(IntPtr symbols, int refs);
        /// <summary>
        /// retrieve set size.
        /// </summary>
        /// <param name="symbols">zbar_symbol_set_t</param>
        /// <returns>the number of symbols in the set</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zbar_symbol_set_get_size(IntPtr symbols);
        /// <summary>
        /// set iterator
        /// </summary>
        /// <param name="symbols">zbar_symbol_set_t</param>
        /// <returns>the first decoded symbol result in a set, NULL if the set is empty(zbar_symbol_t)</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zbar_symbol_set_first_symbol(IntPtr symbols);
        /// <summary>
        /// raw result iterator.
        /// </summary>
        /// <param name="symbols">zbar_symbol_set_t</param>
        /// <returns>the first decoded symbol result in a set, *before* filtering, NULL if the set is empty(zbar_symbol_t)</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zbar_symbol_set_first_unfiltered(IntPtr symbols);
        #endregion Symbol Set interface

        #region Image interface
        /// <summary>new image constructor.
        /// </summary>
        /// <returns>
        /// a new image object with uninitialized data and format.
        /// this image should be destroyed (using zbar_image_destroy()) as
        /// soon as the application is finished with it(zbar_image_t)
        /// </returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zbar_image_create();
        /// <summary>image destructor.  all images created by or returned to the
        /// application should be destroyed using this function.  when an image
        /// is destroyed, the associated data cleanup handler will be invoked
        /// if available
        /// </summary><remarks>
        /// make no assumptions about the image or the data buffer.
        /// they may not be destroyed/cleaned immediately if the library
        /// is still using them.  if necessary, use the cleanup handler hook
        /// to keep track of image data buffers
        /// </remarks>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void zbar_image_destroy(IntPtr image);
        /// <summary>image reference count manipulation.
        /// increment the reference count when you store a new reference to the
        /// image.  decrement when the reference is no longer used.  do not
        /// refer to the image any longer once the count is decremented.
        /// zbar_image_ref(image, -1) is the same as zbar_image_destroy(image)
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void zbar_image_ref(IntPtr image, int refs);
        /// <summary>image format conversion.  refer to the documentation for supported
        /// image formats
        /// </summary>
        /// <returns> a new image with the sample data from the original image
        /// converted to the requested format.  the original image is
        /// unaffected.
        /// </returns>
        /// <remarks> the converted image size may be rounded (up) due to format
        /// constraints
        /// </remarks>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zbar_image_convert(IntPtr image, uint format);
        /// <summary>image format conversion with crop/pad.
        /// if the requested size is larger than the image, the last row/column
        /// are duplicated to cover the difference.  if the requested size is
        /// smaller than the image, the extra rows/columns are dropped from the
        /// right/bottom.
        /// </summary>
        /// <returns> a new image with the sample data from the original
        /// image converted to the requested format and size.
        /// </returns>
        /// <remarks>the image is not scaled</remarks>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zbar_image_convert_resize(IntPtr image, uint format, uint width, uint height);
        /// <summary>retrieve the image format.
        /// </summary>
        /// <returns> the fourcc describing the format of the image sample data</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint zbar_image_get_format(IntPtr image);
        /// <summary>retrieve a "sequence" (page/frame) number associated with this image.
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint zbar_image_get_sequence(IntPtr image);
        /// <summary>retrieve the width of the image.
        /// </summary>
        /// <returns> the width in sample columns</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint zbar_image_get_width(IntPtr image);
        /// <summary>retrieve the height of the image.
        /// </summary>
        /// <returns> the height in sample rows</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint zbar_image_get_height(IntPtr image);
        /// <summary>
        /// retrieve both dimensions of the image.
        /// fills in the width and height in samples
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void zbar_image_get_size(IntPtr image, uint* width, uint* height);
        /// <summary>
        /// retrieve the crop rectangle.
        /// fills in the image coordinates of the upper left corner and size
        /// of an axis-aligned rectangular area of the image that will be scanned.
        /// defaults to the full image
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void zbar_image_get_crop(IntPtr image, uint* x, uint* y, uint* width, uint* height);
        /// <summary>return the image sample data.  the returned data buffer is only
        /// valid until zbar_image_destroy() is called
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zbar_image_get_data(IntPtr image);
        /// <summary>return the size of image data.
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint zbar_image_get_data_length(IntPtr img);
        /// <summary>
        /// retrieve the decoded results.
        /// </summary>
        /// <returns>the (possibly empty) set of decoded symbols,NULL if the image has not been scanned(zbar_symbol_set_t)</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zbar_image_get_symbols(IntPtr image);
        /// <summary>
        /// associate the specified symbol set with the image, replacing any
        /// existing results.  use NULL to release the current results from the image.see zbar_image_scanner_recycle_image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="symbols">zbar_symbol_set_t</param>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void zbar_image_set_symbols(IntPtr image, IntPtr symbols);
        /// <summary>image_scanner decode result iterator.
        /// </summary>
        /// <returns> the first decoded symbol result for an image
        /// or NULL if no results are available
        /// </returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zbar_image_first_symbol(IntPtr image);
        /// <summary>specify the fourcc image format code for image sample data.
        /// refer to the documentation for supported formats.
        /// </summary>
        /// <remarks> this does not convert the data!
        /// (see zbar_image_convert() for that)
        /// </remarks>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void zbar_image_set_format(IntPtr image, uint format);
        /// <summary>associate a "sequence" (page/frame) number with this image.
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void zbar_image_set_sequence(IntPtr image, uint sequence_num);
        /// <summary>specify the pixel size of the image.
        /// </summary>
        /// <remarks>this does not affect the data!</remarks>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void zbar_image_set_size(IntPtr image, uint width, uint height);
        /// <summary>
        /// specify a rectangular region of the image to scan.
        /// the rectangle will be clipped to the image boundaries.
        /// defaults to the full image specified by zbar_image_set_size()
        /// </summary>
        /// <param name="image"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void zbar_image_set_crop(IntPtr image, uint x, uint y, uint width, uint height);
        /// <summary>
        /// Cleanup handler callback for image data.
        /// </summary>
        public delegate void zbar_image_cleanup_handler(IntPtr image);
        /// <summary>specify image sample data.  when image data is no longer needed by
        /// the library the specific data cleanup handler will be called
        /// (unless NULL)
        /// </summary>
        /// <remarks>application image data will not be modified by the library</remarks>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void zbar_image_set_data(IntPtr image, IntPtr data, uint data_byte_length, zbar_image_cleanup_handler cleanup_handler);
        /// <summary>built-in cleanup handler.
        /// passes the image data buffer to free()
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void zbar_image_free_data(IntPtr image);
        /// <summary>associate user specified data value with an image.
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void zbar_image_set_userdata(IntPtr image, IntPtr userdata);
        /// <summary>return user specified data value associated with the image.
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zbar_image_get_userdata(IntPtr image);
        /// <summary>
        /// dump raw image data to a file for debug.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="filebase">base filename, appended with ".XXXX.zimg" where XXXX is the format fourcc</param>
        /// <returns>0 on success</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zbar_image_write(IntPtr image, string filebase);
        /// <summary>
        /// read back an image in the format written by zbar_image_write()
        /// </summary>
        /// <returns>zbar_image_t</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zbar_image_read(string filename);
        #endregion Image interface

        #region Image Scanner interface
        /// <summary>
        /// Constructor
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zbar_image_scanner_create();
        /// <summary>
        /// Destructor.
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void zbar_image_scanner_destroy(IntPtr scanner);
        /// <summary>
        /// data handler callback function.
        /// called when decoded symbol results are available for an image
        /// </summary>
        public delegate void zbar_image_data_handler(IntPtr image, IntPtr userdata);
        /// <summary>
        /// setup result handler callback.
        /// the specified function will be called by the scanner whenever
        /// new results are available from a decoded image.
        /// pass a NULL value to disable callbacks.
        /// </summary>
        /// <returns>the previously registered handler</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern zbar_image_data_handler zbar_image_scanner_set_data_handler(IntPtr scanner, zbar_image_data_handler handler, IntPtr userdata);
        /// <summary>
        /// request sending decoded codes via D-Bus
        /// see zbar_processor_parse_config()
        /// </summary>
        /// <returns></returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zbar_image_scanner_request_dbus(IntPtr scanner, int req_dbus_enabled);
        /// <summary>
        /// set config for indicated symbology (0 for all) to specified value.
        /// </summary>
        /// <param name="symbology">zbar_symbol_type_t SymbolType</param>
        /// <param name="config">zbar_config_t Config</param>
        /// <returns>0 for success, non-0 for failure (config does not apply to
        /// specified symbology, or value out of range)
        /// </returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zbar_image_scanner_set_config(IntPtr scanner, int symbology, int config, int value);
        /// <summary>
        /// get config for indicated symbology
        /// </summary>
        /// <param name="symbology">zbar_symbol_type_t SymbolType</param>
        /// <param name="config">zbar_config_t Config</param>
        /// <returns>0 for success</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int zbar_image_scanner_get_config(IntPtr scanner, int symbology, int config, int* value);
        /// <summary>
        /// enable or disable the inter-image result cache (default disabled).
        /// mostly useful for scanning video frames, the cache filters
        /// duplicate results from consecutive images, while adding some
        /// consistency checking and hysteresis to the results.
        /// this interface also clears the cache
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void zbar_image_scanner_enable_cache(IntPtr scanner, int enable);
        /// <summary>
        /// remove any previously decoded results from the image scanner and the
        /// specified image.  somewhat more efficient version of
        /// zbar_image_set_symbols(image, NULL) which may retain memory for subsequent decodes
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void zbar_image_scanner_recycle_image(IntPtr scanner, IntPtr image);
        /// <summary>
        /// retrieve decode results for last scanned image.
        /// he symbol set does not have its reference count adjusted;
        /// ensure that the count is incremented if the results may be kept
        /// after the next image is scanned
        /// </summary>
        /// <param name="scanner"></param>
        /// <returns>zbar_symbol_set_t</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zbar_image_scanner_get_results(IntPtr scanner);
        /// <summary>
        /// scan for symbols in provided image.  The image format must be
        /// "Y800" or "GRAY".
        /// </summary>
        /// <returns>
        ///  > 0 if symbols were successfully decoded from the image,
        /// 0 if no symbols were found or -1 if an error occurs
        /// </returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zbar_scan_image(IntPtr scanner, IntPtr image);
        #endregion Image Scanner interface

        #region Decoder interface
          /// <summary>
        /// constructor
        /// </summary>
        /// <returns>zbar_decoder_t</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zbar_decoder_create();
        /// <summary>
        /// destructor
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void zbar_decoder_destroy(IntPtr decoder);
        /// <summary>
        /// set config for indicated symbology (0 for all) to specified value.
        /// </summary>
        /// <param name="symbology">zbar_symbol_type_t SymbolType</param>
        /// <param name="config">zbar_config_t Config</param>
        /// <returns></returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zbar_decoder_set_config(IntPtr decoder, int symbology, int config, int value);
        /// <summary>
        /// get config for indicated symbology
        /// </summary>
        /// <param name="symbology">zbar_symbol_type_t SymbolType</param>
        /// <param name="config">zbar_config_t Config</param>
        /// <returns>0 for success</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int zbar_decoder_get_config(IntPtr decoder, int symbology, int config, int* value);
        /// <summary>
        /// retrieve symbology boolean config settings.
        /// </summary>
        /// <param name="symbology">zbar_symbol_type_t SymbolType</param>
        /// <returns>a bitmask indicating which configs are currently set for the specified symbology</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint zbar_decoder_get_configs(IntPtr decoder, int symbology);
        /// <summary>
        /// clear all decoder state.
        /// any partial symbols are flushed
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void zbar_decoder_reset(IntPtr decoder);
        /// <summary>
        /// mark start of a new scan pass.
        /// clears any intra-symbol state and resets color to ::ZBAR_SPACE.
        /// any partially decoded symbol state is retained
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void zbar_decoder_new_scan(IntPtr decoder);
        /// <summary>
        /// process next bar/space width from input stream.
        /// the width is in arbitrary relative units.  first value of a scan
        /// is ::ZBAR_SPACE width, alternating from there.
        /// </summary>
        /// <returns>
        /// zbar_symbol_type_t SymbolType
        /// appropriate symbol type if width completes
        /// decode of a symbol (data is available for retrieval)
        /// PARTIAL as a hint if part of a symbol was decoded
        /// NONE (0) if no new symbol data is available
        /// </returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zbar_decode_width(IntPtr decoder, uint width);
        /// <summary>
        /// retrieve color of @em next element passed to zbar_decode_width()
        /// </summary>
        /// <param name="decoder"></param>
        /// <returns>zbar_color_t Color</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zbar_decoder_get_color(IntPtr decoder);
        /// <summary>
        /// retrieve last decoded data.
        /// </summary>
        /// <param name="decoder"></param>
        /// <returns>the data string or NULL if no new data available.</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zbar_decoder_get_data(IntPtr decoder);
        /// <summary>
        /// retrieve length of binary data.
        /// </summary>
        /// <returns>the length of the decoded data or 0 if no new data available</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint zbar_decoder_get_data_length(IntPtr decoder);
        /// <summary>
        /// retrieve last decoded symbol type.
        /// </summary>
        /// <param name="decoder"></param>
        /// <returns>zbar_symbol_type_t SymbolType</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zbar_decoder_get_type(IntPtr decoder);
        /// <summary>
        /// retrieve modifier flags for the last decoded symbol.
        /// </summary>
        /// <returns>a bitmask indicating which characteristics were detected during decoding</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint zbar_decoder_get_modifiers(IntPtr decoder);
        /// <summary>
        /// retrieve last decode direction.
        /// </summary>
        /// <returns>1 for forward and -1 for reverse 0 if the decode direction is unknown or does not apply</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zbar_decoder_get_direction(IntPtr decoder);
        /// <summary>
        /// decoder data handler callback function.
        /// called by decoder when new data has just been decoded
        /// </summary>
        public delegate void zbar_decoder_handler(IntPtr decoder);
        /// <summary>
        /// setup data handler callback.
        /// the registered function will be called by the decoder
        /// just before zbar_decode_width() returns a non-zero value.
        /// pass a NULL value to disable callbacks.
        /// </summary>
        /// <returns>the previously registered handler</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern zbar_decoder_handler zbar_decoder_set_handler(IntPtr decoder, zbar_decoder_handler handler);
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void zbar_decoder_set_userdata(IntPtr decoder, IntPtr userdata);
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zbar_decoder_get_userdata(IntPtr decoder);
        #endregion Decoder interface

        #region Scanner interface
        /// <summary>
        /// constructor
        /// </summary>
        /// <returns></returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr zbar_scanner_create();
        /// <summary>
        /// destructor
        /// </summary>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void zbar_scanner_destroy(IntPtr scanner);
        /// <summary>
        /// clear all scanner state.also resets an associated decoder
        /// </summary>
        /// <returns>zbar_symbol_type_t SymbolType</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zbar_scanner_reset(IntPtr scanner);
        /// <summary>
        /// mark start of a new scan pass. resets color to ::SPACE.
        /// also updates an associated decoder.
        /// when not using callback handlers, the return value should
        /// be checked the same as zbar_scan_y()
        /// call zbar_scanner_flush() at least twice before calling this
        /// method to ensure no decode results are lost
        /// </summary>
        /// <returns>zbar_symbol_type_t SymbolType</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zbar_scanner_new_scan(IntPtr scanner);
        /// <summary>
        /// flush scanner processing pipeline.
        /// forces current scanner position to be a scan boundary.
        /// call multiple times (max 3) to completely flush decoder.
        /// when not using callback handlers, the return value should 
        /// be checked the same as zbar_scan_y()
        /// </summary>
        /// <returns>zbar_symbol_type_t SymbolType</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zbar_scanner_flush(IntPtr scanner);
        /// <summary>
        /// process next sample intensity value.
        /// intensity (y) is in arbitrary relative units.
        /// </summary>
        /// <param name="scanner"></param>
        /// <param name="y"></param>
        /// <returns>zbar_symbol_type_t SymbolType
        /// result of zbar_decode_width() if a decoder is attached
        /// otherwise @returns (::PARTIAL) when new edge is detected
        /// or 0 (::NONE) if no new edge is detected
        /// </returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zbar_scan_y(IntPtr scanner, int y);
        /// <summary>
        /// retrieve last scanned width
        /// </summary>
        /// <returns></returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint zbar_scanner_get_width(IntPtr scanner);
        /// <summary>
        /// retrieve sample position of last edge.
        /// </summary>
        /// <returns></returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint zbar_scanner_get_edge(IntPtr scn, uint offset, int prec);
        /// <summary>
        /// retrieve last scanned color
        /// </summary>
        /// <returns>zbar_color_t Color</returns>
        [DllImport(DllName, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int zbar_scanner_get_color(IntPtr scanner);
        #endregion Scanner interface
    }
}