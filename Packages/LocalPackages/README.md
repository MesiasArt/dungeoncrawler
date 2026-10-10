# ProBuilder material repair

This archive contains Unity ProBuilder 6.1.3 with only the unavailable HDRP
AssetVersion subasset (script GUID da692e001514ec24dbc4cca1949ff7e8) removed from
Content/Resources/Materials/ProBuilderDefault.mat and Content/Material/Checker.mat.
Material data, GUIDs, code, and upstream licenses are preserved.

The project uses URP and does not include the HDRP script. These stale subassets
caused missing-script warnings during material import. The local archive keeps
the repair reproducible without changing Library/PackageCache.
