namespace KojtoCAD.Updater.Interfaces
{
    /// <summary>
    /// 应用程序配置提供者接口
    /// </summary>
    public interface IAppConfigurationProvider
    {
        string GetBlobContainerUri();

        string GetBlobContainerName();

        string GetKojtoCadVirtualDirectoryName();

        /// <summary>
        /// 获取 Program Files 目录
        /// </summary>
        /// <returns></returns>
        string GetProgramFilesDir();

        /// <summary>
        /// 获取 KojtoCad 插件目录
        /// </summary>
        /// <returns></returns>
        string GetKojtoCadPluginDir();

        /// <summary>
        /// 获取网络跟踪是否可用
        /// </summary>
        /// <returns></returns>
        bool WebTrackerIsEnabled();
    }
}
