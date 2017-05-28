using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using NGettext;
using NGettext.Loaders;
using UnityEngine;

namespace Paladin.Tools
{
    public interface ILocale : ICatalog
    {
        bool IsLoaded { get; }
        void Load(Action OnComplete);
    }

    public class Locale : Catalog, ILocale
    {
        public bool IsLoaded { get; private set; }

        //private static Locale Instance;
        //public static ILocale I { get { return Instance ?? (Instance = new Locale()); }}

        public Locale() : base(GetCultureInfo())
        {
            var path = "locales/" + CultureInfo.Name + "/LC_MESSAGES/messages.mo";
            var bytes = Resources.Load<TextAsset>(path).bytes;
            new MoLoader(new MemoryStream(bytes)).Load(this);

            Debug.Log(">> TRANSLATIONS COUNT: " + Translations.Keys.Count);
        }

        public void Load(Action OnComplete)
        {
            // TEMP VERSION WITH LOADER
            //var www = new WWW("https://admin.agoradoxa.net/paladin/defs/develop/last/ru/messages.mo");
            //yield return www;
            //new MoLoader(new MemoryStream(www.bytes)).Load(this);
            IsLoaded = true;
            OnComplete.Invoke();
        }

        /**
         * from gettext:
         * Set of available languages.
         * af ast bg ca cs da de el eo es fi fr ga gl hr hu id it ja ky lv ms mt nb nl pl pt pt_BR ro ru sk sl sr sv tr uk vi zh_CN zh_HK zh_TW
         *
         * from unity, system language:
         * see: https://docs.unity3d.com/ScriptReference/SystemLanguage.html
         */
        private static readonly Dictionary<string, string> Available = new Dictionary<string, string>
        {
            {"English", "en"},
            {"Russian", "ru"}
        };

        private static string Fallback { get { return "English"; }}

        private static CultureInfo GetCultureInfo()
        {
            CultureInfo info;

#if UNITY_EDITOR
            info = new CultureInfo("en"); // FYI: можно установить любой язык для тестов в эдиторе
#else
                var systemLanguage = Application.systemLanguage.ToString();
                info = new CultureInfo(Available.ContainsKey(systemLanguage)
                    ? Available[systemLanguage]
                    : Available[Fallback]);
#endif

            Debug.Log(">> CULTURE INFO: " + info.Name);
            return info;
        }
    }
}