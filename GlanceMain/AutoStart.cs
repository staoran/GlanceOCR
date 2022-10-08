using System.Reflection;
using Furion.FriendlyException;
using Microsoft.Win32;

namespace GlanceMain;

/// <summary>
/// 自启操作
/// </summary>
public class AutoStart
{
    #region 快捷方式自启

    /// <summary>
    /// 快捷方式名称-任意自定义
    /// </summary>
    private const string QuickName = "一目十行";

    /// <summary>
    /// 自动获取系统自动启动目录
    /// </summary>
    private static string SystemStartPath => Environment.GetFolderPath(Environment.SpecialFolder.Startup);

    /// <summary>
    /// 自动获取程序完整路径
    /// </summary>
    private static string AppAllPath => Application.ExecutablePath;

    /// <summary>
    /// 自动获取桌面目录
    /// </summary>
    private static string DesktopPath => Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

    /// <summary>
    /// 通过快捷方式设置开机自动启动-只需要调用改方法就可以了参数里面的bool变量是控制开机启动的开关的，默认为开启自启启动
    /// </summary>
    /// <param name="onOff">自启开关</param>
    public static void SetMeAutoStart(bool onOff = true)
    {
        if (onOff) //开机启动
        {
            //获取启动路径应用程序快捷方式的路径集合
            List<string> shortcutPaths = GetQuickFromFolder(SystemStartPath, AppAllPath);
            //存在2个以快捷方式则保留一个快捷方式-避免重复多于
            if (shortcutPaths.Count >= 2)
            {
                for (int i = 1; i < shortcutPaths.Count; i++)
                {
                    DeleteFile(shortcutPaths[i]);
                }
            }
            else if (shortcutPaths.Count < 1) //不存在则创建快捷方式
            {
                CreateShortcut(SystemStartPath, QuickName, AppAllPath, "截图识别翻译小工具");
            }
        }
        else //开机不启动
        {
            //获取启动路径应用程序快捷方式的路径集合
            List<string> shortcutPaths = GetQuickFromFolder(SystemStartPath, AppAllPath);
            //存在快捷方式则遍历全部删除
            if (shortcutPaths.Count > 0)
            {
                foreach (string t in shortcutPaths)
                {
                    DeleteFile(t);
                }
            }
        }
        //创建桌面快捷方式-如果需要可以取消注释
        //CreateDesktopQuick(desktopPath, QuickName, appAllPath);
    }

    /// <summary>
    ///  向目标路径创建指定文件的快捷方式
    /// </summary>
    /// <param name="directory">目标目录</param>
    /// <param name="shortcutName">快捷方式名字</param>
    /// <param name="targetPath">文件完全路径</param>
    /// <param name="description">描述</param>
    /// <param name="iconLocation">图标地址</param>
    /// <param name="args">启动参数</param>
    /// <returns>成功或失败</returns>
    private static bool CreateShortcut(string directory, string shortcutName, string targetPath, string description = "",
        string iconLocation = "", string args = "")
    {
        try
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory); //目录不存在则创建
            //添加引用 Com 中搜索 Windows Script Host Object Model
            string shortcutPath = Path.Combine(directory, $"{shortcutName}.lnk"); //合成路径
            Type shellType = Type.GetTypeFromProgID("WScript.Shell") ?? throw Oops.Bah("获取 WScript.Shell 类型失败");
            dynamic shell = Activator.CreateInstance(shellType) ?? throw Oops.Bah("WScript.Shell 初始化失败");
            var shortcut = shell.CreateShortcut(shortcutPath); //创建快捷方式对象
            shortcut.TargetPath = targetPath; //指定目标路径
            shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath); //设置起始位置
            shortcut.Arguments = args;
            shortcut.WindowStyle = 1; //设置运行方式，默认为常规窗口
            shortcut.Description = description; //设置备注
            shortcut.IconLocation = string.IsNullOrWhiteSpace(iconLocation) ? targetPath : iconLocation; //设置图标路径
            shortcut.Save(); //保存快捷方式
            return true;
        }
        catch (Exception)
        {
            // string temp = ex.Message;
            // temp = "";
        }

        return false;
    }

    /// <summary>
    /// 获取指定文件夹下指定应用程序的快捷方式路径集合
    /// </summary>
    /// <param name="directory">文件夹</param>
    /// <param name="targetPath">目标应用程序路径</param>
    /// <returns>目标应用程序的快捷方式</returns>
    private static List<string> GetQuickFromFolder(string directory, string targetPath)
    {
        List<string> tempStrs = new List<string>();
        tempStrs.Clear();
        string tempStr;
        string[] files = Directory.GetFiles(directory, "*.lnk");
        if (files.Length < 1)
        {
            return tempStrs;
        }

        foreach (string t in files)
        {
            //files[i] = string.Format("{0}\\{1}", directory, files[i]);
            tempStr = GetAppPathFromQuick(t);
            if (tempStr == targetPath)
            {
                tempStrs.Add(t);
            }
        }

        return tempStrs;
    }

    /// <summary>
    /// 获取快捷方式的目标文件路径-用于判断是否已经开启了自动启动
    /// </summary>
    /// <param name="shortcutPath"></param>
    /// <returns></returns>
    private static string GetAppPathFromQuick(string shortcutPath)
    {
        //快捷方式文件的路径 = @"d:\Test.lnk";
        if (File.Exists(shortcutPath))
        {
            Type shellType = Type.GetTypeFromProgID("WScript.Shell") ?? throw Oops.Bah("获取 WScript.Shell 类型失败");
            dynamic shell = Activator.CreateInstance(shellType) ?? throw Oops.Bah("WScript.Shell 初始化失败");
            var shortcut = shell.CreateShortcut(shortcutPath);
            //快捷方式文件指向的路径.Text = 当前快捷方式文件IWshShortcut类.TargetPath;
            //快捷方式文件指向的目标目录.Text = 当前快捷方式文件IWshShortcut类.WorkingDirectory;
            return shortcut.TargetPath;
        }

        return "";
    }

    /// <summary>
    /// 根据路径删除文件-用于取消自启时从计算机自启目录删除程序的快捷方式
    /// </summary>
    /// <param name="path">路径</param>
    private static void DeleteFile(string path)
    {
        FileAttributes attr = File.GetAttributes(path);
        if (attr == FileAttributes.Directory)
        {
            Directory.Delete(path, true);
        }
        else
        {
            File.Delete(path);
        }
    }

    /// <summary>
    /// 在桌面上创建快捷方式-如果需要可以调用
    /// </summary>
    /// <param name="desktopPath">桌面地址</param>
    /// <param name="appPath">应用路径</param>
    public static void CreateDesktopQuick(string desktopPath = "", string quickName = "", string appPath = "")
    {
        List<string> shortcutPaths = GetQuickFromFolder(desktopPath, appPath);
        //如果没有则创建
        if (shortcutPaths.Count < 1)
        {
            CreateShortcut(desktopPath, quickName, appPath, "软件描述");
        }
    }

    #endregion

    #region 注册表自启

    /// <summary>
    /// 通过注册表将本程序设为开启自启
    /// </summary>
    /// <param name="onOff">自启开关</param>
    /// <returns></returns>
    public static bool SetMeStart(bool onOff)
    {
        bool isOk = false;
        string appName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
        string appPath = Application.ExecutablePath;
        isOk = SetAutoStart(onOff, appName, appPath);
        return isOk;
    }

    /// <summary>
    /// 通过注册表将应用程序设为或不设为开机启动
    /// </summary>
    /// <param name="onOff">自启开关</param>
    /// <param name="appName">应用程序名</param>
    /// <param name="appPath">应用程序完全路径</param>
    public static bool SetAutoStart(bool onOff, string appName, string appPath)
    {
        bool isOk = true;
        //如果从没有设为开机启动设置到要设为开机启动
        if (!IsExistKey(appName) && onOff)
        {
            isOk = SelfRunning(onOff, appName, @appPath);
        }
        //如果从设为开机启动设置到不要设为开机启动
        else if (IsExistKey(appName) && !onOff)
        {
            isOk = SelfRunning(onOff, appName, @appPath);
        }

        return isOk;
    }

    /// <summary>
    /// 判断注册键值对是否存在，即是否处于开机启动状态
    /// </summary>
    /// <param name="keyName">键值名</param>
    /// <returns></returns>
    private static bool IsExistKey(string keyName)
    {
        try
        {
            var exist = false;
            RegistryKey local = Registry.LocalMachine;
            RegistryKey? runs = local.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (runs == null)
            {
                RegistryKey key2 = local.CreateSubKey("SOFTWARE");
                RegistryKey key3 = key2.CreateSubKey("Microsoft");
                RegistryKey key4 = key3.CreateSubKey("Windows");
                RegistryKey key5 = key4.CreateSubKey("CurrentVersion");
                RegistryKey key6 = key5.CreateSubKey("Run");
                runs = key6;
            }

            string[] runsName = runs.GetValueNames();
            foreach (string strName in runsName)
            {
                if (strName.ToUpper() == keyName.ToUpper())
                {
                    exist = true;
                    return exist;
                }
            }

            return exist;

        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 写入或删除注册表键值对,即设为开机启动或开机不启动
    /// </summary>
    /// <param name="isStart">是否开机启动</param>
    /// <param name="exeName">应用程序名</param>
    /// <param name="path">应用程序路径带程序名</param>
    /// <returns></returns>
    private static bool SelfRunning(bool isStart, string exeName, string path)
    {
        try
        {
            RegistryKey local = Registry.LocalMachine;
            RegistryKey? key = local.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (key == null)
            {
                local.CreateSubKey("SOFTWARE//Microsoft//Windows//CurrentVersion//Run");
            }

            //若开机自启动则添加键值对
            if (isStart)
            {
                key!.SetValue(exeName, path);
                key.Close();
            }
            else //否则删除键值对
            {
                string[] keyNames = key!.GetValueNames();
                foreach (string keyName in keyNames)
                {
                    if (keyName.ToUpper() == exeName.ToUpper())
                    {
                        key.DeleteValue(exeName);
                        key.Close();
                    }
                }
            }
        }
        catch (Exception)
        {
            // string ss = ex.Message;
            return false;
            //throw;
        }

        return true;
    }

    #endregion
}