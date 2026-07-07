# Readme

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

# **Misc note:**

I changed the package ID from `AuthenticodeExaminer` to `Forked-AuthenticodeExaminer` to ensure that the local NuGet package is restored instead of the version published on nuget.org.


# **Verification:**

https://github.com/enda-mullally/AuthenticodeExaminer/actions/runs/28903026509

Artifact : AuthenticodeExaminer-Nuget-Package (zipped)
Digest	 : sha256:33add0e9490892312758d9cd488f173a1be6404e6a234bcbaec518cd3262bbe1

-> Forked-AuthenticodeExaminer.0.4.0.nupkg
   sha256:b8b255e0dc943413b06625bb7a5d30e6a96ce00272eaf5226ad935d0379acb7b
