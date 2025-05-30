﻿using OptifineInstaller;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class Utils
{
    public static string[] Tokenize(string str, string delim)
    {
        List<string> list = new List<string>();

        string[] tokens = str.Split(delim.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        list.AddRange(tokens);

        return list.ToArray();
    }

    public static ushort ReadUnsignedShort(BinaryReader reader)
    {
        int high = reader.ReadByte();
        int low = reader.ReadByte();
        return (ushort)((high << 8) | low);
    }

    public static int ReadInt(BinaryReader reader)
    {
        return (reader.ReadByte() << 24)
                     | (reader.ReadByte() << 16)
                     | (reader.ReadByte() << 8)
                     | reader.ReadByte();
    }

    public static long ReadLong(BinaryReader reader)
    {
        return ((long)reader.ReadByte() << 56)
                            | ((long)reader.ReadByte() << 48)
                            | ((long)reader.ReadByte() << 40)
                            | ((long)reader.ReadByte() << 32)
                            | ((long)reader.ReadByte() << 24)
                            | ((long)reader.ReadByte() << 16)
                            | ((long)reader.ReadByte() << 8)
                            | (long)reader.ReadByte();
    }

    public static byte[] ReadAll(Stream inputStream)
    {
        MemoryStream baos = new MemoryStream();
        byte[] buf = new byte[1024];

        while (true)
        {
            int len = inputStream.Read(buf, 0, buf.Length);
            if (len <= 0)
                break;
            baos.Write(buf, 0, len);
        }

        inputStream.Close();

        byte[] bytes = baos.ToArray();

        return bytes;
    }

    public static int Find(byte[] buffer, byte[] pattern)
    {
        return Find(buffer, 0, pattern);
    }

    public static int Find(byte[] buffer, int startIndex, byte[] pattern)
    {
        for (int i = startIndex; i < buffer.Length - pattern.Length; i++)
        {
            bool found = true;

            for (int pos = 0; pos < pattern.Length; pos++)
            {
                if (pattern[pos] != buffer[i + pos])
                {
                    found = false;
                    break;
                }
            }

            if (found)
                return i;
        }

        return -1;
    }

    public static string RemovePrefix(string str, string prefix)
    {
        if (str == null || prefix == null)
        {
            return str;
        }

        if (str.StartsWith(prefix))
        {
            str = str.Substring(prefix.Length);
        }

        return str;
    }

    public static string ArrayToCommaSeparatedString(object[] arr)
    {
        if (arr == null)
            return "";

        StringBuilder buf = new StringBuilder();

        for (int i = 0; i < arr.Length; i++)
        {
            object val = arr[i];

            if (i > 0)
                buf.Append(", ");

            if (val == null)
            {
                buf.Append("null");
            }
            else if (val.GetType().IsArray)
            {
                buf.Append('[');

                if (val is object[] v)
                {
                    buf.Append(ArrayToCommaSeparatedString(v));
                }
                else
                {
                    int len = ((Array)val).Length;
                    for (int ai = 0; ai < len; ai++)
                    {
                        if (ai > 0)
                            buf.Append(", ");
                        buf.Append(((Array)val).GetValue(ai));
                    }
                }

                buf.Append(']');
            }
            else
            {
                buf.Append(val);
            }
        }

        return buf.ToString();
    }

    public static bool IsLateVersion(OptifineVersion version)
    {
        Version v1 = NormalizeVersion(version.MinecraftVersion);
        Version v2 = NormalizeVersion("1.17.1");

        int result = v1.CompareTo(v2);

        if (result > 0)
        {
            return true;

        }
        else if (result == 0)
        {
            return version.OptifineEdition != "HD_U_G9";
        }
        return false;
    }

    public static string GetLaunchwrapperVersionLegacy(OptifineVersion version)
    {
        Version v1 = NormalizeVersion(version.MinecraftVersion);
        Version v2 = NormalizeVersion("1.7.10");

        int result = v1.CompareTo(v2);

        if (result > 0)
        {
            return "1.12";
        }
        else if (result == 0)
        {
            var alphabet = version.OptifineEdition.Split('_')[2].Substring(0, 1);
            if (alphabet == "E")
                return "1.12";
        }
        return "1.7";
    }

    public static bool IsLegacyVersion(OptifineVersion version)
    {
        Version v1 = NormalizeVersion(version.MinecraftVersion);
        Version v2 = NormalizeVersion("1.8.9");

        int result = v1.CompareTo(v2);

        if (result > 0)
        {
            return false;
        }
        else if (result == 0)
        {
            var alphabet = version.OptifineEdition.Split('_')[2].Substring(0, 1);
            return !(alphabet == "L" || alphabet == "M");
        }
        return true;
    }

    static Version NormalizeVersion(string versionStr)
    {
        var parts = versionStr.Split('.');
        while (parts.Length < 4)
        {
            versionStr += ".0";
            parts = versionStr.Split('.');
        }

        return new Version(versionStr);
    }
}