using SquirrelEDID.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SquirrelEDID.Utilities
{
    /// <summary>
    /// Singleton-Container zur Verwendung in MVVM-Szenarien.
    /// </summary>
    public class IoC : NotifyPropertyChanged
    {
        #region Properties
        /// <summary>
        /// Container aller Singletons
        /// </summary>
        public static Dictionary<Type, Dictionary<string, object>> Objects { get; set; }
        /// <summary>
        /// Eigenschaft, die den Zugriff auf Singletons per Strings erlaubt
        /// </summary>
        public static IocIndexer Repository { get; set; }
        /// <summary>
        /// Sollen nicht gespeicherte Singletons beim Abfragen erstellt werden? (standard: ja)
        /// </summary>
        public static bool CreateObjects { get; set; }
        #endregion

        #region Constructors
        static IoC()
        {
            Repository = new IocIndexer();
            CreateObjects = true;
            Objects = new Dictionary<Type, Dictionary<string, object>>();
        }

        public IoC()
        {
            CreateObjects = true;
            if (Repository == null)
                Repository = new IocIndexer();
            if (Objects == null)
                Objects = new Dictionary<Type, Dictionary<string, object>>();
        }

        #endregion

        #region Methods
        /// <summary>
        /// Setzt/Überschreibt ein Singleton
        /// </summary>
        /// <typeparam name="T">Objekttyp</typeparam>
        /// <param name="key">Schlüssel (optional)</param>
        /// <param name="obj">Objekt</param>
        public static void Set<T>(string key, T obj)
        {
            Repository[typeof(T), key] = obj;
        }

        /// <summary>
        /// Setzt/Überschreibt ein Singleton
        /// </summary>
        /// <typeparam name="T">Objekttyp</typeparam>
        /// <param name="obj">Objekt</param>
        public static void Set<T>(T obj)
        {
            Set<T>("", obj);
        }

        /// <summary>
        /// List ein Singleton. Wenn CreateObjects=true wird versucht das Singleton zu erstellen.
        /// </summary>
        /// <typeparam name="T">Objekttyp</typeparam>
        /// <param name="key">Schlüssel (optional)</param>
        /// <returns>Objekt</returns>
        public static T Get<T>(string key)
        {
            return (T)Repository[typeof(T), key];
        }

        /// <summary>
        /// List ein Singleton. Wenn CreateObjects=true wird versucht das Singleton zu erstellen.
        /// </summary>
        /// <typeparam name="T">Objekttyp</typeparam>
        /// <returns>Objekt</returns>
        public static T Get<T>()
        {
            return Get<T>("");
        }

        /// <summary>
        /// Prüft, ob ein Singleton von einem bestimmten Typ enthaltne ist
        /// </summary>
        /// <typeparam name="T">Objekttyp</typeparam>
        /// <param name="key">Schlüssel (otional)</param>
        /// <returns>Objekt</returns>
        public static bool Contains<T>(string key)
        {
            if (!Objects.ContainsKey(typeof(T)))
                return false;

            return Objects[typeof(T)].ContainsKey(key);
        }

        /// <summary>
        /// Prüft, ob ein Singleton von einem bestimmten Typ enthaltne ist
        /// </summary>
        /// <typeparam name="T">Objekttyp</typeparam>
        /// <returns>Objekt</returns>
        public static bool Contains<T>()
        {
            return Contains<T>("");
        }

        /// <summary>
        /// Versucht ein Singleton für einen Objekttyp unter Anwendung des Standardkonstruktors zu erstellen
        /// </summary>
        /// <typeparam name="T">Objekttyp</typeparam>
        /// <param name="key">Schlüssel (optional)</param>
        public static void Register<T>(string key)
        {
            if (Objects.ContainsKey(typeof(T)))
            {
                IoC.Objects[typeof(T)][key] = Activator.CreateInstance<T>();
            }
            else
            {
                Dictionary<string, object> storage = new Dictionary<string, object>();
                storage[key] = Activator.CreateInstance<T>();
                IoC.Objects[typeof(T)] = storage;
            }
        }

        /// <summary>
        /// Versucht ein Singleton für einen Objekttyp unter Anwendung des Standardkonstruktors zu erstellen
        /// </summary>
        /// <typeparam name="T">Objekttyp</typeparam>
        public static void Register<T>()
        {
            Register<T>("");
        }

        /// <summary>
        /// Löscht ein Singleton (aus Objects-Container)
        /// </summary>
        /// <typeparam name="T">Objekttyp</typeparam>
        /// <param name="key">Schlüssel (optional)</param>
        public static void Unregister<T>(string key)
        {
            if (IoC.Objects.ContainsKey(typeof(T)))
                IoC.Objects[typeof(T)].Remove(key);
        }

        /// <summary>
        /// Löscht ein Singleton (aus Objects-Container)
        /// </summary>
        /// <typeparam name="T">Objekttyp</typeparam>
        public static void Unregister<T>()
        {
            Unregister<T>("");
        }

        /// <summary>
        /// Gibt die Anzahl der verwalteten Singletons eines Typs zurück
        /// </summary>
        /// <typeparam name="T">Objekttyp</typeparam>
        /// <returns>verwaltete Singletons</returns>
        public static int Count<T>()
        {
            if (!Objects.ContainsKey(typeof(T)))
                return 0;

            return Objects[typeof(T)].Keys.Count;
        }

        /// <summary>
        /// Gibt alle verwalteten Singletons eines Typs zurück
        /// </summary>
        /// <typeparam name="T">Objekttyp</typeparam>
        /// <returns>Liste aller Singletons des angegeben Typs</returns>
        public static List<T> All<T>()
        {
            if (!Objects.ContainsKey(typeof(T)))
                return new List<T>();

            return Objects[typeof(T)].Values.Cast<T>().ToList();
        }

        /// <summary>
        /// Löscht alle verwalteteten Singletons
        /// </summary>
        public static void Clear()
        {
            IoC.Objects.Clear();
        }

        #endregion

        #region Nested Class (INDEXER)
        /// <summary>
        /// Indexer-Klasse, die den Zugriff auf Singletons per Klassenname (als string) und Schlüssel (string) erlaubt.
        /// </summary>
        public class IocIndexer
        {
            public object this[Type t, string key]
            {
                get
                {
                    Dictionary<string, object> storage;
                    if (IoC.Objects.ContainsKey(t))
                    {
                        storage = IoC.Objects[t];
                    }
                    else
                    {
                        storage = new Dictionary<string, object>();
                        IoC.Objects[t] = storage;
                    }

                    if (!storage.ContainsKey(key) && IoC.CreateObjects)
                        if (t.GetConstructor(new Type[] { }) != null)
                            storage[key] = Activator.CreateInstance(t);
                        else
                            throw new InvalidOperationException(String.Format("No empty constructor found for Type: {0}", t));

                    return storage[key];
                }
                set
                {
                    Dictionary<string, object> storage = IoC.Objects.ContainsKey(t) ? IoC.Objects[t] : new Dictionary<string, object>();
                    storage[key] = value;
                    if (!IoC.Objects.ContainsKey(t))
                        IoC.Objects[t] = storage;
                }
            }

            public object this[Type t]
            {
                get
                {
                    return this[t, ""];
                }
                set
                {
                    this[t, ""] = value;
                }
            }

            public object this[string typeName, string key]
            {
                get
                {
                    Type t = FindType(typeName);
                    return this[t, key];
                }
                set
                {
                    Type t = FindType(typeName);
                    this[t, key] = value;
                }
            }

            public object this[string typeName]
            {
                get
                {
                    return this[typeName, ""];
                }
                set
                {
                    this[typeName, ""] = value;
                }
            }

            private static Type FindType(string typeName)
            {

                Type t = null;

                Dictionary<string, Type> known = Get<Dictionary<string, Type>>("KnownTypes");
                if (known.ContainsKey(typeName))
                    return known[typeName];

                if (t == null)
                {
                    t = Type.GetType(typeName);
                }
                if (t == null)
                {
                    var types = Assembly.GetEntryAssembly().GetTypes();
                    t = types.Where(type => type.Name.Equals(typeName)).FirstOrDefault();
                }
                if (t == null)
                {
                    var types = Assembly.GetCallingAssembly().GetTypes();
                    t = types.Where(type => type.Name.Equals(typeName)).FirstOrDefault();
                }
                if (t == null)
                {
                    var types = Assembly.GetExecutingAssembly().GetTypes();
                    t = types.Where(type => type.Name.Equals(typeName)).FirstOrDefault();
                }
                if (t == null) // Wir sind verzweifelt => wir durchsuchen nun ALLE gelandenen Assemblies
                {
                    foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        var types = a.GetTypes();
                        t = types.Where(type => type.Name.Equals(typeName)).FirstOrDefault();
                        if (t != null)
                            break;
                    }
                }

                //Type cachen, um spätere Zugriffe zu beschleunigen
                known[typeName] = t;

                return t;
            }

            public static void Register<T>(string key)
            {
                IoC.Register<T>(key);
            }

            public static void Register<T>()
            {
                IoC.Register<T>();
            }

            public static void Unregister<T>(string key)
            {
                IoC.Unregister<T>(key);
            }

            public static void Unregister<T>()
            {
                IoC.Unregister<T>();
            }

            public static void Clear()
            {
                IoC.Clear();
            }
        }
        #endregion
    }
}
