`Forked-AuthenticodeExaminer.0.4.0.nupkg` is a fork of:

https://github.com/vcsjones/AuthenticodeExaminer

Created by Kevin Jones, licensed under the MIT License.

**Fork repository:**

https://github.com/enda-mullally/AuthenticodeExaminer

This fork includes the following important security fixes:

* `System.Security.Cryptography.Pkcs` 8.0.1 → 10.0.9
* `System.Security.Cryptography.Xml` 8.0.1 → 10.0.9

Additionally, it includes miscellaneous package upgrades and a manual GitHub Actions workflow to build the NuGet package using Visual Studio 2018.

See the full diff:

https://github.com/vcsjones/AuthenticodeExaminer/compare/main...enda-mullally:AuthenticodeExaminer:main

# **Miscellaneous note**

I changed the package ID in the `.nuspec` file from `AuthenticodeExaminer` to `Forked-AuthenticodeExaminer` to ensure that the local NuGet package is restored instead of the version published on NuGet.org.
