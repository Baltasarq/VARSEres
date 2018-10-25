// VARSEres (c) 2018 MIT License <baltasarq@gmail.com>

namespace VARSEres.Core {
    public static class AppInfo {
        public static string Name = "VARSEres";
        public static string Author = "jbgarcia@uvigo.es";
        public static string Version = "v0.1.1";

        public static string CompleteName {
            get {
                return Name + " " + Version;
            }
        }
    }
}
