using System;
#if !bcad
using Autodesk.AutoCAD.ApplicationServices;
#else
using Bricscad.ApplicationServices;
#endif

namespace KojtoCAD.Utilities.Interfaces
{
    /// <summary>
    /// 网络跟踪接口
    /// </summary>
    interface IWebTracker
    {
        /// <summary>
        /// 跟踪命令使用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void TrackCommandUsage(object sender, CommandEventArgs args);

        /// <summary>
        /// 跟踪异常
        /// </summary>
        /// <param name="exception"></param>
        void TrackException(Exception exception);
    }
}
