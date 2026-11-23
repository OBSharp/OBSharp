namespace OBSharp.Runtime;

/// <summary>
/// OBS 模块宿主的路径配置。未显式配置时，路径由 OBS 提供的模块信息填充。
/// </summary>
internal sealed class OBSharpHostOptions
{
    /// <summary>
    /// OBS 模块二进制路径，用于解析插件列表中的 <c>$bin</c> 路径。
    /// </summary>
    public string BinaryPath { get; internal set; } = default!;

    /// <summary>
    /// OBS 模块数据目录，用于保存插件列表及解析其中的 <c>$data</c> 路径。
    /// </summary>
    public string DataPath { get; internal set; } = default!;
}
