using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Data;

namespace SquirrelEDID.Utilities
{
    public class Settings
    {
        #region Fields
        private Hashtable _entries;
        private string _path;
        #endregion

        #region Properties
        public Hashtable Entries
        {
            get { return _entries; }
        }
        #endregion

        #region Constructors
        private Settings(string file)
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), file);
            _path = path;
            Load();
        }
        #endregion

        #region Methods
        public static Settings Load(string file)
        {
            return new Settings(file);
        }

        public void Load()
        {
            if (!File.Exists(_path))
            {
                _entries = new Hashtable();
                return;
            }

            using (FileStream fs = File.OpenRead(_path))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
                _entries = (Hashtable)JSON.Decode(buffer);
            }
        }

        public void Save()
        {
            if (File.Exists(_path))
                File.Delete(_path);

            using (FileStream fs = File.OpenWrite(_path))
            {
                string json = JSON.Encode(_entries);
                byte[] buffer = ASCIIEncoding.UTF8.GetBytes(json);
                fs.Write(buffer, 0, buffer.Length);
            }
        }

        public void LoadObject(object obj)
        {
            string main = null;
            Type t = obj.GetType();
            SettingsObjectAttribute mainAttr = (SettingsObjectAttribute)t.GetCustomAttributes(typeof(SettingsObjectAttribute), true).FirstOrDefault();
            if (mainAttr != null)
                main = mainAttr.Key;

            foreach (PropertyInfo pi in t.GetProperties())
            {
                SettingAttribute attr = (SettingAttribute)pi.GetCustomAttributes(typeof(SettingAttribute), true).FirstOrDefault();
                if (attr == null)
                    continue;

                object value = attr.DefaultValue;
                if (String.IsNullOrEmpty(main))
                {
                    if (ContainsKey(attr.Key))
                        value = Get(attr.Key);
                }
                else
                {
                    if (ContainsKey(main, attr.Key))
                        value = Get(main, attr.Key);
                }

                if (attr.Converter != null && attr.Converter.GetInterfaces().Any(i => i == typeof(IValueConverter)))
                {
                    IValueConverter conv = (IValueConverter)Activator.CreateInstance(attr.Converter);
                    value = conv.Convert(value, null, null, null);
                }

                pi.SetValue(obj, value, null);
            }
        }

        public void SaveObject(object obj)
        {
            string main = null;
            Type t = obj.GetType();
            SettingsObjectAttribute mainAttr = (SettingsObjectAttribute)t.GetCustomAttributes(typeof(SettingsObjectAttribute), true).FirstOrDefault();
            if (mainAttr != null)
                main = mainAttr.Key;

            foreach (PropertyInfo pi in t.GetProperties())
            {
                SettingAttribute attr = (SettingAttribute)pi.GetCustomAttributes(typeof(SettingAttribute), true).FirstOrDefault();
                if (attr == null)
                    continue;

                object value = pi.GetValue(obj, null);
                if (attr.Converter != null && attr.Converter.GetInterfaces().Any(i => i == typeof(IValueConverter)))
                {
                    IValueConverter conv = (IValueConverter)Activator.CreateInstance(attr.Converter);
                    value = conv.ConvertBack(value, null, null, null);
                }

                if (String.IsNullOrEmpty(main))
                    Set(value, attr.Key);
                else
                    Set(value, main, attr.Key);
            }
        }

        private Hashtable Dive(bool create, params string[] keys)
        {
            if (keys.Length < 1)
                return null;

            Hashtable table = _entries;
            if (keys.Length > 1)
            {
                for (int i = 0; i < keys.Length - 1; i++)
                {
                    if (!table.ContainsKey(keys[i]))
                        if (create)
                            table[keys[i]] = new Hashtable();
                        else
                            return null;

                    if (!(table[keys[i]] is Hashtable))
                        return null;

                    table = (Hashtable)table[keys[i]];
                }
            }
            return table;
        }

        public bool ContainsKey(params string[] keys)
        {
            Hashtable table = Dive(false, keys);
            if (table == null)
                return false;

            return table.ContainsKey(keys[keys.Length - 1]);
        }

        public bool IsType<T>(params string[] keys)
        {
            Hashtable table = Dive(false, keys);
            if (table == null)
                return false;

            if (!table.ContainsKey(keys[keys.Length - 1]))
                return false;

            return table[keys[keys.Length - 1]] is T;
        }

        public object Get(params string[] keys)
        {
            Hashtable table = Dive(false, keys);
            if (table == null)
                return table;

            if (!table.ContainsKey(keys[keys.Length - 1]))
                return null;

            return table[keys[keys.Length - 1]];
        }

        public bool CanGet<T>(params string[] keys)
        {
            Hashtable table = Dive(false, keys);
            if (table == null)
                return false;

            if (!table.ContainsKey(keys[keys.Length - 1]))
                return false;

            if (table[keys[keys.Length - 1]] is T)
                return true;

            try
            {
                T val = (T)Convert.ChangeType(table[keys[keys.Length - 1]], typeof(T));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public T Get<T>(params string[] keys)
        {
            Hashtable table = Dive(false, keys);
            if (table == null)
                return default(T);

            if (!table.ContainsKey(keys[keys.Length - 1]))
                return default(T);

            if (table[keys[keys.Length - 1]] is T)
                return (T)table[keys[keys.Length - 1]];

            try
            {
                return (T)Convert.ChangeType(table[keys[keys.Length - 1]], typeof(T));
            }
            catch (Exception ex)
            {
                //Messenger<Exception>.Invoke(ex);
                return default(T);
            }
        }

        public void Set(object obj, params string[] keys)
        {
            Hashtable table = Dive(true, keys);
            if (table == null)
                return;

            table[keys[keys.Length - 1]] = obj;
        }
        #endregion
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SettingAttribute : Attribute
    {
        #region Properties
        public string Key { get; set; }
        public Type Converter { get; set; }
        public object DefaultValue { get; set; }
        #endregion

        #region Constructors
        public SettingAttribute(string key, Type converter, object defaultValue)
        {
            Key = key;
            Converter = converter;
            DefaultValue = defaultValue;
        }

        public SettingAttribute(string key, Type converter)
            : this(key, converter, null)
        {

        } 

        public SettingAttribute(string key)
            : this(key, null, null)
        {

        }

        public SettingAttribute()
            : this(null, null, null)
        {

        }
        #endregion
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class SettingsObjectAttribute : Attribute
    {
        #region Properties
        public string Key { get; set; } 
        #endregion

        #region Constructors
        public SettingsObjectAttribute(string key)
        {
            Key = key;
        } 
        #endregion
    }
}
