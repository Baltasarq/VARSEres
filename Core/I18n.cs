// VARSEres (c) 2018 MIT License <baltasarq@gmail.com>

namespace VARSEres.Core {
    using System.Globalization;

    /// <summary>Internationalization.</summary>
    public static class I18n {
        /// <summary>String identifiers.</summary>
        public enum Id {
            All,
            Tag,
            RR,
            Summary,
            Loading,
            Chart,
        }

        /// <summary>English strings.</summary>
        public static string[] StrEN = {
            "All",
            "Tag",
            "RR",
            "Summary",
            "Loading",
            "Chart",
        };

        /// <summary>Spanish strings.</summary>
        public static string[] StrES = {
            "Todo",
            "Etiqueta",
            "RR",
            "Resumen",
            "Cargando",
            "Gráfica",
        };

        /// <summary>Chose among strings.</summary>
        public static CultureInfo Language {
            get {
                return locale;   
            }
            set {
                if ( value.TwoLetterISOLanguageName == "ES") {
                    locale = value;
                    strings = StrES;
                } else {
                    locale = CultureInfo.InvariantCulture;
                    strings = StrEN;
                }
            }
        }

        /// <summary>
        /// Gets the string corresponding to the given id.
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="id">An <see cref="Id"/>.</param>
        public static string Get(Id id) => strings[ (int) id ];

        static CultureInfo locale = CultureInfo.InvariantCulture;
        static string[] strings = StrEN;
    }
}
