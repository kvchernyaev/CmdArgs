#region usings
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    internal static class Extensions
    {
        public static void SplitPairByEquality(this string s, out string name, out string value)
        {
            if (s == null)
            {
                name = value = null;
                return;
            }
            int ieq = s.IndexOf("=");
            if (ieq < 0)
            {
                name = s;
                value = null;
            }
            else
            {
                name = s.Substring(0, ieq);
                value = s.Substring(ieq + 1);
            }
        }


        public static bool IsTypeDerivedFromGenericType(this Type typeToCheck, Type genericType)
        {
            if (typeToCheck == typeof(object))
                return false;
            if (typeToCheck == null)
                return false;
            if (typeToCheck.IsGenericType &&
                typeToCheck.GetGenericTypeDefinition() == genericType)
                return true;
            return IsTypeDerivedFromGenericType(typeToCheck.BaseType, genericType);
        }


        public static MemberInfo[] GetFieldsAndProps(this Type t)
        {
            MemberInfo[] fields = t.GetFields();
            MemberInfo[] properties = t.GetProperties();

            return fields.Concat(properties).ToArray();
        }


        public static Argument GetArgument(this MemberInfo mi)
        {
            var attr = (ArgumentAttribute) mi
                .GetCustomAttributes(typeof(ArgumentAttribute), true)
                .FirstOrDefault();
            if (attr == null)
                return null;
            return attr.Argument;
        }


        public static Type GetFieldType(this MemberInfo mi)
        {
            if (mi is FieldInfo fi) return fi.FieldType;
            if (mi is PropertyInfo pi) return pi.PropertyType;
            throw new ConfException(
                $"MemberInfo.GetFieldType(): type [{mi.GetType().Name}] is not accepted");
        }


        public static object GetValue(this MemberInfo mi, object target)
        {
            if (mi is FieldInfo fi) return fi.GetValue(target);
            if (mi is PropertyInfo pi) return pi.GetValue(target);
            throw new ConfException(
                $"MemberInfo.GetValue(): type [{mi.GetType().Name}] is not accepted");
        }


        public static void SetValue(this MemberInfo mi, object target, object argVal)
        {
            if (mi is FieldInfo fi)
                fi.SetValue(target, argVal);
            else if (mi is PropertyInfo pi)
                pi.SetValue(target, argVal);
            else
                throw new ConfException(
                    $"MemberInfo.SetValue(): type [{mi.GetType().Name}] is not accepted");
        }


        public static string[] ReadFileAsArgs(this FileInfo fi)
        {
            string contents = File.ReadAllText(fi.FullName);
            string[] lines = contents.Split(new[] {Environment.NewLine},
                StringSplitOptions.RemoveEmptyEntries);

            string[] fileCmdArgs = lines.SelectMany(SplitAsArgs).ToArray();
            return fileCmdArgs;
        }


        static string[] SplitAsArgs(string argsString)
        {
            var args = new List<string>();
            var currentArg = new StringBuilder();
            var escape = false;
            var inQuote = false;
            var hadQuote = false;
            var prevCh = '\0';
            // Iterate all characters from the input string
            foreach (char ch in argsString)
            {
                if (ch == '\\' && !escape)
                    escape = true;
                else if (ch == '\\' && escape)
                {
                    // Double backslash, keep one
                    currentArg.Append(ch);
                    escape = false;
                }
                else if (ch == '"' && !escape)
                {
                    // Toggle quoted range
                    inQuote = !inQuote;
                    hadQuote = true;
                    if (inQuote && prevCh == '"') currentArg.Append(ch);
                }
                else if (ch == '"' && escape)
                {
                    // Backslash-escaped quote, keep it
                    currentArg.Append(ch);
                    escape = false;
                }
                else if (char.IsWhiteSpace(ch) && !inQuote)
                {
                    if (escape)
                    {
                        // Add pending escape char
                        currentArg.Append('\\');
                        escape = false;
                    }
                    // Accept empty arguments only if they are quoted
                    if (currentArg.Length > 0 || hadQuote) args.Add(currentArg.ToString());
                    // Reset for next argument
                    currentArg.Clear();
                    hadQuote = false;
                }
                else
                {
                    if (escape)
                    {
                        // Add pending escape char
                        currentArg.Append('\\');
                        escape = false;
                    }
                    // Copy character from input, no special meaning
                    currentArg.Append(ch);
                }
                prevCh = ch;
            }
            // Save last argument
            if (currentArg.Length > 0 || hadQuote) args.Add(currentArg.ToString());
            return args.ToArray();
        }
    }
}