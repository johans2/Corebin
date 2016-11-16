namespace RGCommon {
    /// Gives information about the current build.
    ///
    /// For release build, it assumes that the build system has generated
    /// a class called RGVersion, and retrieves data from it.
    /// For non-release builds, just returns static data.
    public static class VersionInformation {
        /// The official version number of this build.
        /// Returns "adhoc" if it is not a proper release.
        public static string Version {
            get {
#if RELEASE_BUILD
                return RGVersion.VERSION;
#else
                return "adhoc";
#endif
            }
        }
    }
}
