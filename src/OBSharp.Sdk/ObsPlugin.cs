using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OBSharp;

/// <summary>
/// 创建插件实例时传给插件基类的宿主信息。
/// </summary>
public sealed class ObsPluginInfo
{
    /// <summary>
    /// 插件程序集所在的目录。
    /// </summary>
    public string PluginFolder { get; }

    internal ObsPluginInfo(string pluginFolder)
    {
        PluginFolder = pluginFolder;
    }
}

/// <summary>
/// OB# 插件的基类。宿主从插件程序集的导出类型中选择一个非抽象子类，
/// 通过依赖注入创建实例，并在插件生命周期中调用 <see cref="Load"/>、
/// <see cref="PostLoad"/> 和 <see cref="Unload"/>。
/// </summary>
public abstract class ObsPlugin(ObsPluginInfo info)
{
    /// <summary>
    /// 插件程序集所在的目录。
    /// </summary>
    public string PluginFolder { get; } = info.PluginFolder;

    /// <summary>
    /// 插件名称。默认使用插件程序集的名称；程序集未提供名称时，依次回退到类型命名空间和类型名称。
    /// </summary>
    public virtual string Name
    {
        get
        {
            var type = GetType();
            return type.Assembly.GetName().Name ?? type.Namespace ?? type.Name;
        }
    }

    /// <summary>
    /// 插件描述。默认返回 <see langword="null"/>。
    /// </summary>
    public virtual string? Description => null;

    /// <summary>
    /// 初始化插件并报告是否成功。
    /// </summary>
    /// <returns>初始化成功时返回 <see langword="true"/>；否则返回 <see langword="false"/>。</returns>
    public abstract bool Load();

    /// <summary>
    /// 在宿主完成初始插件加载后调用，可用于初始化依赖其他插件的功能。
    /// 宿主运行期间加载的新插件会在 <see cref="Load"/> 之后立即收到此回调。
    /// </summary>
    public virtual void PostLoad() { }

    /// <summary>
    /// 卸载插件并释放插件自行持有的资源。
    /// </summary>
    /// <remarks>
    /// 此方法正常返回后，宿主会对实现了 <see cref="IDisposable"/> 的插件实例调用 <see cref="IDisposable.Dispose"/>，
    /// 并请求卸载插件程序集的加载上下文。
    /// </remarks>
    public virtual void Unload() { }

    /// <summary>
    /// 创建以插件名称和目录为默认值的 .NET Host 应用构建器，并配置 OBS 日志提供程序。
    /// </summary>
    /// <param name="name">应用名称；省略时使用 <see cref="Name"/>。</param>
    /// <param name="contentRootPath">内容根目录；省略时使用 <see cref="PluginFolder"/>。</param>
    /// <returns>配置完成的 Host 应用构建器。</returns>
    protected virtual IHostApplicationBuilder CreateBuilder(string? name = null, string? contentRootPath = null)
    {
        name ??= Name;
        contentRootPath ??= PluginFolder;

        var builder = Host.CreateApplicationBuilder(settings: new()
        {
            ApplicationName = name,
            ContentRootPath = contentRootPath,
        });

        // 将插件日志写入 OBS 日志，并使用应用名称标识日志来源。
        builder.Logging.AddProvider(new ObsLoggerProvider(name));

        return builder;
    }
}
