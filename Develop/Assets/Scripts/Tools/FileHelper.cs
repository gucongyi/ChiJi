using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileHelper : MonoBehaviour {

    public static void createOrWriteRoleFile(string RoleName)
    {
        string RoleNameDir = Path.Combine(Application.persistentDataPath, @"RoleName");
        if (!Directory.Exists(RoleNameDir))
        {
            // Create the directory it does not exist.
            Directory.CreateDirectory(RoleNameDir);
        }
        var fullFileName = Path.Combine(RoleNameDir, "RoleNameFile");
        Stream fileHandler = null;
        if (File.Exists(fullFileName))
            fileHandler = File.Open(fullFileName, FileMode.Create);//复写方式
        else
            fileHandler = File.Create(fullFileName);
        if (fileHandler == null)
        {
            Debug.Assert(false);
            return;
        }

        using (var streamWriter = new StreamWriter(fileHandler))
        {
            streamWriter.WriteLine("RoleName:" + RoleName);
            streamWriter.Flush();
        }
        fileHandler.Close();
        fileHandler.Dispose();
        fileHandler = null;
    }

   

    public static string LoadAndRoleNameFile()
    {
        string AccountDir = Path.Combine(Application.persistentDataPath, @"RoleName");
        if (!Directory.Exists(AccountDir))
        {
            // Create the directory it does not exist.
            Directory.CreateDirectory(AccountDir);
        }
        var fullFileName = Path.Combine(AccountDir, "RoleNameFile");
        if (!File.Exists(fullFileName))
        {
            return string.Empty;
        }

        using (var fileHandler = File.Open(fullFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            if (fileHandler == null)
                return string.Empty;
            using (var fileReader = new StreamReader(fileHandler))
            {
                string line = string.Empty;
                while (!fileReader.EndOfStream)
                {
                    line = fileReader.ReadLine();
                    if (string.IsNullOrEmpty(line))
                        continue;
                    if (line.Contains("RoleName"))
                    {
                        string[] roleNameSplit = line.Split(new char[] { ':' });//设置时是：所以用:分割
                        return roleNameSplit[1];
                    }
                }
            }
        }
        return string.Empty;
    }
    
}
