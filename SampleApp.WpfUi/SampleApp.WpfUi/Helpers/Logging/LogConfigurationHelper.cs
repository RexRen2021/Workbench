using Microsoft.Extensions.Configuration;
using System.IO;

namespace SampleApp.WpfUi.Helpers.Logging;

public static class LogConfigurationHelper
{
    /// <summary>
    /// 从IConfiguration中获取第一个文件日志路径
    /// </summary>
    public static string? GetFirstLogFilePath(IConfiguration configuration)
    {
        // 方法1: 直接按索引访问
        var writeToSection = configuration.GetSection("Serilog:WriteTo");

        // 遍历所有WriteTo节点，找到第一个File sink
        var writeToArray = writeToSection.GetChildren();

        foreach (var writeToItem in writeToArray)
        {
            var name = writeToItem["Name"];

            if (name == "File")
            {
                var path = writeToItem["Args:path"];
                if (!string.IsNullOrEmpty(path))
                {
                    return ResolvePath(path);
                }
            }
            else if (name == "Async")
            {
                // 检查Async Sink内部配置的File Sink
                var configureSection = writeToItem.GetSection("Args:configure");
                var configureArray = configureSection.GetChildren();

                foreach (var configItem in configureArray)
                {
                    var configName = configItem["Name"];
                    if (configName == "File")
                    {
                        var path = configItem["Args:path"];
                        if (!string.IsNullOrEmpty(path))
                        {
                            return ResolvePath(path);
                        }
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 获取所有文件日志路径（包括嵌套在Async中的）
    /// </summary>
    public static List<string> GetAllLogFilePaths(IConfiguration configuration)
    {
        var paths = new List<string>();
        var writeToSection = configuration.GetSection("Serilog:WriteTo");

        foreach (var writeToItem in writeToSection.GetChildren())
        {
            FindFilePathsInSink(writeToItem, paths);
        }

        return paths.Select(ResolvePath).Distinct().ToList();
    }

    private static void FindFilePathsInSink(IConfigurationSection sinkConfig, List<string> paths)
    {
        var name = sinkConfig["Name"];

        if (name == "File")
        {
            var path = sinkConfig["Args:path"];
            if (!string.IsNullOrEmpty(path))
            {
                paths.Add(path);
            }
        }
        else if (name == "Async")
        {
            // 递归检查Async Sink内部的配置
            var configureSection = sinkConfig.GetSection("Args:configure");
            foreach (var configItem in configureSection.GetChildren())
            {
                FindFilePathsInSink(configItem, paths);
            }
        }
    }

    /// <summary>
    /// 解析路径中的环境变量和特殊标记
    /// </summary>
    private static string ResolvePath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        // 解析相对路径为绝对路径
        if (!Path.IsPathRooted(path))
        {
            // 处理路径中的环境变量
            path = Environment.ExpandEnvironmentVariables(path);

            // 将相对路径转换为基于应用程序目录的绝对路径
            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        }

        // 处理滚动日志的路径模式（如log-.txt）
        // 这里可以返回当前活动日志文件的完整路径
        return GetCurrentLogFilePathFromPattern(path);
    }

    /// <summary>
    /// 从路径模式获取当前日志文件的完整路径
    /// </summary>
    private static string GetCurrentLogFilePathFromPattern(string pathPattern)
    {
        if (string.IsNullOrEmpty(pathPattern))
            return pathPattern;

        // 检查是否是滚动日志模式（包含-）
        if (pathPattern.Contains("-") && pathPattern.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var directory = Path.GetDirectoryName(pathPattern);
                var fileName = Path.GetFileName(pathPattern);

                // 常见滚动模式：log-.txt -> log-20240101.txt
                if (fileName.Contains("-."))
                {
                    // 获取日期部分
                    var datePart = DateTime.Now.ToString("yyyyMMdd");
                    var baseName = fileName.Replace("-.txt", "");
                    var currentFileName = $"{baseName}-{datePart}.txt";

                    return Path.Combine(directory ?? string.Empty, currentFileName);
                }
            }
            catch
            {
                // 如果解析失败，返回原始路径
                return pathPattern;
            }
        }

        return pathPattern;
    }
}