using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/// <summary>
/// 升级信息的具体包装
/// </summary>
[Serializable]
public class UpdateInfo
{
    public string AppName { get; set; }

    /// <summary>
    /// 应用程序版本
    /// </summary>
    public string AppVersion { get; set; }

    /// <summary>
    /// 升级需要的最低版本
    /// </summary>
    public string RequiredMinVersion { get; set; }

    public string MD5 { get; set; }

    /// <summary>
    /// 更新描述
    /// </summary>
    public string Desc { get; set; }

    public string UrlZip { get; set; }

    public string UpdateTime { get; set; }
    public string PackageSize { get; set; }
    public string PackageName { get; set; }
}

